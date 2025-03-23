
// The refactored Shop class that uses the components

using AirportTime;

public class Shop : IShop
{
    private readonly ITreasury treasury;
    private readonly GameLogger logger;
    
    // The components
    private readonly ShopInventory inventory;
    private readonly ShopPurchaseHandler purchaseHandler;
    private readonly ShopLevelManager levelManager;
    private readonly AchievementShopHandler achievementHandler;

    public Shop(ITreasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
        
        // Initialize components
        inventory = new ShopInventory(logger);
        purchaseHandler = new ShopPurchaseHandler(treasury, logger);
        levelManager = new ShopLevelManager(inventory, logger);
        achievementHandler = new AchievementShopHandler(inventory, logger);

        InitializeItems();
    }

    private void InitializeItems()
    {
        // Initialize with small runways
        inventory.AddItem(new SmallRunway(inventory.GetNextItemId(), "Small Runway", 100, "Basic runway suitable for small aircraft"));
        inventory.AddItem(new SmallRunway(inventory.GetNextItemId(),"Small Runway2", 1000, "Basic runway suitable for small aircraft"));
        
        // Queue items for future levels
        levelManager.QueueItemForLevel(new RunwayBuff(inventory.GetNextItemId(), "Runway Speed Upgrade", "Increases runway speed by 10%", 10000, 
            BuffType.LandingDurationReduction, 0.9, availability:10), 2);
        levelManager.QueueItemForLevel(new GoldIncomeBuff(inventory.GetNextItemId(), "Gold Income Buff", "Increases gold income by 10%", 1000,
            1.1, 1, availability:1), 2);        
        levelManager.QueueItemForLevel(new GoldIncomeBuff(inventory.GetNextItemId(), "Gold Income Buff", "Increases gold income by 20%", 1000,
            1.2, 1, availability:1), 3);
        levelManager.QueueItemForLevel(new RunwayBuff(inventory.GetNextItemId(), "Runway Wear Reduction", "Reduces runway wear by 100%", 5000,
            BuffType.WearReduction, 100, availability:3), 3);
        levelManager.QueueItemForLevel(new MediumRunway(inventory.GetNextItemId(), "Medium Runway", 10000, "Capable of handling medium aircraft"), 5);
        levelManager.QueueItemForLevel(new GoldIncomeBuff(inventory.GetNextItemId(), "Gold Income Buff", "Increases gold income by 10%", 10000,
            1.1, 1, availability:5), 5);
        levelManager.QueueItemForLevel(new GoldIncomeBuff(inventory.GetNextItemId(), "Gold Income Buff", "Increases gold income by 20%", 1000,
            1.2, 1, availability:1), 5);
        levelManager.QueueItemForLevel(new SmallRunway(inventory.GetNextItemId(), "Small Runway3", 250000, "Capable of handling small aircraft"), 6);
        levelManager.QueueItemForLevel(new MediumRunway(inventory.GetNextItemId(), "Medium Runway2", 500000, "Capable of handling medium aircraft"), 8);
        levelManager.QueueItemForLevel(new GoldIncomeBuff(inventory.GetNextItemId(), "Gold Income Buff", "Increases gold income by 20%", 1000,
            1.5, 1, availability:1), 6);
        levelManager.QueueItemForLevel(new LargeRunway(inventory.GetNextItemId(), "Large Runway", 1000000, "Capable of handling large aircraft"), 9);
        levelManager.QueueItemForLevel(new LargeRunway(inventory.GetNextItemId(), "Large Runway2", 3000000, "Capable of handling large aircraft"), 9);
    }

    // Public methods that delegate to the appropriate components

    public void AddItemToShop(IPurchasable item)
    {
        inventory.AddItem(item);
    }

    public void QueueItemForLevel(IPurchasable item, int requiredLevel)
    {
        levelManager.QueueItemForLevel(item, requiredLevel);
    }

    public void CheckQueuedItemsForLevel(int currentLevel)
    {
        levelManager.CheckQueuedItemsForLevel(currentLevel);
    }

    public void InitializeAchievementHandling(Airport airport)
    {
        achievementHandler.InitializeAchievementHandling(airport);
    }

    public int GetNextItemId()
    {
        return inventory.GetNextItemId();
    }

    public void ViewItemsForSale()
    {
        var availableItems = inventory.GetAvailableItems();
        
        if (availableItems.Count == 0)
        {
            logger.Log("No items currently available for purchase.");
            return;
        }
        
        logger.Log("Items for Sale:");
        foreach (var item in availableItems)
        {
            logger.Log($"ID: {item.Id} - {item.Name} (Tier {item.ItemTier}) - {item.Description} - Price: {item.Price:C}");
        }
    }

    public PurchaseResult BuyItem(int itemId, Airport airport = null)
    {
        var item = inventory.GetItemById(itemId);
        return purchaseHandler.PurchaseItem(item, airport);
    }

    public PurchaseResult BuyItem(string itemName, Airport airport = null)
    {
        var item = inventory.GetItemByName(itemName);
        return purchaseHandler.PurchaseItem(item, airport);
    }

    public PurchaseResult BuyItem(RunwayTier tier, Airport airport = null)
    {
        var item = inventory.GetAllItems().FirstOrDefault(i =>
            i.Type == ItemType.Runway && i.ItemTier == (int)tier);
            
        if (item == null)
        {
            logger.Log("Runway item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        
        return purchaseHandler.PurchaseItem(item, airport);
    }
}