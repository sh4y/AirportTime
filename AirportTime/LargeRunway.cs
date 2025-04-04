﻿namespace AirportTime;

public class LargeRunway : Runway
{
    private const int DefaultLength = 10000;
    public RunwayTier CurrentTier { get; private set; }
    public string Name { get; }
    public double Price { get; }
    public string Description { get; }

    public LargeRunway(int id, string runwayName, int price, string desc, int avail=1) 
        : base(id, runwayName, DefaultLength, (int)RunwayTier.Tier3, price, desc, avail)
    {
        Id = id;

        Name = runwayName;
        Price = price;
        Description = desc;
        CurrentTier = RunwayTier.Tier3;
    }

    public override void OnPurchase(Airport airport)
    {
        airport.RunwayManager.UnlockRunway((Runway)this);
    }
}