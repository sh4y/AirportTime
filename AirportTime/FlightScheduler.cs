
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class FlightScheduler
{
    private Dictionary<int, List<Flight>> scheduledFlights = new Dictionary<int, List<Flight>>();

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
        if (scheduledFlights.TryGetValue(tick, out var flights))
        {
            return flights;
        }

        return new List<Flight>();
    }
}
