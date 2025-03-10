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
        EventSystem = new EventSystem(RandomGenerator); // Assumes DefaultRandomGenerator implements IRandomGenerator
        AirportRevenue = new Revenue();
        ModifierManager = new ModifierManager(AirportRevenue);
    }

    public void Tick(int currentTick)
    {
        // Accumulate gold at every tick
        Treasury.AccumulateGold();

        // Process scheduled flights for the current tick
        ProcessScheduledFlights(currentTick);
    }

    private void ProcessScheduledFlights(int currentTick)
    {
        var scheduledFlights = FlightScheduler.GetFlightsAtTick(currentTick);
        foreach (var flight in scheduledFlights)
        {
            ProcessFlight(flight);
        }
    }

    private void ProcessFlight(Flight flight)
    {
        if (RunwayManager.CanLand(flight.Plane))
        {
            var availableRunway = RunwayManager.GetAvailableRunway(flight.Plane);
            if (!flight.AttemptLanding(availableRunway))
            {
                GameLogger.Log($"Flight {flight.FlightNumber} failed landing.");
                EventSystem.TriggerDelayEvent(flight);
                return;
            }
            RunwayManager.HandleLanding(availableRunway.Name, new Weather(RandomGenerator), 10);
            double revenue = ModifierManager.CalculateRevenue(flight);
            Treasury.AddFunds(revenue, "Flight Revenue");
            GameLogger.Log($"Flight {flight.FlightNumber} landed successfully.");
        }
        else
        {
            GameLogger.Log($"Flight {flight.FlightNumber} delayed — no available runway.");
            EventSystem.TriggerDelayEvent(flight);
        }
    }

    private bool IsRandomEventTick(int currentTick)
    {
        // Random events are triggered every 60 ticks (i.e., once per minute)
        return currentTick % 60 == 0;
    }
}
