
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
// Item class to represent purchasable items (e.g., Runways, Staff, etc.)
public class Item
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public double Price { get; private set; }
    public ItemType Type { get; private set; }
    public int Tier { get; private set; }

    public Item(string name, string description, double price, ItemType type, int tier)
    {
        Name = name;
        Description = description;
        Price = price;
        Type = type;
        Tier = tier;
    }
}
