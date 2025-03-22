public class FlightMetrics
{
    private readonly Airport airport;

    public FlightMetrics(Airport airport)
    {
        this.airport = airport;
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
                IsDelayed = flight.IsDelayed()
            };
            result.Add(flightInfo);
        }

        return result;
    }

    private double CalculateEstimatedRevenue(Flight flight)
    {
        // Simple revenue calculation as placeholder
        double baseRevenue = flight.Passengers * 10;
        return flight.Type switch
        {
            FlightType.Commercial => baseRevenue,
            FlightType.Cargo => baseRevenue * 0.75,
            FlightType.VIP => baseRevenue * 2.0,
            FlightType.Emergency => baseRevenue * 1.5,
            _ => baseRevenue
        };
    }

    public int TotalScheduledFlights => airport.FlightScheduler.GetUnlandedFlights().Count;
}