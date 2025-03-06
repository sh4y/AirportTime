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

    public Airport(string name, double startingGold)
    {
        Name = name;
        Treasury = new Treasury(startingGold);
        RunwayManager = new RunwayManager(new RunwayMaintenanceSystem());
        Shop = new Shop(Treasury);
        FlightScheduler = new FlightScheduler();
        EventSystem = new EventSystem(new RandomGenerator()); // Assumes DefaultRandomGenerator implements IRandomGenerator
        GameLogger = new GameLogger();
        ModifierManager = new ModifierManager();
    }

    public void Tick(int currentTick)
    {
        // Accumulate gold at every tick
        Treasury.AccumulateGold();

        // Process scheduled flights for the current tick
        ProcessScheduledFlights(currentTick);

        // Trigger a random event every 60 ticks
        if (IsRandomEventTick(currentTick))
        {
            EventSystem.TriggerRandomEvent(this);
        }

        // Perform runway maintenance and other routine operations
        RunwayManager.PerformMaintenance(Treasury, GameLogger);
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
            flight.AttemptLanding(availableRunway);

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
