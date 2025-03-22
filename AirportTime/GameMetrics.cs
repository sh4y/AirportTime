

// A facade to access all metrics in one place

namespace AirportTime;

public class GameMetrics
{
    public AirportMetrics AirportMetrics { get; }
    public EmergencyMetrics EmergencyMetrics { get; }
    public ExperienceMetrics ExperienceMetrics { get; }
    public FailureMetrics FailureMetrics { get; }
    public RunwayMetrics RunwayMetrics { get; }
    public FlightMetrics FlightMetrics { get; }
    public ModifierMetrics ModifierMetrics { get; }
    public AchievementMetrics AchievementMetrics { get; }
    public LogMetrics LogMetrics { get; }

    public GameMetrics(Airport airport, IGameLogger logger)
    {
        AirportMetrics = new AirportMetrics(airport);
        EmergencyMetrics = new EmergencyMetrics(airport);
        ExperienceMetrics = new ExperienceMetrics(airport);
        FailureMetrics = new FailureMetrics(airport);
        RunwayMetrics = new RunwayMetrics(airport);
        FlightMetrics = new FlightMetrics(airport);
        ModifierMetrics = new ModifierMetrics(airport);
        AchievementMetrics = new AchievementMetrics(airport);
        LogMetrics = new LogMetrics(logger);
    }
}