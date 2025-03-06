// File: AirportTime/MediumRunway.cs

public class MediumRunway : Runway
{
    private const int DefaultLength = 2000;  // Medium runway length
    private const int DefaultTier = 2;         // Tier 2 for medium runway

    public MediumRunway(string runwayID) : base(runwayID, DefaultLength, DefaultTier) { }
}
