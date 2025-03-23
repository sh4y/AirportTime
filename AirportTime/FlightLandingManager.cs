// Modify FlightLandingManager to handle special flights appropriately
using System;
using System.Collections.Generic;
using System.Linq;
using AirportTime;

public class FlightLandingManager : IFlightLandingManager
{
    private readonly RunwayManager runwayManager;
    private readonly Treasury treasury;
    private readonly ModifierManager modifierManager;
    private readonly GameLogger gameLogger;
    private readonly EventSystem eventSystem;
    private readonly Weather weather;
    private readonly IRandomGenerator randomGenerator;
    private readonly ITickManager tickManager;
    private readonly EmergencyFlightHandler emergencyFlightHandler;
    
    // Reference to the airport view controller for UI interactions
    private AirportViewController viewController;
    public bool ForceManualForEmergencies { get; set; } = true;
    public bool ForceManualForSpecialFlights { get; set; } = true; // New property for special flights

    // Define the landing mode
    public enum LandingMode
    {
        Automatic,
        Manual
    }
    
    // Default to automatic landings
    public LandingMode CurrentLandingMode { get; set; } = LandingMode.Automatic;
    
    // Event for when a flight lands successfully
    public event Action<Flight, Runway, bool, int> OnFlightLanded;

    public FlightLandingManager(
        RunwayManager runwayManager,
        Treasury treasury,
        ModifierManager modifierManager,
        GameLogger gameLogger,
        EventSystem eventSystem,
        IRandomGenerator randomGenerator,
        ITickManager tickManager,
        EmergencyFlightHandler emergencyFlightHandler)
    {
        this.runwayManager = runwayManager;
        this.treasury = treasury;
        this.modifierManager = modifierManager;
        this.gameLogger = gameLogger;
        this.eventSystem = eventSystem;
        this.randomGenerator = randomGenerator;
        this.tickManager = tickManager;
        this.emergencyFlightHandler = emergencyFlightHandler;
        this.weather = new Weather(randomGenerator);
    }
    
    /// <summary>
    /// Sets the view controller used for UI interactions
    /// </summary>
    /// <param name="controller">The controller to use</param>
    public void SetViewController(AirportViewController controller)
    {
        this.viewController = controller;
    }

    /// <summary>
    /// Processes a flight landing based on the current landing mode.
    /// Emergency flights and special flights are always processed manually regardless of the landing mode.
    /// </summary>
    /// <param name="flight">The flight to process.</param>
    /// <param name="currentTick">The current game tick.</param>
    /// <returns>True if landing was successful, false otherwise.</returns>
    public bool ProcessFlight(Flight flight, int currentTick)
    {
        // Calculate how many ticks past schedule we are
        int ticksPastSchedule = currentTick - flight.ScheduledLandingTime;
        bool isOnTime = ticksPastSchedule <= 0;
        
        if (ticksPastSchedule > 0)
        {
            // Flight is past its scheduled time, add delay
            gameLogger.Log($"Flight {flight.FlightNumber} is {ticksPastSchedule} ticks past scheduled landing time.");
            eventSystem.TriggerDelayEvent(flight, ticksPastSchedule, "Past scheduled landing time", currentTick);
            // Don't return - continue trying to land
        }

        // Check if runways are available first
        if (!runwayManager.CanLand(flight.Plane))
        {
            gameLogger.Log($"Flight {flight.FlightNumber} delayed — no available runway.");
            eventSystem.TriggerDelayEvent(flight, 5, "No available runway", currentTick);
            return false;
        }

        // Check if this is an emergency flight that requires manual handling
        bool isEmergency = flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency;
        
        // Register emergency flight if not already registered
        if (isEmergency && !emergencyFlightHandler.IsActiveEmergency(flight.FlightNumber))
        {
            emergencyFlightHandler.RegisterEmergencyFlight(flight, currentTick);
        }
        
        // Check if this is a special flight - display notification if so
        if (flight.IsSpecial && viewController != null)
        {
            viewController.ShowNotification($"Special Flight {flight.FlightNumber} requires manual landing! Double revenue!", true);
        }
        
        // Process according to landing mode and flight type
        // Special flights must be landed manually if ForceManualForSpecialFlights is true
        bool forceManual = (isEmergency && ForceManualForEmergencies) || 
                           (flight.IsSpecial && ForceManualForSpecialFlights);
                           
        if (CurrentLandingMode == LandingMode.Manual || forceManual)
        {
            return ProcessManualLanding(flight, currentTick, isOnTime);
        }
        else
        {
            return ProcessAutomaticLanding(flight, currentTick, isOnTime);
        }
    }

    /// <summary>
    /// Processes an automatic landing by selecting the best available runway.
    /// </summary>
    private bool ProcessAutomaticLanding(Flight flight, int currentTick, bool isOnTime)
    {
        var availableRunway = runwayManager.GetAvailableRunway(flight.Plane);
        if (availableRunway == null)
        {
            gameLogger.Log($"Flight {flight.FlightNumber} delayed — no available runway found during automatic landing attempt.");
            eventSystem.TriggerDelayEvent(flight, 5, "No suitable runway for automatic landing", currentTick);
            return false;
        }

        // Double-check that the runway is still not occupied
        if (availableRunway.IsOccupied)
        {
            gameLogger.Log($"Flight {flight.FlightNumber} delayed — selected runway {availableRunway.Name} became occupied.");
            eventSystem.TriggerDelayEvent(flight, 5, "Selected runway became occupied", currentTick);
            return false;
        }

        if (!flight.AttemptLanding(availableRunway))
        {
            gameLogger.Log($"Flight {flight.FlightNumber} failed automatic landing attempt.");
            eventSystem.TriggerDelayEvent(flight, 5, "Failed landing attempt", currentTick);
            return false;
        }

        CompleteSuccessfulLanding(flight, availableRunway, currentTick, isOnTime);
        
        // If this was an emergency flight, mark it as handled
        if (flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency)
        {
            emergencyFlightHandler.MarkEmergencyHandled(flight.FlightNumber);
        }
        
        return true;
    }

