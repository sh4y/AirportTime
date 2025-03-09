public class Runway : Item
{
    public string Name { get; }
    public int Length { get; }
    public ItemType Type { get; }
    public int ItemTier { get; }
    public int Availability { get; }
    public double Price { get; }
    public string Description { get; }
    public RunwayTier Tier => (RunwayTier)ItemTier;

    // NOW we keep the runway’s wear state in the runway itself
    public int WearLevel { get; private set; } = 0;

    public Runway(string name, int length, int tier, double price, string description)
        : base(name, description, price, ItemType.Runway, tier, 1)
    {
        Name = name;
        Length = length;
        ItemTier = tier;
        Price = price;
        Description = description;
    }

    public virtual void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway(Tier);
        airport.GameLogger.Log($"✅ Purchased and unlocked {Name} (Tier {ItemTier}).");
    }

    public bool CanLand(Plane plane)
    {
        if (plane == null)
            throw new ArgumentNullException(nameof(plane));

        return plane.RequiredRunwayLength <= Length;
    }

    /// <summary>
    /// Increment the runway’s WearLevel by the amount specified.
    /// The calculation of <c>totalWear</c> is done by the maintenance system,
    /// but the actual state update is here.
    /// </summary>
    public void ApplyWear(int totalWear)
    {
        WearLevel = Math.Min(100, WearLevel+totalWear);
    }

    /// <summary>
    /// Resets wear to zero.
    /// </summary>
    public void Repair()
    {
        WearLevel = 0;
    }
}