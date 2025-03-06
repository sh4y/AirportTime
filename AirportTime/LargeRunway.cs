// File: AirportTime/LargeRunway.cs

public class LargeRunway : Runway
{
    private const int DefaultLength = 50000; // large runway length
    private const int DefaultTier = 3;         // tier 3 for large runway

    public LargeRunway(string runwayID) : base(runwayID, DefaultLength, DefaultTier) { }
}
