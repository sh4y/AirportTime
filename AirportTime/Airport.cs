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
        EventSystem = new EventSystem(RandomGenerator, GameLogger);
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
        // Process flights scheduled for this tick
        var scheduledFlights = FlightScheduler.GetFlightsAtTick(currentTick);
        
        // Also process any delayed flights that haven't landed yet
        var delayedFlights = FlightScheduler.GetUnlandedFlights()
            .Where(f => f.ScheduledLandingTime < currentTick);

        // Process all flights
        foreach (var flight in scheduledFlights.Concat(delayedFlights))
        {
            ProcessFlight(flight, currentTick);
        }
    }

    private void ProcessFlight(Flight flight, int currentTick)
    {
        // Calculate how many ticks past schedule we are
        int ticksPastSchedule = currentTick - flight.ScheduledLandingTime;
        if (ticksPastSchedule > 0)
        {
            // Flight is past its scheduled time, add delay
            GameLogger.Log($"Flight {flight.FlightNumber} is {ticksPastSchedule} ticks past scheduled landing time.");
            EventSystem.TriggerDelayEvent(flight, ticksPastSchedule, "Past scheduled landing time", currentTick);
            // Don't return - continue trying to land
        }

        if (RunwayManager.CanLand(flight.Plane))
        {
            var availableRunway = RunwayManager.GetAvailableRunway(flight.Plane);
            if (!flight.AttemptLanding(availableRunway))
            {
                GameLogger.Log($"Flight {flight.FlightNumber} failed landing.");
                EventSystem.TriggerDelayEvent(flight, 5, "Failed landing attempt", currentTick);
                return;
            }
            RunwayManager.HandleLanding(availableRunway.Name, new Weather(RandomGenerator), 10);
            double revenue = ModifierManager.CalculateRevenue(flight, currentTick);
            Treasury.AddFunds(revenue, "Flight Revenue");
            GameLogger.Log($"Flight {flight.FlightNumber} landed successfully.");
        }
        else
        {
            GameLogger.Log($"Flight {flight.FlightNumber} delayed — no available runway.");
            EventSystem.TriggerDelayEvent(flight, 5, "No available runway", currentTick);
        }
    }

    private bool IsRandomEventTick(int currentTick)
    {
        // Random events are triggered every 60 ticks (i.e., once per minute)
        return currentTick % 60 == 0;
    }
}
