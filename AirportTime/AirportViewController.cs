namespace AirportTime;

/// <summary>
/// Controller that mediates between the Airport model and the UI view
/// using dependency injection.
/// </summary>
public class AirportViewController : IAirportViewController
{
    private readonly IAirportView view;
    private readonly Airport airport;
    private readonly IGameMetricsProvider metricsProvider;
    
    /// <summary>
    /// Creates a new controller with the given dependencies
    /// </summary>
    /// <param name="view">The view implementation to use</param>
    /// <param name="airport">The airport to display</param>
    /// <param name="metricsProvider">The metrics provider</param>
    public AirportViewController(
        IAirportView view, 
        Airport airport, 
        IGameMetricsProvider metricsProvider)
    {
        this.view = view ?? throw new ArgumentNullException(nameof(view));
        this.airport = airport ?? throw new ArgumentNullException(nameof(airport));
        this.metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
    }
    
    /// <summary>
    /// Alternative constructor that creates its own metrics provider
    /// </summary>
    /// <param name="view">The view implementation to use</param>
    /// <param name="airport">The airport to display</param>
    public AirportViewController(IAirportView view, Airport airport)
        : this(view, airport, new GameMetricsProvider(airport, airport.GameLogger))
    {
    }
    
    /// <summary>
    /// Updates the view with the current state of the airport
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    public void UpdateView(int currentTick)
    {
        // Skip if game is over
        if (airport.IsGameOver) return;
        
        // Clear the view
        view.ClearDisplay();
        
        // Update the view in sequence
        UpdateHeader(currentTick);
        UpdateEmergencies(currentTick);
        UpdateMainContent(currentTick);
        view.DisplayControls();
    }
    
    /// <summary>
    /// Updates the header section of the view
    /// </summary>
    private void UpdateHeader(int currentTick)
    {
        var airportInfo = metricsProvider.GetAirportMetrics();
        var timeInfo = metricsProvider.GetTimeInfo(currentTick);
        
        // Update header and landing info
        view.UpdateHeaderInfo(
            airport.Name,
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
    private void UpdateEmergencies(int currentTick)
    {
        var emergencies = metricsProvider.GetEmergencyMetrics().GetActiveEmergencies(currentTick);
        if (emergencies.Count > 0)
        {
            view.DisplayEmergencies(emergencies, currentTick);
        }
    }
    
    /// <summary>
    /// Updates the main content of the view
    /// </summary>
    private void UpdateMainContent(int currentTick)
    {
        // Experience and flight section
        var xpMetrics = metricsProvider.GetExperienceMetrics();
        view.UpdateExperienceDisplay(
            xpMetrics.CurrentLevel,
            xpMetrics.CurrentXP,
            xpMetrics.NextLevelXP,
            xpMetrics.ProgressPercentage,
            xpMetrics.GetEstimatedTimeToNextLevel(),
            xpMetrics.EfficiencyBonus,
            xpMetrics.GetNextUnlock());
            
        view.DisplayUpcomingFlights(
            metricsProvider.GetFlightMetrics().GetUpcomingFlights(4));
            
        // Failures and modifiers section
        view.UpdateFailuresDisplay(
            metricsProvider.GetFailureMetrics().GetTopFailures());
            
        view.UpdateModifiersDisplay(
            metricsProvider.GetModifierMetrics().GetActiveModifiers());
            
        // Runways and achievements section
        view.UpdateRunwaysDisplay(
            metricsProvider.GetRunwayMetrics().GetRunwayInfo());
            
        view.UpdateAchievementsDisplay(
            metricsProvider.GetAchievementMetrics().GetRecentAchievements(),
            metricsProvider.GetAchievementMetrics().TotalUnlockedAchievements);
            
        // Get recent logs from the logger
        List<string> recentLogs = metricsProvider.GetLogMetrics().GetRecentLogs(4);
        view.UpdateLogsDisplay(recentLogs);
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