public class LargeRunway : Runway
{
    private const int DefaultLength = 10000;
    public RunwayTier CurrentTier { get; private set; }
    public string Name { get; }
    public double Price { get; }
    public string Description { get; }

    public LargeRunway(string runwayName, int price, string desc) 
        : base(runwayName, DefaultLength, (int)RunwayTier.Tier3, price, desc)
    {
        Name = runwayName;
        Price = price;
        Description = desc;
        CurrentTier = RunwayTier.Tier3;
    }

    public void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway((Runway)this);
    }
}