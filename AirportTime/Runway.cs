
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
// Runway class to represent runways available for landing
public class Runway
{
    public string RunwayID { get; private set; }
    public int Length { get; private set; }

    public Runway(string runwayID, int length)
    {
        RunwayID = runwayID;
        Length = length;
    }

    // Check if a plane can land on this runway
    public bool CanLand(Plane plane)
    {
        return plane.RequiredRunwayLength <= Length;
    }
}
