using System;
using System.Collections.Generic;

/// <summary>
/// Controller that mediates between the Airport model and the UI view.
/// This class gathers all the necessary data from the Airport and its subsystems
/// and passes it to the view for display.
/// </summary>
public class AirportViewController
{
    private readonly IAirportView view;
    private readonly GameMetrics metrics;
    
    /// <summary>
    /// Creates a new controller with the given view and airport
    /// </summary>
    /// <param name="view">The view implementation to use</param>
    /// <param name="airport">The airport to display</param>
    public AirportViewController(IAirportView view, Airport airport)
    {
        this.view = view;
        this.metrics = new GameMetrics(airport, airport.GameLogger);
    }
    
    /// <summary>
    /// Updates the view with the current state of the airport
    /// </summary>
    /// <param name="airport">The airport to display</param>
    /// <param name="currentTick">The current game tick</param>
    public void UpdateView(Airport airport, int currentTick)
    {
        // Skip if game is over
        if (airport.IsGameOver) return;
        
        // Clear the view
        view.ClearDisplay();
        
        // Update the view in sequence
        UpdateHeader(airport, currentTick);
        UpdateEmergencies(airport, currentTick);
        UpdateMainContent(airport, currentTick);
        view.DisplayControls();
    }
    
    /// <summary>
    /// Updates the header section of the view
    /// </summary>
    private void UpdateHeader(Airport airport, int currentTick)
    {
        var airportInfo = metrics.AirportMetrics;
        var timeInfo = metrics.AirportMetrics.GetTimeInfo(currentTick);
        
        // Update header and landing info
        view.UpdateHeaderInfo(
            airportInfo.Name,
            timeInfo,
            airportInfo.GoldBalance,
            airportInfo.GetWeatherInfo());
            
        view.UpdateLandingInfo(
            airportInfo.CurrentLandingMode,
            airportInfo.ActiveFlightsCount);
    }
    
    /// <summary>
    /// Updates the emergency section of the view if needed
    /// </summary>
    private void UpdateEmergencies(Airport airport, int currentTick)
    {
        var emergencies = metrics.EmergencyMetrics.GetActiveEmergencies(currentTick);
        if (emergencies.Count > 0)
        {
            view.DisplayEmergencies(emergencies, currentTick);
        }
    }
    
    /// <summary>
    /// Updates the main content of the view
    /// </summary>
    private void UpdateMainContent(Airport airport, int currentTick)
    {
        // Experience and flight section
        var xpMetrics = metrics.ExperienceMetrics;
        view.UpdateExperienceDisplay(
            xpMetrics.CurrentLevel,
            xpMetrics.CurrentXP,
            xpMetrics.NextLevelXP,
            xpMetrics.ProgressPercentage,
            xpMetrics.GetEstimatedTimeToNextLevel(),
            xpMetrics.EfficiencyBonus,
            xpMetrics.GetNextUnlock());
            
        view.DisplayUpcomingFlights(
            metrics.FlightMetrics.GetUpcomingFlights(4));
            
        // Failures and modifiers section
        view.UpdateFailuresDisplay(
            metrics.FailureMetrics.GetTopFailures());
            
        view.UpdateModifiersDisplay(
            metrics.ModifierMetrics.GetActiveModifiers());
            
        // Runways and achievements section
        view.UpdateRunwaysDisplay(
            metrics.RunwayMetrics.GetRunwayInfo());
            
        view.UpdateAchievementsDisplay(
            metrics.AchievementMetrics.GetRecentAchievements(),
            metrics.AchievementMetrics.TotalUnlockedAchievements);
            
        // Get recent logs from the logger (this would need to be implemented)
        List<string> recentLogs = GetRecentLogs(airport);
        view.UpdateLogsDisplay(recentLogs);
    }
    
    /// <summary>
    /// Gets recent logs from the logger
    /// </summary>
    private List<string> GetRecentLogs(Airport airport)
    {
        // In a real implementation, you'd get this from the logger
        // For now, we'll return an empty list
        return new List<string>();
    }
    
    /// <summary>
    /// Handles the process of selecting a runway for landing
    /// </summary>
    /// <param name="flight">The flight that needs to land</param>
    /// <param name="availableRunways">List of available runways</param>
    /// <returns>The selected runway or null if auto-selection or delay was chosen</returns>
    public Runway PromptRunwaySelection(Flight flight, List<Runway> availableRunways)
    {
        return view.PromptRunwaySelection(flight, availableRunways);
    }
    
    /// <summary>
    /// Shows a notification to the user
    /// </summary>
    /// <param name="message">The message to show</param>
    /// <param name="isImportant">Whether the message is important</param>
    public void ShowNotification(string message, bool isImportant = false)
    {
        view.ShowNotification(message, isImportant);
    }
}