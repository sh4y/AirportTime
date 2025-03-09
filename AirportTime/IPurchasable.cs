
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public interface IPurchasable
{
    public string Name { get; }
    public string Description { get; }
    public double Price { get; }
    public ItemType Type { get; }
    public int ItemTier { get; }
    public int Availability { get; } // Number of available units. Defaults to 1.
    void OnPurchase(Airport airport);
}
