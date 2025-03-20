
// Game simulation class to simulate the purchase of items and running the airport
public class GameSimulation
{
    public static void Msain()
    {
        // Start with 0 gold
        GameLogger g = new GameLogger("GameLogs.db");

        Treasury playerTreasury = new Treasury(g, new TransactionLogStore());
        Shop shop = new Shop(playerTreasury, g);
        RunwayMaintenanceSystem maintenanceSystem = new RunwayMaintenanceSystem();
        RunwayManager runwayManager = new RunwayManager(maintenanceSystem, g);

        Console.WriteLine("Starting with 0 gold.");

        // Simulate game loop (with 60 ticks for example)
        for (int tick = 1; tick <= 60; tick++)
        {
            // Accumulate gold over time
            playerTreasury.AccumulateGold();

            Console.WriteLine($"\nTick {tick}: Gold: {playerTreasury.GetBalance():C}");

            // Display available items in the shop
            shop.ViewItemsForSale();

            // Try to buy a Tier 1 Runway
            if (tick == 10)  // For example, buy after 10 ticks
            {
                shop.BuyItem("Tier 1 Runway");
            }

            // Try to buy a Tier 2 Runway after enough gold is accumulated
            if (tick == 20 && playerTreasury.GetBalance() >= 10000)
            {
                shop.BuyItem("Tier 2 Runway");
            }

            // Display runways info
            runwayManager.DisplayRunwayInfo();

            System.Threading.Thread.Sleep(1000);  // Wait for 1 second before the next tick
        }
    }
}
