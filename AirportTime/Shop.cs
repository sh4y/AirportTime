using System;
using System.Collections.Generic;
using System.Linq;

public class Shop
{
    private readonly List<IPurchasable> itemsForSale;
    private readonly List<QueuedShopItem> queuedItems;
    private readonly Treasury treasury;
    private readonly GameLogger logger;
    private int nextItemId = 1;

    public Shop(Treasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
        itemsForSale = new List<IPurchasable>();
        queuedItems = new List<QueuedShopItem>();

        InitializeItems();
    }

    private void InitializeItems()
    {
        // Initialize with small runways
        AddItemToShop(new SmallRunway(nextItemId++, "Small Runway", 100, "Basic runway suitable for small aircraft"));
        AddItemToShop(new SmallRunway(nextItemId++,"Small Runway2", 1000, "Basic runway suitable for small aircraft"));
        
        // Queue items for future levels
        QueueItemForLevel(new RunwayBuff(nextItemId++, "Runway Speed Upgrade", "Increases runway speed by 10%", 10000, 
            BuffType.LandingDurationReduction, 0.9), 2);
        QueueItemForLevel(new MediumRunway(nextItemId++, "Medium Runway", 10000, "Capable of handling medium aircraft"), 5);
        QueueItemForLevel(new SmallRunway(nextItemId++, "Small Runway3", 250000, "Capable of handling small aircraft"), 6);
        QueueItemForLevel(new MediumRunway(nextItemId++, "Medium Runway2", 500000, "Capable of handling medium aircraft"), 8);
        QueueItemForLevel(new LargeRunway(nextItemId++, "Large Runway", 1000000, "Capable of handling large aircraft"), 9);
    }

    /// <summary>
    /// Initialize achievement handling once we have an airport reference
    /// </summary>
    public void InitializeAchievementHandling(Airport airport)
    {
        if (airport == null) return;
        
        airport.AchievementSystem.OnAchievementUnlocked += (achievement) => 
        {
            if (achievement.Type == AchievementType.FlightTypeSpecialization)
            {
                var buff = FlightSpecializationBuff.FromAchievement(achievement, GetNextItemId());
                AddItemToShop(buff);
                logger.Log($"New specialization buff added to shop: {buff.Name} - {buff.Description} - Price: {buff.Price:C}");
            }
        };
    }

