public class FlightGenerationService
{
    private readonly FlightGenerator _flightGenerator;
    private readonly FlightScheduler _flightScheduler;
    private readonly GameLogger _logger;
    private readonly ExperienceSystem _experienceSystem;
    private readonly RunwayManager _runwayManager;
    private readonly EventScheduler _flightEventScheduler = new EventScheduler();
    
    public FlightGenerationService(
        FlightGenerator flightGenerator,
        FlightScheduler flightScheduler,
        GameLogger logger,
        ExperienceSystem experienceSystem,
        RunwayManager runwayManager)
    {
        _flightGenerator = flightGenerator;
        _flightScheduler = flightScheduler;
        _logger = logger;
        _experienceSystem = experienceSystem;
        _runwayManager = runwayManager;
    }
    
    public void GenerateFlightsIfNeeded(int currentTick)
    {
        // Process any scheduled flight generation events
        _flightEventScheduler.ProcessEvents(currentTick);

        // Only generate new batches every 12 ticks
        if (currentTick % 12 == 0)
        {
            int airportLevel = _experienceSystem.CurrentLevel;
            int runwayCount = _runwayManager.GetRunwayCount();
            int flightsToGenerate = Math.Max(1, (int)Math.Ceiling(runwayCount * 2.5));
            
            _logger.Log($"Generating {flightsToGenerate} flights (2.5 × {runwayCount} runways)");
            
            // Stagger the flights over the next several ticks
            for (int i = 0; i < flightsToGenerate; i++)
            {
                int staggerInterval = (int)Math.Ceiling(12.0 / Math.Max(1, flightsToGenerate));
                int staggerTicks = i * staggerInterval;
                int scheduledTick = currentTick + staggerTicks;
                
                _flightEventScheduler.ScheduleEvent(new ScheduledEvent(scheduledTick, (tick) => {
                    // Generate a single flight
                    Flight flight = _flightGenerator.GenerateRandomFlight(tick, airportLevel);
                    
                    // Schedule the flight
                    _flightScheduler.ScheduleFlight(flight, flight.ScheduledLandingTime);
                    _logger.Log($"Scheduled {flight.FlightNumber} ({flight.Type}, {flight.Priority}) with {flight.Passengers} passengers for tick {flight.ScheduledLandingTime}");
                }));
            }
        }
    }
}