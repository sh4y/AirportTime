using AirportTime;

public class InputHandler
{
    private readonly Airport airport;
    private readonly FlightGenerator flightGenerator;
    private readonly TickManager tickManager;
    private readonly IGameLogger logger;

    public InputHandler(Airport airport, FlightGenerator flightGenerator, TickManager tickManager, IGameLogger logger)
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
                
            case ConsoleKey.M:
                // Toggle between automatic and manual landing modes
                airport.ToggleLandingMode();
                FlightLandingManager.LandingMode currentMode = airport.LandingManager.CurrentLandingMode;
                logger.Log($"🔄 Landing mode switched to {currentMode} mode.");
                
                // Show mode status on screen
                Console.WriteLine($"\n>>> Landing mode is now: {currentMode} <<<");
                break;

            case ConsoleKey.H:
                DisplayHelp();
                break;
                
            case ConsoleKey.T:
                // Open the metrics window
                OpenMetricsWindow(currentTick);
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
    
    private void DisplayHelp()
    {
        tickManager.Pause();
        
        Console.Clear();
        Console.WriteLine("=== AIRPORT TIME HELP ===");
        Console.WriteLine("Q - Pause/Unpause Game");
        Console.WriteLine("F - Generate Random Flight");
        Console.WriteLine("S - Open Shop");
        Console.WriteLine("R - Repair All Runways");
        Console.WriteLine("M - Toggle Landing Mode (Automatic/Manual)");
        Console.WriteLine("T - Open Metrics Dashboard");
        Console.WriteLine("H - Show This Help Screen");
        Console.WriteLine("\nManual Landing Mode:");
        Console.WriteLine("  When a flight is ready to land, you'll be prompted to choose a runway.");
        Console.WriteLine("  You have 15 seconds to make a selection or the system will auto-select.");
        Console.WriteLine("\nPress any key to return to the game...");
        
        Console.ReadKey(true);
        tickManager.Start();
    }
    
    private void OpenMetricsWindow(int currentTick)
    {
        tickManager.Pause();
        logger.Log("Opening metrics dashboard (T pressed).");
        
        // Display metrics window
        MetricsWindow.DisplayMetrics(airport, currentTick);
        
        // Resume the game when metrics window is closed
        tickManager.Start();
        logger.Log("Closed metrics dashboard, game resumed.");
    }
}