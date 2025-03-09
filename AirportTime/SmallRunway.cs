public class SmallRunway : Runway
{
    private const int DefaultLength = 5000; // large runway length
    private const int DefaultTier = 1;         // tier 3 for large runway
    public string Name { get; }
    public double Price { get; }
    public string Description { get; }
    public RunwayTier CurrentTier { get; private set; }
    
    public SmallRunway(string runwayName, int price, string desc) : 
        base(runwayName, DefaultLength, (int)RunwayTier.Tier1, price, desc)
    {
        Name = runwayName;
        Price = price;  
        Description = desc;
        CurrentTier = RunwayTier.Tier1;
    }

    public void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway(RunwayTier.Tier1);
    }
}
