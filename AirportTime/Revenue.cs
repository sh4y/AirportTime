
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class Revenue
{
    private double totalRevenue;

    public Revenue()
    {
        totalRevenue = 0;
    }

    // Calculate revenue based on flight type and passenger count
    public void CalculateRevenue(Flight flight)
    {
        double revenue = 0;
        double baseFare = 100;

        // Revenue based on flight type and passengers
        switch (flight.Type)
        {
            case FlightType.Commercial:
                revenue = baseFare * flight.Passengers;
                break;
            case FlightType.Cargo:
                revenue = baseFare * flight.Passengers * 0.75;  // Cargo flights have reduced revenue
                break;
            case FlightType.VIP:
                revenue = baseFare * flight.Passengers * 2;  // VIP flights pay double
                break;
            case FlightType.Emergency:
                revenue = baseFare * flight.Passengers * 1.5;  // Emergency flights pay 50% more
                break;
        }

        totalRevenue += revenue;
        Console.WriteLine($"Flight {flight.FlightNumber} generated {revenue:C} in revenue.");
    }

    public double GetTotalRevenue() => totalRevenue;
}
