using AirportTime;

public class ShopLevelManager
{
    private readonly List<QueuedShopItem> queuedItems = new List<QueuedShopItem>();
    private readonly ShopInventory inventory;
    private readonly GameLogger logger;

    public ShopLevelManager(ShopInventory inventory, GameLogger logger)
    {
        this.inventory = inventory;
        this.logger = logger;
    }

    /// <summary>
    /// Queues an item to be added to the shop when the airport reaches a specific level
    /// </summary>
    public void QueueItemForLevel(IPurchasable item, int requiredLevel)
    {
        // Check if already queued
        if (queuedItems.Any(qi => qi.Item.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Log($"Item '{item.Name}' is already queued for a level unlock.");
            return;
        }

        // Check if already in shop
        if (inventory.GetItemByName(item.Name) != null)
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
            inventory.AddItem(queuedItem.Item);
            
            // Remove from queue
            queuedItems.Remove(queuedItem);
            
            // Log the unlock
            logger.Log($"Unlocked queued item '{queuedItem.Item.Name}' at airport level {currentLevel}.");
        }
    }
}