    /// <summary>
    /// Adds a new item to the shop
    /// </summary>
    public void AddItemToShop(IPurchasable item)
    {
        // Check if we already have this item
        if (itemsForSale.Any(i => i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Log($"Item '{item.Name}' already exists in the shop.");
            return;
        }
    
        // Add the item
        itemsForSale.Add(item);
        logger.Log($"New item added to shop: {item.Name} - {item.Description} - Price: {item.Price:C}");
    }

    /// <summary>
    /// Queues an item to be added to the shop when the airport reaches a specific level
    /// </summary>
    public void QueueItemForLevel(IPurchasable item, int requiredLevel)
    {
        // Check if already queued or in shop
        if (queuedItems.Any(qi => qi.Item.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Log($"Item '{item.Name}' is already queued for a level unlock.");
            return;
        }

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
        // Find items that can be unlocked
        var itemsToAdd = queuedItems.Where(qi => qi.RequiredLevel <= currentLevel).ToList();

        foreach (var queuedItem in itemsToAdd)
        {
            // Add the item to the shop
            AddItemToShop(queuedItem.Item);
            
            // Remove from queue
            queuedItems.Remove(queuedItem);
            
            // Log the unlock
            logger.Log($"Unlocked queued item '{queuedItem.Item.Name}' at airport level {currentLevel}.");
        }
    }

    /// <summary>
    /// Utility method to queue a runway with specified tier for a level
    /// </summary>
    public void QueueRunwayForLevel(string name, int price, string description, RunwayTier tier, int requiredLevel)
    {
        IPurchasable runway;
        switch (tier)
        {
            case RunwayTier.Tier1:
                runway = new SmallRunway(nextItemId++, name, price, description);
                break;
            case RunwayTier.Tier2:
                runway = new MediumRunway(nextItemId++, name, price, description);
                break;
            case RunwayTier.Tier3:
                runway = new LargeRunway(nextItemId++, name, price, description);
                break;
            default:
                logger.Log($"Invalid runway tier {tier} for queued item {name}");
                return;
        }
        
        QueueItemForLevel(runway, requiredLevel);
    }

    /// <summary>
    /// Display available items in the shop (hiding sold-out items)
    /// </summary>
    public void ViewItemsForSale()
    {
        logger.Log("Items for Sale:");
        
        var availableItems = new List<IPurchasable>();
        
        // Filter out sold-out items
        foreach (var item in itemsForSale)
        {
            bool isSoldOut = false;
            
            // Check if the item is sold out (for Item types)
            if (item is Item itemWithStock)
            {
                try
                {
                    // Try to use the IsSoldOut method
                    var method = typeof(Item).GetMethod("IsSoldOut");
                    if (method != null)
                    {
                        isSoldOut = (bool)method.Invoke(itemWithStock, null);
                    }
                }
                catch
                {
                    // If method doesn't exist, check availability directly
                    isSoldOut = itemWithStock.Availability <= 0;
                }
            }
            
            if (!isSoldOut)
            {
                availableItems.Add(item);
            }
        }
        
        if (availableItems.Count == 0)
        {
            logger.Log("No items currently available for purchase.");
            return;
        }
        
        foreach (var item in availableItems)
        {
            logger.Log($"ID: {item.Id} - {item.Name} (Tier {item.ItemTier}) - {item.Description} - Price: {item.Price:C}");
        }
    }

    /// <summary>
    /// Buy an item by its ID
    /// </summary>
    public PurchaseResult BuyItem(int itemId, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            logger.Log("Item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        
        // Check if the item is sold out (if it's an Item)
        if (item is Item itemWithStock)
        {
            bool isSoldOut = false;
            try
            {
                // Try to use the IsSoldOut method if it exists
                var method = typeof(Item).GetMethod("IsSoldOut");
                if (method != null)
                {
                    isSoldOut = (bool)method.Invoke(itemWithStock, null);
                }
            }
            catch
            {
                // If method doesn't exist or fails, check availability directly
                isSoldOut = itemWithStock.Availability <= 0;
            }
            
            if (isSoldOut)
            {
                logger.Log($"Item {item.Name} is sold out.");
                return PurchaseResult.ItemNotFound;
            }
        }
        
        // Check if we can afford it
        if (treasury.GetBalance() < item.Price)
        {
            logger.Log("Not enough gold to purchase this item.");
            return PurchaseResult.NotEnoughGold;
        }
        
        // Deduct funds
        treasury.DeductFunds(item.Price, $"Purchased {item.Name}");
        logger.Log($"You have purchased: {item.Name}");
        
        // Reduce availability if it's an Item type
        if (item is Item purchasableItem)
        {
            try
            {
                // Try to use the Purchase method if it exists
                var method = typeof(Item).GetMethod("Purchase");
                if (method != null)
                {
                    method.Invoke(purchasableItem, null);
                }
            }
            catch
            {
                // If method doesn't exist, we can't reduce availability
            }
        }
        
        // Execute item's purchase logic
        item.OnPurchase(airport);
        return PurchaseResult.Success;
    }

    /// <summary>
    /// Buy an item by runway tier
    /// </summary>
    public PurchaseResult BuyItem(RunwayTier tier, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i =>
            i.Type == ItemType.Runway && i.ItemTier == (int)tier);
            
        if (item == null)
        {
            logger.Log("Runway item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        
        // Forward to main BuyItem method
        return BuyItem(item.Id, airport);
    }

    /// <summary>
    /// Buy an item by its name
    /// </summary>
    public PurchaseResult BuyItem(string itemName, Airport airport = null)
    {
        var item = itemsForSale.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            logger.Log("Item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        
        // Forward to main BuyItem method
        return BuyItem(item.Id, airport);
    }
    
    /// <summary>
    /// Gets the next available item ID for creating new shop items
    /// </summary>
    public int GetNextItemId()
    {
        if (itemsForSale.Count == 0)
            return 1;
            
        return itemsForSale.Max(item => item.Id) + 1;
    }
}