using AirportTime;

/// <summary>
/// Implementation of IGameMetricsProvider that uses the GameMetrics class
/// </summary>
public class GameMetricsProvider : IGameMetricsProvider
{
    private readonly GameMetrics metrics;
    
    /// <summary>
    /// Creates a new metrics provider
    /// </summary>
    /// <param name="airport">The airport to get metrics for</param>
    /// <param name="logger">Game logger for log metrics</param>
    public GameMetricsProvider(Airport airport, IGameLogger logger)
    {
        this.metrics = new GameMetrics(airport, logger);
    }
    
    /// <summary>
    /// Gets airport metrics
    /// </summary>
    public AirportMetrics GetAirportMetrics() => metrics.AirportMetrics;
    
    /// <summary>
    /// Gets emergency metrics
    /// </summary>
    public EmergencyMetrics GetEmergencyMetrics() => metrics.EmergencyMetrics;
    
    /// <summary>
    /// Gets experience metrics
    /// </summary>
    public ExperienceMetrics GetExperienceMetrics() => metrics.ExperienceMetrics;
    
    /// <summary>
    /// Gets failure metrics
    /// </summary>
    public FailureMetrics GetFailureMetrics() => metrics.FailureMetrics;
    
    /// <summary>
    /// Gets runway metrics
    /// </summary>
    public RunwayMetrics GetRunwayMetrics() => metrics.RunwayMetrics;
    
    /// <summary>
    /// Gets flight metrics
    /// </summary>
    public FlightMetrics GetFlightMetrics() => metrics.FlightMetrics;
    
    /// <summary>
    /// Gets modifier metrics
    /// </summary>
    public ModifierMetrics GetModifierMetrics() => metrics.ModifierMetrics;
    
    /// <summary>
    /// Gets achievement metrics
    /// </summary>
    public AchievementMetrics GetAchievementMetrics() => metrics.AchievementMetrics;
    
    /// <summary>
    /// Gets log metrics
    /// </summary>
    public LogMetrics GetLogMetrics() => metrics.LogMetrics;
    
    /// <summary>
    /// Gets game time information
    /// </summary>
    public GameTimeInfo GetTimeInfo(int currentTick) => metrics.AirportMetrics.GetTimeInfo(currentTick);
}