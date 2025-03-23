using AirportTime;

public enum OccupationReason
{
    Landing,
    Repair,
    Maintenance,
    Emergency
}

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
    public int LandingDuration = 10; // Landing process takes 10 ticks
    public int RepairDuaration = 10; // Repair process takes 10 ticks
    
    // New properties to track occupation details
    public OccupationReason? CurrentOccupationReason { get; private set; } = null;
    public string OccupyingEntity { get; private set; } = string.Empty;

    public Runway(int id, string name, int length, int tier, double price, string description, int avail)
        : base(id, name, description, price, ItemType.Runway, tier, avail)
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

        return plane.RequiredRunwayLength <= Length && !IsOccupied;
    }

    public void ApplyWear(int totalWear)
    {
        WearLevel = Math.Min(100, WearLevel+totalWear);
    }

    public void Repair()
    {
        WearLevel = 0;
        // Keep original method for backward compatibility
        Occupy(RepairDuaration);
        // Set additional details
        CurrentOccupationReason = OccupationReason.Repair;
        OccupyingEntity = "Maintenance Crew";
    }

    public void RepairNoOccupy()
    {
        WearLevel = 0;
        Console.WriteLine($"Runway {Name} has been repaired and is now available.");
    }
    

    // Keep original method for backward compatibility
    private void Occupy(int duration)
    {
        IsOccupied = true;
        OccupiedCountdown = duration;
    }

    // New method with additional parameters
    private void OccupyWithDetails(int duration, OccupationReason reason, string entity)
    {
        IsOccupied = true;
        OccupiedCountdown = duration;
        CurrentOccupationReason = reason;
        OccupyingEntity = entity;
    }
    
    // Keep original method for backward compatibility
    public void OccupyForLanding()
    {
        Occupy(LandingDuration);
        CurrentOccupationReason = OccupationReason.Landing;
        OccupyingEntity = "Unknown Flight";
        
        Console.WriteLine($"Runway {Name} is now occupied for landing. Will be free in {OccupiedCountdown} ticks.");
    }
    
    // New method with flight number
    public void OccupyForLanding(string flightNumber)
    {
        if (string.IsNullOrEmpty(flightNumber))
        {
            OccupyForLanding();
            return;
        }
        
        Occupy(LandingDuration);
        CurrentOccupationReason = OccupationReason.Landing;
        OccupyingEntity = flightNumber;
    
        Console.WriteLine($"Runway {Name} is now occupied for landing of {flightNumber}. Will be free in {OccupiedCountdown} ticks.");
    }

    public void UpdateOccupiedStatus()
    {
        if (IsOccupied && OccupiedCountdown > 0)
        {
            OccupiedCountdown--;
        
            if (OccupiedCountdown == 0)
            {
                string occupationInfo = "";
                if (CurrentOccupationReason.HasValue)
                {
                    occupationInfo = $" (previously {CurrentOccupationReason} by {OccupyingEntity})";
                }
                
                Console.WriteLine($"Runway {Name} is now free{occupationInfo}.");
                
                // Clear occupation data
                IsOccupied = false;
                CurrentOccupationReason = null;
                OccupyingEntity = string.Empty;
            }
        }
    }
    
    public void ReduceLandingDuration(double reductionFactor)
    {
        reductionFactor = Math.Clamp(reductionFactor, 0.0, 1.0);
        int originalDuration = LandingDuration;
        LandingDuration = Math.Max(1, (int)(LandingDuration * (reductionFactor)));
    
        Console.WriteLine($"Runway {Name} landing duration reduced from {originalDuration} to {LandingDuration} ticks.");
    }
    
    public string GetDetailedOccupationStatus()
    {
        if (!IsOccupied)
            return "AVAILABLE";
            
        string reasonText = CurrentOccupationReason?.ToString() ?? "Unknown";
        return $"OCCUPIED: {reasonText} by {OccupyingEntity} ({OccupiedCountdown} ticks remaining)";
    }
}