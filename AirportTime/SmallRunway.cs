public class SmallRunway : Runway
{
    private const int DefaultLength = 5000; // large runway length
    private const int DefaultTier = 1;         // tier 3 for large runway

    public SmallRunway(string runwayID) : base(runwayID, DefaultLength, DefaultTier) { }
}
