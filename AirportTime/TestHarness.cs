public class TestHarness
{
    // A reference to the main Airport instance.
    private readonly Airport airport;

    // Individual services retrieved from the Airport.
    private readonly Shop shop;
    private readonly Treasury treasury;
    private readonly RunwayManager runwayManager;
    private readonly EventScheduler eventScheduler;
    private readonly GameLogger logger;

    // Accept a single Airport dependency.
    public TestHarness(Airport airport)
    {
        this.airport = airport;

        // Extract the required services from the Airport.
        this.shop = airport.Shop;
        this.treasury = airport.Treasury;
        this.runwayManager = airport.RunwayManager;
        this.logger = airport.GameLogger;

        this.eventScheduler = new EventScheduler();

        ScheduleEvents();
    }

    private void ScheduleEvents()
    {
        // Schedule a Tier 1 Runway unlock event.
        eventScheduler.ScheduleEvent(new ScheduledEvent(10, tick =>
        {
            shop.BuyItem("Tier 1 Runway", airport);
            runwayManager.UnlockRunway(RunwayTier.Tier1);
            logger.Log($"[Tick {tick}] Tier 1 Runway unlocked.");
        }));

        // Schedule a Tier 2 Runway unlock event.
        eventScheduler.ScheduleEvent(new ScheduledEvent(20, tick =>
        {
            if (treasury.GetBalance() >= 10000)
            {
                shop.BuyItem("Tier 2 Runway", airport);
                runwayManager.UnlockRunway(RunwayTier.Tier2);
                logger.Log($"[Tick {tick}] Tier 2 Runway unlocked.");
            }
        }));

        // Schedule a flight landing event.
        eventScheduler.ScheduleEvent(new ScheduledEvent(15, tick =>
        {
            var plane = new Plane("Boeing777", PlaneSize.Large, 50000);
            var flight = new Flight("AA123", plane, FlightType.Commercial, FlightPriority.Standard, tick, 300);
            var availableRunway = runwayManager.GetAvailableRunway(plane);
            flight.AttemptLanding(availableRunway);
            logger.Log($"[Tick {tick}] Flight AA123 attempted landing.");
        }));
    }

    public void StartTest()
    {
        Console.WriteLine("Test Harness Initialized. Starting test...");

        for (int tick = 1; tick <= 60; tick++)
        {
            treasury.AccumulateGold();
            eventScheduler.ProcessEvents(tick);
            logger.Log($"Test Tick {tick} completed.");
        }

        Console.WriteLine("Test completed.");
    }
}
