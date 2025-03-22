public class MediumRunway : Runway
{
    private const int DefaultLength = 7500;
    public RunwayTier CurrentTier { get; private set; }
    public string Name { get; }
    public double Price { get; }
    public string Description { get; }

    public MediumRunway(int id, string runwayName, int price, string desc) 
        : base(id, runwayName, DefaultLength, (int)RunwayTier.Tier2, price, desc)
    {
        Id = id;

        Name = runwayName;
        Price = price;
        Description = desc;
        CurrentTier = RunwayTier.Tier2;
    }

    public override void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway(this);
    }
}