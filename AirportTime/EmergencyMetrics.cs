public class EmergencyMetrics
{
    private readonly Airport airport;

    public EmergencyMetrics(Airport airport)
    {
        this.airport = airport;
    }
    
    public int ActiveEmergencyCount => airport.EmergencyFlightHandler.GetActiveEmergencyCount();
    
    public int TotalEmergenciesHandled 
    {
        get
        {
            // In a real implementation, we'd track this
            return 25; // Placeholder
        }
    }
    
    public double EmergencySuccessRate 
    {
        get
        {
            // In a real implementation, we'd calculate this from actual data
            return 0.85; // 85% success rate placeholder
        }
    }
    
    public Dictionary<FlightType, int> GetEmergencyDistribution()
    {
        // In a real implementation, we'd track by type
        return new Dictionary<FlightType, int>
        {
            { FlightType.Commercial, 15 },
            { FlightType.Cargo, 5 },
            { FlightType.VIP, 3 },
            { FlightType.Emergency, 2 }
        };
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