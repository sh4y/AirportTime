using System;
using System.Collections.Generic;
using System.Linq;

// This allows the game logic to react appropriately to each outcome.
public class Shop
{
    private readonly List<IPurchasable> itemsForSale;
    private readonly Treasury treasury;
    private readonly GameLogger logger;
    private int nextItemId = 1; // Auto-incrementing ID

    public Shop(Treasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
        itemsForSale = new List<IPurchasable>();

        InitializeItems();
    }

    private void InitializeItems()
    {
        // Use the RunwayTier enum for clarity.
        itemsForSale.Add(new SmallRunway("Small Runway", 100, "Basic runway suitable for small aircraft") { Id = nextItemId++ });
        itemsForSale.Add(new SmallRunway("Small Runway2", 100, "Basic runway suitable for small aircraft") { Id = nextItemId++ });
    }

    public void ViewItemsForSale()
    {
        logger.Log("Items for Sale:");
        foreach (var item in itemsForSale)
        {
            logger.Log($"ID: {item.Id} - {item.Name} (Tier {item.ItemTier}) - {item.Description} - Price: {item.Price:C}");
        }
    }

    public PurchaseResult BuyItem(int itemId, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i => i.Id == itemId);
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
            i.Type == ItemType.Runway && i.ItemTier == (int)tier);
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
}
