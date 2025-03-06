using System;
using System.Collections.Generic;
using System.Linq;

// This allows the game logic to react appropriately to each outcome.
public class Shop
{
    private List<Item> itemsForSale;
    private Treasury treasury;
    private GameLogger logger;  // centralized logger

    // New constructor accepting a GameLogger.
    public Shop(Treasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
        itemsForSale = new List<Item>();

        // Initialize predefined items.
        InitializeItems();
    }

    // Initialize items available in the shop.
    private void InitializeItems()
    {
        itemsForSale.Add(new Item("Tier 1 Runway", "Basic runway for small planes.", 5000, ItemType.Runway, (int)RunwayTier.Tier1));
        itemsForSale.Add(new Item("Tier 2 Runway", "Medium runway suitable for medium planes.", 10000, ItemType.Runway, (int)RunwayTier.Tier2));
        itemsForSale.Add(new Item("Tier 3 Runway", "Large runway for jumbo jets.", 20000, ItemType.Runway, (int)RunwayTier.Tier3));
    }

    // View all available items for sale using the centralized logger.
    public void ViewItemsForSale()
    {
        if (logger != null)
            logger.Log("Items for Sale:");
        else
            Console.WriteLine("Items for Sale:");

        foreach (var item in itemsForSale)
        {
            string message = $"{item.Name} (Tier {item.Tier}) - {item.Description} - Price: {item.Price:C}";
            if (logger != null)
                logger.Log(message);
            else
                Console.WriteLine(message);
        }
    }

    // Buy an item based on its name.
    // Optionally pass an Airport context so that the item's OnPurchase method can be called.
    public PurchaseResult BuyItem(string itemName, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            if (logger != null)
                logger.Log("Item not found in the shop.");
            else
                Console.WriteLine("Item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }

        if (treasury.GetBalance() < item.Price)
        {
            if (logger != null)
                logger.Log("Not enough gold to purchase this item.");
            else
                Console.WriteLine("Not enough gold to purchase this item.");
            return PurchaseResult.NotEnoughGold;
        }

        treasury.DeductFunds(item.Price, $"Purchased {item.Name}");
        if (logger != null)
            logger.Log($"You have purchased: {item.Name}");
        else
            Console.WriteLine($"You have purchased: {item.Name}");

        // Invoke the item's OnPurchase behavior if an Airport context is provided.
        item.OnPurchase(airport);

        return PurchaseResult.Success;
    }

    // New overload: Buy an item based on a RunwayTier enum value.
    public PurchaseResult BuyItem(RunwayTier tier, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i =>
            i.Type == ItemType.Runway && i.Tier == (int)tier);
        if (item == null)
        {
            if (logger != null)
                logger.Log("Runway item not found in the shop.");
            else
                Console.WriteLine("Runway item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }

        if (treasury.GetBalance() < item.Price)
        {
            if (logger != null)
                logger.Log("Not enough gold to purchase this runway item.");
            else
                Console.WriteLine("Not enough gold to purchase this runway item.");
            return PurchaseResult.NotEnoughGold;
        }

        treasury.DeductFunds(item.Price, $"Purchased {item.Name}");
        if (logger != null)
            logger.Log($"You have purchased: {item.Name}");
        else
            Console.WriteLine($"You have purchased: {item.Name}");

        // Invoke the item's OnPurchase behavior if an Airport context is provided.
        item.OnPurchase(airport);

        return PurchaseResult.Success;
    }
}
