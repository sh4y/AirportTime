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

    public int WearLevel { get; private set; } = 0;
    
    public bool IsOccupied { get; private set; } = false;
    public int OccupiedCountdown { get; private set; } = 0;
    public int LandingDuration = 5; // Landing process takes 5 ticks
    public int RepairDuaration = 10; // Repair process takes 10 ticks

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
        airport.RunwayManager.UnlockRunway((Runway)this);
        airport.GameLogger.Log($"✅ Purchased and unlocked {Name} (Tier {ItemTier}).");
    }

    public bool CanLand(Plane plane)
    {
        if (plane == null)
            throw new ArgumentNullException(nameof(plane));

        return plane.RequiredRunwayLength <= Length;
    }

    /// <summary>
    /// Increment the runway's WearLevel by the amount specified.
    /// The calculation of <c>totalWear</c> is done by the maintenance system,
    /// but the actual state update is here.
    /// </summary>
    public void ApplyWear(int totalWear)
    {
        WearLevel = Math.Min(100, WearLevel+totalWear);
    }

    /// <summary>
    /// Resets wear to zero and occupies the runway for 10 ticks.
    /// </summary>
    public void Repair()
    {
        WearLevel = 0;
        Occupy(RepairDuaration); // Occupy the runway for landing after repair
    }

    private void Occupy(int duration)
    {
        IsOccupied = true;
        OccupiedCountdown = duration;
    }
    
    // New method to occupy the runway for landing
    public void OccupyForLanding()
    {
        Occupy(LandingDuration);
    }
    
    // New method to update runway status each tick
    public void UpdateOccupiedStatus()
    {
        if (IsOccupied && OccupiedCountdown > 0)
        {
            OccupiedCountdown--;
            if (OccupiedCountdown == 0)
            {
                IsOccupied = false;
            }
        }
    }
}