    /// <summary>
    /// Processes a manual landing by prompting the user to select a runway through the view controller.
    /// </summary>
    private bool ProcessManualLanding(Flight flight, int currentTick, bool isOnTime)
    {
        // Get all available runways that can handle this plane
        List<Runway> availableRunways = runwayManager.GetAvailableRunways(flight.Plane);

        if (availableRunways == null || availableRunways.Count == 0)
        {
            gameLogger.Log($"Flight {flight.FlightNumber} delayed — no runways available for manual selection.");
            eventSystem.TriggerDelayEvent(flight, 5, "No available runways for manual selection", currentTick);
            return false;
        }

        // Pause the game while making selection
        bool wasRunning = tickManager.IsRunning();
        if (wasRunning)
        {
            tickManager.Pause();
            gameLogger.Log($"Game paused for runway selection (Flight {flight.FlightNumber})");
        }

        try
        {
            // Use the view controller to prompt for runway selection
            Runway selectedRunway = null;
            
            if (viewController != null)
            {
                // Add indication in the prompt that this is a special flight
                if (flight.IsSpecial)
                {
                    // The viewController will show this is a special flight in the UI
                    gameLogger.Log($"SPECIAL FLIGHT {flight.FlightNumber} requires manual landing! Double revenue!");
                }
                
                selectedRunway = viewController.PromptRunwaySelection(flight, availableRunways);
            }
            else
            {
                // Fallback to auto-selection if no view controller is available
                selectedRunway = availableRunways.FirstOrDefault();
                gameLogger.Log($"No view controller available, auto-selecting runway {selectedRunway?.Name ?? "None"}");
            }
            
            // If the user selected a runway
            if (selectedRunway != null)
            {
                // Double-check that the runway is still not occupied
                if (selectedRunway.IsOccupied)
                {
                    gameLogger.Log($"Flight {flight.FlightNumber} delayed — selected runway {selectedRunway.Name} became occupied.");
                    eventSystem.TriggerDelayEvent(flight, 5, "Selected runway became occupied", currentTick);
                    return false;
                }
                
                // Attempt landing
                if (!flight.AttemptLanding(selectedRunway))
                {
                    gameLogger.Log($"Flight {flight.FlightNumber} failed manual landing attempt on {selectedRunway.Name}.");
                    eventSystem.TriggerDelayEvent(flight, 5, "Failed landing attempt", currentTick);
                    return false;
                }
                
                CompleteSuccessfulLanding(flight, selectedRunway, currentTick, isOnTime);
                
                // If this was an emergency flight, mark it as handled
                if (flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency)
                {
                    emergencyFlightHandler.MarkEmergencyHandled(flight.FlightNumber);
                }
                
                return true;
            }
            else
            {
                // User chose to delay or auto-select
                eventSystem.TriggerDelayEvent(flight, 5, "User decision", currentTick);
                return false;
            }
        }
        finally
        {
            // Resume game if it was running before
            if (wasRunning)
            {
                tickManager.Start();
                gameLogger.Log("Game resumed after runway selection");
            }
        }
    }
    
    private void CompleteSuccessfulLanding(Flight flight, Runway runway, int currentTick, bool isOnTime)
    {
        // Handle the landing in the runway manager
        runwayManager.HandleLanding(runway.Name, weather, 10, flight.FlightNumber);
    
        // Calculate and add revenue
        double revenue = modifierManager.CalculateRevenue(flight, currentTick);
        
        // Special success message for special flights
        string specialMessage = flight.IsSpecial ? " [SPECIAL FLIGHT - DOUBLE REVENUE]" : "";
        
        treasury.AddFunds(revenue, $"Flight Revenue{specialMessage}");
    
        // Log the successful landing
        gameLogger.Log($"Flight {flight.FlightNumber}{specialMessage} landed successfully on {runway.Name} and generated {revenue:C} in revenue.");
    
        // Trigger landing event for experience system
        OnFlightLanded?.Invoke(flight, runway, isOnTime, currentTick);
        
        // Show a notification through the view controller if available
        if (viewController != null)
        {
            string message = flight.IsSpecial 
                ? $"SPECIAL Flight {flight.FlightNumber} landed successfully and generated {revenue:C} (DOUBLE REVENUE!)"
                : $"Flight {flight.FlightNumber} landed successfully and generated {revenue:C}";
                
            viewController.ShowNotification(message, flight.IsSpecial);
        }
    }

    /// <summary>
    /// Toggles between automatic and manual landing modes.
    /// </summary>
    public void ToggleLandingMode()
    {
        CurrentLandingMode = CurrentLandingMode == LandingMode.Automatic 
            ? LandingMode.Manual 
            : LandingMode.Automatic;
        
        gameLogger.Log($"Landing mode switched to {CurrentLandingMode}.");
        
        // Show a notification through the view controller if available
        viewController?.ShowNotification($"Landing mode switched to {CurrentLandingMode}");
    }
}