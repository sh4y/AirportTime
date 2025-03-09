public class MediumRunway : Runway
{
    private const int DefaultLength = 7500;
    public RunwayTier CurrentTier { get; private set; }
    public string Name { get; }
    public double Price { get; }
    public string Description { get; }

    public MediumRunway(string runwayName, int price, string desc) 
        : base(runwayName, DefaultLength, (int)RunwayTier.Tier2, price, desc)
    {
        Name = runwayName;
        Price = price;
        Description = desc;
        CurrentTier = RunwayTier.Tier2;
    }

    public void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway(RunwayTier.Tier2);
    }
}