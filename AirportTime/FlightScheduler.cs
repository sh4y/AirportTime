// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class FlightScheduler : IFlightScheduler
{
    private readonly Dictionary<int, List<Flight>> scheduledFlights = new();

    public void ScheduleFlight(Flight flight, int scheduledTick)
    {
        if (!scheduledFlights.ContainsKey(scheduledTick))
        {
            scheduledFlights[scheduledTick] = new List<Flight>();
        }
        scheduledFlights[scheduledTick].Add(flight);
    }

    public List<Flight> GetFlightsAtTick(int tick)
    {
        return scheduledFlights.TryGetValue(tick, out var flights)
            ? flights
            : new List<Flight>();
    }

    public List<Flight> GetUnlandedFlights()
    {
        return scheduledFlights.Values
            .SelectMany(flightList => flightList) // Flatten all lists into a single sequence
            .Where(flight => flight.Status != FlightStatus.Landed && flight.Status != FlightStatus.Canceled)   // Filter out landed and canceled flights
            .ToList();
    }

}
