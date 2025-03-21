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
    public int LandingDuration = 10; // Landing process takes 5 ticks
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

    public override void OnPurchase(Airport airport)
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
    
// Add this method to the Runway class to ensure proper occupation handling

    /// <summary>
    /// Occupies the runway specifically for a landing operation.
    /// </summary>
    public void OccupyForLanding()
    {
        // Mark the runway as occupied for the landing duration
        IsOccupied = true;
        OccupiedCountdown = LandingDuration;
    
        Console.WriteLine($"Runway {Name} is now occupied for landing. Will be free in {OccupiedCountdown} ticks.");
    }

    /// <summary>
    /// Updates the runway's occupied status each tick.
    /// </summary>
    public void UpdateOccupiedStatus()
    {
        if (IsOccupied && OccupiedCountdown > 0)
        {
            OccupiedCountdown--;
        
            if (OccupiedCountdown == 0)
            {
                IsOccupied = false;
                Console.WriteLine($"Runway {Name} is now free.");
            }
        }
    }
    /// <summary>
    /// Reduces the landing duration time by the specified percentage.
    /// The minimum landing duration is 1 tick.
    /// </summary>
    /// <param name="reductionFactor">Percentage to reduce duration by (0.0 to 1.0)</param>
    public void ReduceLandingDuration(double reductionFactor)
    {
        // Ensure the reduction factor is within valid range
        reductionFactor = Math.Clamp(reductionFactor, 0.0, 1.0);
    
        // Calculate the new duration with a minimum of 1 tick
        int originalDuration = LandingDuration;
        LandingDuration = Math.Max(1, (int)(LandingDuration * (reductionFactor)));
    
        Console.WriteLine($"Runway {Name} landing duration reduced from {originalDuration} to {LandingDuration} ticks.");
    }
}