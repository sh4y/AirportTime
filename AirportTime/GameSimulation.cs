﻿
// Game simulation class to simulate the purchase of items and running the airport
public class GameSimulation
{
    public static void Main(string[] args)
    {
        // Start with 0 gold
        Treasury playerTreasury = new Treasury(0);
        Shop shop = new Shop(playerTreasury);
        RunwayMaintenanceSystem maintenanceSystem = new RunwayMaintenanceSystem();
        RunwayManager runwayManager = new RunwayManager(maintenanceSystem);

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
                runwayManager.UnlockRunway("Tier 1 Runway");
            }

            // Try to buy a Tier 2 Runway after enough gold is accumulated
            if (tick == 20 && playerTreasury.GetBalance() >= 10000)
            {
                shop.BuyItem("Tier 2 Runway");
                runwayManager.UnlockRunway("Tier 2 Runway");
            }

            // Display runways info
            runwayManager.DisplayRunwayInfo();

            System.Threading.Thread.Sleep(1000);  // Wait for 1 second before the next tick
        }
    }
}
