using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the process of landing flights, offering both automatic and manual runway selection.
/// </summary>
public class FlightLandingManager
{
    private readonly RunwayManager runwayManager;
    private readonly Treasury treasury;
    private readonly ModifierManager modifierManager;
    private readonly GameLogger gameLogger;
    private readonly EventSystem eventSystem;
    private readonly Weather weather;
    private readonly IRandomGenerator randomGenerator;
    private readonly TickManager tickManager;
    
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
        TickManager tickManager)
    {
        this.runwayManager = runwayManager;
        this.treasury = treasury;
        this.modifierManager = modifierManager;
        this.gameLogger = gameLogger;
        this.eventSystem = eventSystem;
        this.randomGenerator = randomGenerator;
        this.tickManager = tickManager;
        this.weather = new Weather(randomGenerator);
    }

    /// <summary>
    /// Processes a flight landing based on the current landing mode.
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

        // Process according to landing mode
        switch (CurrentLandingMode)
        {
            case LandingMode.Automatic:
                return ProcessAutomaticLanding(flight, currentTick, isOnTime);
            case LandingMode.Manual:
                return ProcessManualLanding(flight, currentTick, isOnTime);
            default:
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
        return true;
    }

    /// <summary>
    /// Processes a manual landing by prompting the user to select a runway.
    /// Game will pause during selection.
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
            // Display landing request and runway options
            Console.WriteLine();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Flight {flight.FlightNumber} requires landing. Please assign a runway:");
            Console.WriteLine(new string('-', 60));
            
            // Display flight details
            Console.WriteLine($"Flight Details: {flight.FlightNumber} ({flight.Type}, {flight.Priority})");
            Console.WriteLine($"Aircraft: {flight.Plane.PlaneID} (Size: {flight.Plane.Size}, Required Runway Length: {flight.Plane.RequiredRunwayLength}m)");
            Console.WriteLine($"Passengers: {flight.Passengers}");
            Console.WriteLine(new string('-', 60));
            
            // Display runway options
            Console.WriteLine("Available Runways:");
            for (int i = 0; i < availableRunways.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableRunways[i].Name} " +
                                  $"(Length: {availableRunways[i].Length}m, " +
                                  $"Wear: {availableRunways[i].WearLevel}%)");
            }
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{availableRunways.Count + 1}. Auto-select best runway");
            Console.WriteLine($"{availableRunways.Count + 2}. Delay flight (5 ticks)");
            Console.WriteLine(new string('=', 60));
            
            // Get user input (no timeout needed since game is paused)
            Console.Write("Enter your selection: ");
            string input = Console.ReadLine();

            // Process the selection
            bool result = false;
            
            if (int.TryParse(input, out int selection))
            {
                // Auto-select option
                if (selection == availableRunways.Count + 1)
                {
                    Console.WriteLine("Auto-selecting best runway...");
                    result = ProcessAutomaticLanding(flight, currentTick, isOnTime);
                }
                // Delay option
                else if (selection == availableRunways.Count + 2)
                {
                    Console.WriteLine("Flight delayed by controller decision.");
                    eventSystem.TriggerDelayEvent(flight, 5, "Controller decision", currentTick);
                    result = false;
                }
                // Valid runway selection
                else if (selection >= 1 && selection <= availableRunways.Count)
                {
                    Runway selectedRunway = availableRunways[selection - 1];
                    
                    // Double-check that the runway is still not occupied
                    if (selectedRunway.IsOccupied)
                    {
                        Console.WriteLine($"Runway {selectedRunway.Name} has become occupied since selection.");
                        gameLogger.Log($"Flight {flight.FlightNumber} delayed — selected runway {selectedRunway.Name} became occupied.");
                        eventSystem.TriggerDelayEvent(flight, 5, "Selected runway became occupied", currentTick);
                        result = false;
                    }
                    // Attempt landing
                    else if (!flight.AttemptLanding(selectedRunway))
                    {
                        gameLogger.Log($"Flight {flight.FlightNumber} failed manual landing attempt on {selectedRunway.Name}.");
                        eventSystem.TriggerDelayEvent(flight, 5, "Failed landing attempt", currentTick);
                        result = false;
                    }
                    else
                    {
                        CompleteSuccessfulLanding(flight, selectedRunway, currentTick, isOnTime);
                        result = true;
                    }
                }
                else
                {
                    // Invalid selection number
                    Console.WriteLine("Invalid selection. Auto-selecting runway...");
                    result = ProcessAutomaticLanding(flight, currentTick, isOnTime);
                }
            }
            else
            {
                // Invalid input (not a number)
                Console.WriteLine("Invalid input. Auto-selecting runway...");
                result = ProcessAutomaticLanding(flight, currentTick, isOnTime);
            }
            
            // Show simple message and auto-continue after 3 seconds
            ShowRunwayAnimationWithAutoResume(result);
            
            return result;
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
    
    /// <summary>
    /// Completes the landing process for a successful landing.
    /// </summary>
    private void CompleteSuccessfulLanding(Flight flight, Runway runway, int currentTick, bool isOnTime)
    {
        
        // Handle the landing in the runway manager
        runwayManager.HandleLanding(runway.Name, weather, 10);
        
        // Calculate and add revenue
        double revenue = modifierManager.CalculateRevenue(flight, currentTick);
        treasury.AddFunds(revenue, "Flight Revenue");
        
        // Log the successful landing
        gameLogger.Log($"Flight {flight.FlightNumber} landed successfully on {runway.Name} and generated {revenue:C} in revenue.");
        
        // Trigger landing event for experience system
        OnFlightLanded?.Invoke(flight, runway, isOnTime, currentTick);
    }

    /// <summary>
    /// Displays a simple message and auto-continues after 3 seconds
    /// </summary>
    private void ShowRunwayAnimationWithAutoResume(bool landingSuccessful)
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 60));
        Console.WriteLine(landingSuccessful 
            ? "Landing successful. Resuming in 3 seconds..." 
            : "Flight delayed. Resuming in 3 seconds...");
        Console.WriteLine(new string('-', 60));
        
        // Pause for 3 seconds then auto-continue
        System.Threading.Thread.Sleep(3000);
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
    }
}