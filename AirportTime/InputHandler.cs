public class InputHandler
{
    private readonly Airport airport;
    private readonly FlightGenerator flightGenerator;
    private readonly TickManager tickManager;
    private readonly GameLogger logger;

    public InputHandler(Airport airport, FlightGenerator flightGenerator, TickManager tickManager, GameLogger logger)
    {
        this.airport = airport;
        this.flightGenerator = flightGenerator;
        this.tickManager = tickManager;
        this.logger = logger;
    }

    public void HandleInput(int currentTick)
    {
        if (!Console.KeyAvailable)
            return;

        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.Q:
                tickManager.Pause();
                logger.Log("Game paused by player (Q pressed).");
                break;

            case ConsoleKey.F:
                var flight = flightGenerator.GenerateRandomFlight(currentTick, 10);
                airport.FlightScheduler.ScheduleFlight(flight, flight.ScheduledLandingTime);
                logger.Log($"✈️ Scheduled random flight {flight.FlightNumber} for tick {flight.ScheduledLandingTime}.");
                break;

            case ConsoleKey.S:
                OpenShop();
                break;

            case ConsoleKey.R:
                RepairRunways();
                break;
        }
    }

    private void OpenShop()
    {
        tickManager.Pause();
        airport.Shop.ViewItemsForSale();
        Console.WriteLine("Enter item name to purchase:");
        var itemName = Console.ReadLine();

        if (airport.Shop.BuyItem(itemName, airport) == PurchaseResult.Success)
        {
            if (itemName.Contains("Runway"))
                airport.RunwayManager.UnlockRunway(RunwayTier.Tier1);

            logger.Log($"✅ Purchased {itemName}.");
        }
        else
        {
            logger.Log($"❌ Could not purchase {itemName}. Insufficient funds or invalid item.");
        }

        tickManager.Start();
    }

    private void RepairRunways()
    {
        logger.Log($"🛠️ Manual runway maintenance initiated by user.");
        airport.RunwayManager.PerformMaintenance(airport.Treasury);
    }
}


