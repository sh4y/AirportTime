public class FlightGenerator
{
    private readonly IRandomGenerator randomGenerator;

    public FlightGenerator(IRandomGenerator randomGenerator)
    {
        this.randomGenerator = randomGenerator;
    }

    public Flight GenerateRandomFlight(int tick, int passengerCount)
    {
        // Generate a flight number between 100 and 999 (using 1000 as the exclusive upper bound)
        string flightNumber = "AA" + randomGenerator.Next(100, 1000);

        // Randomize between small, medium, and large planes (0, 1, or 2)
        PlaneSize planeSize = (PlaneSize)randomGenerator.Next(0,0);

        // Generate a random weight for the plane between 10,000 and 80,000
        Plane plane = new Plane(flightNumber, planeSize, randomGenerator.Next(10000, 80000));

        // Randomly choose a flight type (0 to 3)
        FlightType flightType = (FlightType)randomGenerator.Next(0, 4);

        // Randomly choose a flight priority (0 to 2)
        FlightPriority flightPriority = (FlightPriority)randomGenerator.Next(0, 3);

        // Determine the scheduled landing time: current tick plus a random offset between 5 and 15 ticks
        int scheduledLandingTime = tick + randomGenerator.Next(5, 15);

        // Create and return the flight with random details
        return new Flight(flightNumber, plane, flightType, flightPriority, scheduledLandingTime, passengerCount);
    }

    // Generate multiple flights at the given tick
    public List<Flight> GenerateMultipleFlights(int tick, int count)
    {
        var flights = new List<Flight>();
        for (int i = 0; i < count; i++)
        {
            // Generate a random number of passengers between 50 and 300
            int passengerCount = randomGenerator.Next(50, 300);
            flights.Add(GenerateRandomFlight(tick, passengerCount));
        }
        return flights;
    }
}
