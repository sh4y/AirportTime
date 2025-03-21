// File: AirportTime/Item.cs
// This class now implements IPurchasable so that items can define additional behavior on purchase.
public abstract class Item : IPurchasable
{
    public int Id {get; private set;}
    public string Name { get; private set; }
    public string Description { get; private set; }
    public double Price { get; private set; }
    public ItemType Type { get; private set; }
    public int ItemTier { get; private set; }
    public int Availability { get; private set; }

    /// <summary>
    /// Creates a new Item with the specified properties.
    /// </summary>
    /// <param name="name">Name of the item.</param>
    /// <param name="description">Description of the item.</param>
    /// <param name="price">Price of the item.</param>
    /// <param name="type">Type of the item (e.g., Runway, Staff, etc.).</param>
    /// <param name="tier">Tier level of the item.</param>
    /// <param name="availability">Optional initial availability (default is 1).</param>
    public Item(string name, string description, double price, ItemType type, int tier, int availability = 1)
    {
        Name = name;
        Description = description;
        Price = price;
        Type = type;
        ItemTier = tier;
        Availability = availability;
    }

    /// <summary>
    /// Attempts to purchase the item by reducing its availability.
    /// Returns true if the item was available, false if it is sold out.
    /// </summary>
    public bool Purchase()
    {
        if (IsSoldOut())
        {
            return false;
        }
        Availability--;
        return true;
    }

    /// <summary>
    /// Indicates whether the item is sold out.
    /// </summary>
    /// <returns>True if sold out; otherwise false.</returns>
    public bool IsSoldOut() => Availability <= 0;

    /// <summary>
    /// Default OnPurchase behavior: logs the purchase, reduces the availability, and logs if the item is sold out.
    /// </summary>
    /// <param name="airport">The airport context used for logging.</param>
    public virtual void OnPurchase(Airport airport)
    {
        airport.GameLogger.Log($"✅ Purchased {Name} for {Price:C}.");

    }

    /// <summary>
    /// Provides a useful summary of the item, including its availability.
    /// </summary>
    public override string ToString()
    {
        string availabilityStatus = IsSoldOut() ? "Sold Out" : Availability.ToString();
        return $"{Name} (Tier {ItemTier}) - {Description} - Price: {Price:C} - Available: {availabilityStatus}";
    }
    


}
