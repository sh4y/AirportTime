// File: AirportTime/Runway.cs

public class Runway
{
    public string RunwayID { get; set; }
    public int Length { get; private set; }
    public int Tier { get; private set; }

    public Runway(string runwayID, int length, int tier)
    {
        RunwayID = runwayID;
        Length = length;
        Tier = tier;

    }

    // Check if a plane can land on this runway.
    public virtual bool CanLand(Plane plane)
    {
        // Example logic: the runway length must be at least the plane's required runway length.
        return Length >= plane.RequiredRunwayLength;
    }
}