
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
// FlightManager.cs (Modified to Handle Revenue)
public class FlightManager
{
    private List<Flight> activeFlights;
    private Queue<Flight> landingQueue;
    private FlightProcessingSystem flightProcessingSystem;

    public FlightManager(FlightProcessingSystem flightProcessingSystem)
    {
        this.flightProcessingSystem = flightProcessingSystem;
        activeFlights = new List<Flight>();
        landingQueue = new Queue<Flight>();
    }

    // Schedule a flight for landing
    public void ScheduleFlight(Flight flight)
    {
        activeFlights.Add(flight);
        landingQueue.Enqueue(flight);
    }

    // Process flight landings and log revenue
    public void ProcessLandings()
    {
        flightProcessingSystem.ProcessLandingQueue();
    }

    // Get current landing queue
    public Queue<Flight> GetLandingQueue()
    {
        return landingQueue;
    }
}
