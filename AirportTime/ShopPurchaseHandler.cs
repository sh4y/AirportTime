public class ShopPurchaseHandler
{
    private readonly ITreasury treasury;
    private readonly GameLogger logger;

    public ShopPurchaseHandler(ITreasury treasury, GameLogger logger)
    {
        this.treasury = treasury;
        this.logger = logger;
    }

    /// <summary>
    /// Attempts to purchase an item
    /// </summary>
    public PurchaseResult PurchaseItem(IPurchasable item, Airport airport)
    {
        if (item == null)
        {
            logger.Log("Item not found in the shop.");
            return PurchaseResult.ItemNotFound;
        }
        
        // Check if the item is sold out (if it's an Item)
        if (IsItemSoldOut(item))
        {
            logger.Log($"Item {item.Name} is sold out.");
            return PurchaseResult.ItemNotFound;
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