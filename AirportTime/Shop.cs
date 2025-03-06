using System;
using System.Collections.Generic;
using System.Linq;

// This allows the game logic to react appropriately to each outcome.
public class Shop
{
    private readonly List<Item> itemsForSale;
    private readonly Treasury treasury;
    private readonly GameLogger logger;

    public Shop(Treasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
        itemsForSale = new List<Item>();

        InitializeItems();
    }

    private void InitializeItems()
    {
        // Use the RunwayTier enum for clarity.
        itemsForSale.Add(new Item("Tier 1 Runway", "Basic runway for small planes.", 5000, ItemType.Runway, (int)RunwayTier.Tier1));
        itemsForSale.Add(new Item("Tier 2 Runway", "Medium runway suitable for medium planes.", 10000, ItemType.Runway, (int)RunwayTier.Tier2));
        itemsForSale.Add(new Item("Tier 3 Runway", "Large runway for jumbo jets.", 20000, ItemType.Runway, (int)RunwayTier.Tier3));
    }

    public void ViewItemsForSale()
    {
        logger.Log("Items for Sale:");
        foreach (var item in itemsForSale)
        {
            logger.Log($"{item.Name} (Tier {item.Tier}) - {item.Description} - Price: {item.Price:C}");
        }
    }

    public PurchaseResult BuyItem(string itemName, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            logger.Log("Item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        if (treasury.GetBalance() < item.Price)
        {
            logger.Log("Not enough gold to purchase this item.");
            return PurchaseResult.NotEnoughGold;
        }
        treasury.DeductFunds(item.Price, $"Purchased {item.Name}");
        logger.Log($"You have purchased: {item.Name}");
        item.OnPurchase(airport);
        return PurchaseResult.Success;
    }

    public PurchaseResult BuyItem(RunwayTier tier, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i =>
            i.Type == ItemType.Runway && i.Tier == (int)tier);
        if (item == null)
        {
            logger.Log("Runway item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        if (treasury.GetBalance() < item.Price)
        {
            logger.Log("Not enough gold to purchase this runway item.");
            return PurchaseResult.NotEnoughGold;
        }
        treasury.DeductFunds(item.Price, $"Purchased {item.Name}");
        logger.Log($"You have purchased: {item.Name}");
        item.OnPurchase(airport);
        return PurchaseResult.Success;
    }
}
