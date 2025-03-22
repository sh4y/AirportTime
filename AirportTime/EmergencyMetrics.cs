public class EmergencyMetrics
{
    private readonly Airport airport;

    public EmergencyMetrics(Airport airport)
    {
        this.airport = airport;
    }

    public List<EmergencyInfo> GetActiveEmergencies(int currentTick)
    {
        var emergencyData = airport.EmergencyFlightHandler.GetActiveEmergencies(currentTick);
        return emergencyData.Select(e => new EmergencyInfo
        {
            FlightNumber = e.flightNumber,
            TimeRemaining = e.ticksRemaining,
            FlightType = e.flight.Type,
            Priority = e.flight.Priority
        }).ToList();
    }
}