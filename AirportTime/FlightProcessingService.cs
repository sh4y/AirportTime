public class FlightProcessingService
{
    private readonly FlightScheduler _flightScheduler;
    private readonly FlightLandingManager _landingManager;
    private readonly List<Flight> _activeFlights = new List<Flight>();

    public FlightProcessingService(
        FlightScheduler flightScheduler,
        FlightLandingManager landingManager)
    {
        _flightScheduler = flightScheduler;
        _landingManager = landingManager;
    }

    public void ProcessScheduledFlights(int currentTick)
    {
        // Process flights scheduled for this tick
        var scheduledFlights = _flightScheduler.GetFlightsAtTick(currentTick);
        
        // Also process any delayed flights that haven't landed yet
        var delayedFlights = _flightScheduler.GetUnlandedFlights()
            .Where(f => f.ScheduledLandingTime < currentTick);

        // Process all flights
        foreach (var flight in scheduledFlights.Concat(delayedFlights))
        {
            _landingManager.ProcessFlight(flight, currentTick);
        }
    }
    
    public void UpdateActiveFlights()
    {
        // Update our list of active flights (for XP calculations)
        _activeFlights.Clear();
        _activeFlights.AddRange(_flightScheduler.GetUnlandedFlights());
    }
    
    public int GetSimultaneousFlightCount()
    {
        return Math.Max(0, _activeFlights.Count - 1);
    }
    
    public List<Flight> GetActiveFlights()
    {
        return new List<Flight>(_activeFlights);
    }
}