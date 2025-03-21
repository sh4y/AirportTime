using System;

public class SmallRunway : Runway, IPurchasable
{
    private const int DefaultLength = 5000; // large runway length
    private const int DefaultTier = 1;         // tier 3 for large runway
    public int Id { get; set; }
    public string Name { get; }
    public double Price { get; }
    public ItemType Type => ItemType.Runway;
    public int ItemTier => (int)CurrentTier;
    public string Description { get; }
    public RunwayTier CurrentTier { get; private set; }
    
    public SmallRunway(int id, string runwayName, int price, string desc) : 
        base(id, runwayName, DefaultLength, (int)RunwayTier.Tier1, price, desc)
    {
        Id = id;
        Name = runwayName;
        Price = price;  
        Description = desc;
        CurrentTier = RunwayTier.Tier1;
    }

    public override void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway((Runway)this);
    }
}
