
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class Modifier
{
    public string Name { get; set; }
    public double Value { get; set; }

    public Modifier(string name, double value)
    {
        Name = name;
        Value = value;
    }
}
