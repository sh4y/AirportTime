public class TestHarness
{
    private RunwayManager runwayManager;
    private Shop shop;
    private Treasury treasury;
    private EventScheduler eventScheduler;

    public TestHarness()
    {
        treasury = new Treasury(90, 3);  // Start with 0 gold
        GameLogger g = new GameLogger();
        shop = new Shop(treasury, g);
        var maintenanceSystem = new RunwayMaintenanceSystem(); // Create maintenance system
        runwayManager = new RunwayManager(maintenanceSystem);
        eventScheduler = new EventScheduler();

        // Schedule events using the eventScheduler
        eventScheduler.ScheduleEvent(new ScheduledEvent(10, tick =>
        {
            shop.BuyItem("Tier 1 Runway");
            runwayManager.UnlockRunway(RunwayTier.Tier1);
            Console.WriteLine($"[Tick {tick}] Tier 1 Runway unlocked.");
        }));

        eventScheduler.ScheduleEvent(new ScheduledEvent(20, tick =>
        {
            if (treasury.GetBalance() >= 10000)
            {
                shop.BuyItem("Tier 2 Runway");
                runwayManager.UnlockRunway(RunwayTier.Tier2);
                Console.WriteLine($"[Tick {tick}] Tier 2 Runway unlocked.");
            }
        }));

        eventScheduler.ScheduleEvent(new ScheduledEvent(15, tick =>
        {
            var plane = new Plane("Boeing777", PlaneSize.Large, 50000);
            var flight = new Flight("AA123", plane, FlightType.Commercial, FlightPriority.Standard, tick, 300);
            var availableRunway = runwayManager.GetAvailableRunway(plane);
            flight.AttemptLanding(availableRunway);
            Console.WriteLine($"[Tick {tick}] Flight AA123 attempted landing.");
        }));
    }

    // Start running the simulation with the scheduled events
    public void StartTest()
    {
        Console.WriteLine("Test Harness Initialized. Starting test...");

        for (int tick = 1; tick <= 60; tick++)
        {
            // Accumulate gold at each tick
            treasury.AccumulateGold();

            // Process any events scheduled for this tick
            eventScheduler.ProcessEvents(tick);

            // Log tick progress
            Console.WriteLine($"Test Tick {tick} completed.");
        }

        Console.WriteLine("Test completed.");
    }
}
