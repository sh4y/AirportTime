
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class Plane
{
    public string PlaneID { get; private set; }
    public PlaneSize Size { get; private set; }
    public double Weight { get; private set; }
    public int RequiredRunwayLength { get; private set; }

    public Plane(string planeID, PlaneSize size, double weight)
    {
        PlaneID = planeID;
        Size = size;
        Weight = weight;

        // Set runway requirements based on plane size
        switch (size)
        {
            case PlaneSize.Small:
                RequiredRunwayLength = 1000;
                break;
            case PlaneSize.Medium:
                RequiredRunwayLength = 1500;
                break;
            case PlaneSize.Large:
                RequiredRunwayLength = 2000;
                break;
            default:
                RequiredRunwayLength = 1500; // Default to medium length
                break;
        }
    }
}
