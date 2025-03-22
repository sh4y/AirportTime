public interface IPurchasable
{
    int Id { get; }
    string Name { get; }
    string Description { get; }
    double Price { get; }
    ItemType Type { get; }
    int ItemTier { get; }
    int Availability { get; } // Number of available units. Defaults to 1.
    void OnPurchase(Airport airport);
}