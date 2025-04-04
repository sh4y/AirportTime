using AirportTime;

public class FlightMetrics
{
    private readonly Airport airport;
    private readonly Dictionary<FlightType, int> _flightTypeCounts = new Dictionary<FlightType, int>();
    private readonly Dictionary<string, int> _airlineOperations = new Dictionary<string, int>();
    private int _totalFlightsProcessed = 0;
    private int _specialFlightsProcessed = 0; // Track special flights

    public FlightMetrics(Airport airport)
    {
        this.airport = airport;
        InitializeCounters();
    }
    
    private void InitializeCounters()
    {
        foreach (FlightType type in Enum.GetValues(typeof(FlightType)))
        {
            _flightTypeCounts[type] = 0;
        }
    }
    
    public void RecordFlight(Flight flight)
    {
        _totalFlightsProcessed++;
        _flightTypeCounts[flight.Type]++;
        
        // Track special flights
        if (flight.IsSpecial)
        {
            _specialFlightsProcessed++;
        }
        
        // Extract airline code from flight number (e.g., "AA123" → "AA")
        string airlineCode = flight.FlightNumber.Length >= 2 ? flight.FlightNumber.Substring(0, 2) : flight.FlightNumber;
        if (!_airlineOperations.ContainsKey(airlineCode))
            _airlineOperations[airlineCode] = 0;
            
        _airlineOperations[airlineCode]++;
    }
    
    // Enhanced methods
    public Dictionary<FlightType, int> GetFlightTypeDistribution() => new Dictionary<FlightType, int>(_flightTypeCounts);
    
    public Dictionary<FlightType, double> GetFlightTypePercentages() => 
        _flightTypeCounts.ToDictionary(
            kvp => kvp.Key, 
            kvp => _totalFlightsProcessed > 0 ? (double)kvp.Value / _totalFlightsProcessed * 100 : 0);
    
    public Dictionary<string, int> GetTopAirlines(int count = 5) => 
        _airlineOperations.OrderByDescending(kvp => kvp.Value)
            .Take(count)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    
    // New method to get special flight statistics
    public int GetSpecialFlightCount() => _specialFlightsProcessed;
    
    public double GetSpecialFlightPercentage() => 
        _totalFlightsProcessed > 0 ? (double)_specialFlightsProcessed / _totalFlightsProcessed * 100 : 0;
                          
    public int TotalPassengersServed => CalculateTotalPassengers();
    
    public double AveragePassengersPerFlight => _totalFlightsProcessed > 0 ? 
        (double)CalculateTotalPassengers() / _totalFlightsProcessed : 0;
    
    private int CalculateTotalPassengers()
    {
        // In a real implementation, we'd track this properly
        return _totalFlightsProcessed * 120; // Rough estimate of 120 passengers per flight
    }

    public List<FlightInfo> GetUpcomingFlights(int maxCount = 5)
    {
        var flights = airport.FlightScheduler.GetUnlandedFlights();
        var result = new List<FlightInfo>();

        int displayCount = Math.Min(flights.Count, maxCount);
        
        for (int i = 0; i < displayCount; i++)
        {
            var flight = flights[i];
            var flightInfo = new FlightInfo
            {
                FlightNumber = flight.FlightNumber,
                FlightType = flight.Type,
                Priority = flight.Priority,
                PlaneSize = flight.Plane.Size,
                Passengers = flight.Passengers,
                ScheduledLandingTime = flight.ScheduledLandingTime,
                EstimatedRevenue = CalculateEstimatedRevenue(flight),
                Status = flight.Status,
                IsEmergency = flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency,
                IsDelayed = flight.IsDelayed(),
                IsSpecial = flight.IsSpecial // Add special flight flag
            };
            result.Add(flightInfo);
        }

        return result;
    }

    private double CalculateEstimatedRevenue(Flight flight)
    {
        // Simple revenue calculation as placeholder
        double baseRevenue = flight.Passengers * 10;
        double typeMultiplier = flight.Type switch
        {
            FlightType.Commercial => 1.0,
            FlightType.Cargo => 0.75,
            FlightType.VIP => 2.0,
            FlightType.Emergency => 1.5,
            _ => 1.0
        };
        
        // Apply double revenue for special flights
        double specialMultiplier = flight.IsSpecial ? 2.0 : 1.0;
        
        return baseRevenue * typeMultiplier * specialMultiplier;
    }

    public int TotalScheduledFlights => airport.FlightScheduler.GetUnlandedFlights().Count;
}