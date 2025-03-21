public class FlightGenerator
{
    private readonly IRandomGenerator randomGenerator;
    private readonly FlightSpawnProfile spawnProfile;
    private int flightNumberCounter = 100;
    
    public FlightGenerator(IRandomGenerator randomGenerator)
    {
        this.randomGenerator = randomGenerator;
        this.spawnProfile = new FlightSpawnProfile(randomGenerator);
    }
    
    /// <summary>
    /// Generates a flight with random properties based on the airport level
    /// </summary>
    public Flight GenerateRandomFlight(int tick, int airportLevel)
    {
        // Get flight properties based on probability distributions
        PlaneSize planeSize = spawnProfile.GetRandomPlaneSize(airportLevel);
        FlightType flightType = spawnProfile.GetRandomFlightType(airportLevel);
        FlightPriority flightPriority = spawnProfile.GetRandomFlightPriority(airportLevel);
        
        // Get the passenger count based on plane size and airport level
        int passengerCount = spawnProfile.GetPassengerCount(planeSize, airportLevel);
        
        // Generate a unique flight number with airline code
        string airlineCode = GenerateAirlineCode(flightType);
        string flightNumber = $"{airlineCode}{flightNumberCounter++}";
        
        // Generate a random weight for the plane based on size
        double planeWeight = GeneratePlaneWeight(planeSize);
        
        // Create a plane with the generated properties
        Plane plane = new Plane(flightNumber, planeSize, planeWeight);
        
        // Determine the scheduled landing time: current tick plus a random offset
        int scheduledLandingTime = CalculateScheduledLandingTime(tick, airportLevel);
        
        // Create and return the flight with the generated properties
        return new Flight(flightNumber, plane, flightType, flightPriority, scheduledLandingTime, passengerCount);
    }
    
    /// <summary>
    /// Generate multiple flights at the given tick based on the airport level
    /// </summary>
    public List<Flight> GenerateFlightBatch(int tick, int airportLevel)
    {
        // Determine how many flights to generate based on airport level
        int batchSize = spawnProfile.GetFlightBatchSize(airportLevel);
        
        var flights = new List<Flight>();
        for (int i = 0; i < batchSize; i++)
        {
            flights.Add(GenerateRandomFlight(tick, airportLevel));
        }
        
        return flights;
    }
    
    /// <summary>
    /// Determines if a new flight batch should be generated based on airport level
    /// </summary>
    public bool ShouldGenerateFlights(int tick, int airportLevel)
    {
        // Get the generation frequency (ticks between flights)
        int frequency = spawnProfile.GetFlightGenerationFrequency(airportLevel);
        
        // Generate flights at intervals based on frequency
        return tick % frequency == 0;
    }
    
    #region Helper Methods
    
    private string GenerateAirlineCode(FlightType flightType)
    {
        // Different airline codes based on flight type
        string[] commercialAirlines = { "AA", "DL", "UA", "BA", "LH", "QF", "SQ" };
        string[] cargoAirlines = { "FX", "UPS", "DHL", "CAL" };
        string[] vipAirlines = { "PJ", "XO", "VP" };
        string[] emergencyAirlines = { "MED", "RES", "EMG" };
        
        // Select an airline code based on flight type
        string[] airlineCodes = flightType switch
        {
            FlightType.Commercial => commercialAirlines,
            FlightType.Cargo => cargoAirlines,
            FlightType.VIP => vipAirlines,
            FlightType.Emergency => emergencyAirlines,
            _ => commercialAirlines
        };
        
        // Randomly select an airline code from the appropriate array
        return airlineCodes[randomGenerator.Next(0, airlineCodes.Length)];
    }
    
    private double GeneratePlaneWeight(PlaneSize planeSize)
    {
        // Generate weight ranges based on plane size
        return planeSize switch
        {
            PlaneSize.Small => randomGenerator.Next(10000, 30000),
            PlaneSize.Medium => randomGenerator.Next(30000, 60000),
            PlaneSize.Large => randomGenerator.Next(60000, 100000),
            _ => randomGenerator.Next(10000, 30000)
        };
    }
    
    private int CalculateScheduledLandingTime(int currentTick, int airportLevel)
    {
        // Base offset range
        int minOffset = 5;
        int maxOffset = 15;
        
        // Adjust offset range based on airport level (higher levels get more compressed schedules)
        int adjustedMinOffset = Math.Max(3, minOffset - (airportLevel / 3));
        int adjustedMaxOffset = Math.Max(8, maxOffset - (airportLevel / 4));
        
        // Ensure min is less than max
        if (adjustedMinOffset >= adjustedMaxOffset)
        {
            adjustedMaxOffset = adjustedMinOffset + 1;
        }
        
        // Calculate scheduled time
        return currentTick + randomGenerator.Next(adjustedMinOffset, adjustedMaxOffset);
    }
    
    #endregion
}