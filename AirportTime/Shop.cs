using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class Shop
{
    private readonly List<IPurchasable> itemsForSale;
    private readonly List<QueuedShopItem> queuedItems; // New list for queued items
    private readonly Treasury treasury;
    private readonly GameLogger logger;
    private int nextItemId = 1;

    public Shop(Treasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
        itemsForSale = new List<IPurchasable>();
        queuedItems = new List<QueuedShopItem>(); // Initialize queued items list

        InitializeItems();
    }

    private void InitializeItems()
    {
        // Use the RunwayTier enum for clarity.
        AddItemToShop(new SmallRunway(nextItemId++, "Small Runway", 100, "Basic runway suitable for small aircraft"));
        AddItemToShop(new SmallRunway(nextItemId++,"Small Runway2", 1000, "Basic runway suitable for small aircraft"));
        
        // Queue items for future levels instead of directly adding thems1
        QueueItemForLevel(new RunwayBuff(nextItemId++,"Runway Speed Upgrade", "Increases runway speed by 10%", 10000, 
            BuffType.LandingDurationReduction, 0.9), 2);
        QueueItemForLevel(new MediumRunway(nextItemId++,"Medium Runway", 10000, "Capable of handling medium aircraft"), 5);
        QueueItemForLevel(new SmallRunway(nextItemId++,"Small Runway3", 250000, "Capable of handling small aircraft"), 6);
        QueueItemForLevel(new MediumRunway(nextItemId++,"Medium Runway2", 500000, "Capable of handling medium aircraft"), 8);
        QueueItemForLevel(new LargeRunway(nextItemId++,"Large Runway", 1000000, "Capable of handling large aircraft"), 9);

    }

    /// <summary>
    /// Adds a new item to the shop
    /// </summary>
    public void AddItemToShop(IPurchasable item)
    {
        // Check if we already have an item with this name
        if (itemsForSale.Any(i => i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Log($"Item '{item.Name}' already exists in the shop.");
            return;
        }
    
        // Add the item to the shop
        itemsForSale.Add(item);
        logger.Log($"New item added to shop: {item.Name} - {item.Description} - Price: {item.Price:C}");
    }

    /// <summary>
    /// Queues an item to be added to the shop when the airport reaches a specific level
    /// </summary>
    public void QueueItemForLevel(Item item, int requiredLevel)
    {
        // Check if already queued
        if (queuedItems.Any(qi => qi.Item.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Log($"Item '{item.Name}' is already queued for a level unlock.");
            return;
        }

        // Check if already in shop
        if (itemsForSale.Any(i => i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Log($"Item '{item.Name}' is already available in the shop.");
            return;
        }

        // Add to queue
        queuedItems.Add(new QueuedShopItem(item, requiredLevel));
        logger.Log($"Item '{item.Name}' queued to unlock at airport level {requiredLevel}.");
    }

    /// <summary>
    /// Checks for queued items that should be unlocked at the current level
    /// </summary>
    public void CheckQueuedItemsForLevel(int currentLevel)
    {
        // Find items that can be unlocked at current level
        var itemsToAdd = queuedItems.Where(qi => qi.RequiredLevel <= currentLevel).ToList();

        foreach (var queuedItem in itemsToAdd)
        {
            // Add the item to the shop
            AddItemToShop(queuedItem.Item);
            
            // Remove from queue
            queuedItems.Remove(queuedItem);
            
            // Log that it's been unlocked
            logger.Log($"Unlocked queued item '{queuedItem.Item.Name}' at airport level {currentLevel}.");
        }
    }

    /// <summary>
    /// Utility method to queue a runway with specified tier for a level
    /// </summary>
    public void QueueRunwayForLevel(string name, int price, string description, RunwayTier tier, int requiredLevel)
    {
        Item runway;
        switch (tier)
        {
            case RunwayTier.Tier1:
                runway = new SmallRunway(nextItemId++,name, price, description);
                break;
            case RunwayTier.Tier2:
                runway = new MediumRunway(nextItemId++,name, price, description);
                break;
            case RunwayTier.Tier3:
                runway = new LargeRunway(nextItemId++,name, price, description);
                break;
            default:
                logger.Log($"Invalid runway tier {tier} for queued item {name}");
                return;
        }
        
        QueueItemForLevel(runway, requiredLevel);
    }

    // Existing methods remain unchanged
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