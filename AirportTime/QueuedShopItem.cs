using AirportTime;

/// <summary>
/// Represents a shop item that will be unlocked when the airport reaches a specific level
/// </summary>
public class QueuedShopItem
{
    /// <summary>
    /// The item to be added to the shop
    /// </summary>
    public IPurchasable Item { get; }
    
    /// <summary>
    /// The airport level required to unlock this item
    /// </summary>
    public int RequiredLevel { get; }

    public QueuedShopItem(IPurchasable item, int requiredLevel)
    {
        Item = item;
        RequiredLevel = requiredLevel;
    }
}