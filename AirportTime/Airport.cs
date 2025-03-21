public class Airport
{
    public string Name { get; private set; }
    public Treasury Treasury { get; private set; }
    public RunwayManager RunwayManager { get; private set; }
    public Shop Shop { get; private set; }
    public FlightScheduler FlightScheduler { get; private set; }
    public EventSystem EventSystem { get; private set; }
    public GameLogger GameLogger { get; private set; }
    public ModifierManager ModifierManager { get; private set; }
    public FlightLandingManager LandingManager { get; private set; }
    private Revenue AirportRevenue { get; set; }
    private readonly IRandomGenerator RandomGenerator;

    public Airport(string name, double startingGold)
    {
        Name = name;
        GameLogger = new GameLogger("GameLogs.db");
        Treasury = new Treasury(GameLogger, new TransactionLogStore());
        RunwayManager = new RunwayManager(new RunwayMaintenanceSystem(), GameLogger);
        Shop = new Shop(Treasury, GameLogger);
        FlightScheduler = new FlightScheduler();
        RandomGenerator = new RandomGenerator();
        EventSystem = new EventSystem(RandomGenerator, GameLogger);
        AirportRevenue = new Revenue();
        ModifierManager = new ModifierManager(AirportRevenue, GameLogger);
        LandingManager = null;
    }

    public void Tick(int currentTick)
    {
        // Accumulate gold at every tick
        Treasury.AccumulateGold();
        
        // Update runway occupation status
        RunwayManager.UpdateRunwaysStatus();

        // Process scheduled flights for the current tick
        ProcessScheduledFlights(currentTick);
    }

    private void ProcessScheduledFlights(int currentTick)
    {
        // Process flights scheduled for this tick
        var scheduledFlights = FlightScheduler.GetFlightsAtTick(currentTick);
        
        // Also process any delayed flights that haven't landed yet
        var delayedFlights = FlightScheduler.GetUnlandedFlights()
            .Where(f => f.ScheduledLandingTime < currentTick);

        // Process all flights
        foreach (var flight in scheduledFlights.Concat(delayedFlights))
        {
            LandingManager.ProcessFlight(flight, currentTick);
        }
    }

    public void ToggleLandingMode()
    {
        LandingManager.ToggleLandingMode();
    }

    // Method to set the LandingManager after TickManager is available
    public void SetLandingManager(TickManager tickManager)
    {
        LandingManager = new FlightLandingManager(
            RunwayManager,
            Treasury,
            ModifierManager,
            GameLogger,
            EventSystem,
            RandomGenerator,
            tickManager
        );
    }
    
    private bool IsRandomEventTick(int currentTick)
    {
        // Random events are triggered every 60 ticks (i.e., once per minute)
        return currentTick % 60 == 0;
    }
}