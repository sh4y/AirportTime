using AirportTime;

public class ShopInventory
{
    private readonly List<IPurchasable> itemsForSale = new List<IPurchasable>();
    private readonly GameLogger logger;
    private int nextItemId = 1;

    public ShopInventory(GameLogger logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Adds a new item to the shop inventory
    /// </summary>
    public void AddItem(IPurchasable item)
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
    /// Gets all available items in the shop
    /// </summary>
    public List<IPurchasable> GetAvailableItems()
    {
        return itemsForSale.Where(item => !IsItemSoldOut(item)).ToList();
    }

    /// <summary>
    /// Gets all items in the shop
    /// </summary>
    public List<IPurchasable> GetAllItems()
    {
        return new List<IPurchasable>(itemsForSale);
    }

    /// <summary>
    /// Finds an item by its ID
    /// </summary>
    public IPurchasable GetItemById(int itemId)
    {
        return itemsForSale.FirstOrDefault(i => i.Id == itemId);
    }

    /// <summary>
    /// Finds an item by its name
    /// </summary>
    public IPurchasable GetItemByName(string itemName)
    {
        return itemsForSale.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the next available item ID
    /// </summary>
    public int GetNextItemId()
    {
        return nextItemId++;
    }

    /// <summary>
    /// Checks if an item is sold out
    /// </summary>
    private bool IsItemSoldOut(IPurchasable item)
    {
        // Check if the item is sold out (for Item types)
        if (item is Item itemWithStock)
        {
            try
            {
                // Try to use the IsSoldOut method if it exists
                var method = typeof(Item).GetMethod("IsSoldOut");
                if (method != null)
                {
                    return (bool)method.Invoke(itemWithStock, null);
                }
            }
            catch
            {
                // If method doesn't exist or fails, check availability directly
                return itemWithStock.Availability <= 0;
            }
        }
        
        return false;
    }
}