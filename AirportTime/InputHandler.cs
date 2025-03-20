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
        Console.WriteLine("Enter item ID or name to purchase:");
        var input = Console.ReadLine();

        // Try to parse the input as an integer
        if (int.TryParse(input, out int itemId))
        {
            // Attempt to buy the item by ID
            if (airport.Shop.BuyItem(itemId, airport) == PurchaseResult.Success)
            {
                logger.Log($"✅ Purchased item with ID {itemId}.");
            }
            else
            {
                logger.Log($"❌ Could not purchase item with ID {itemId}. Insufficient funds or invalid item.");
            }
        }
        else
        {
            // Attempt to buy the item by name
            if (airport.Shop.BuyItem(input, airport) == PurchaseResult.Success)
            {
                logger.Log($"✅ Purchased {input}.");
            }
            else
            {
                logger.Log($"❌ Could not purchase {input}. Insufficient funds or invalid item.");
            }
        }

        tickManager.Start();
    }

    private void RepairRunways()
    {
        logger.Log($"🛠️ Manual runway maintenance initiated by user.");
        airport.RunwayManager.PerformMaintenance(airport.Treasury);
    }
}


