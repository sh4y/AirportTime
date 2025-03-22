using AirportTime;

/// <summary>
/// Manages probability distributions for flight generation
/// </summary>
public class FlightSpawnProfile
{
    private readonly IRandomGenerator random;
    
    // Base probabilities for flight types
    private readonly Dictionary<FlightType, double> flightTypeProbabilities = new Dictionary<FlightType, double>
    {
        { FlightType.Commercial, 0.65 },
        { FlightType.Cargo, 0.20 },
        { FlightType.VIP, 0.10 },
        { FlightType.Emergency, 0.05 }
    };
    
    // Base probabilities for flight priorities
    private readonly Dictionary<FlightPriority, double> flightPriorityProbabilities = new Dictionary<FlightPriority, double>
    {
        { FlightPriority.Standard, 0.80 },
        { FlightPriority.VIP, 0.15 },
        { FlightPriority.Emergency, 0.05 }
    };
    
    // Base probabilities for plane sizes
    private readonly Dictionary<PlaneSize, double> planeSizeProbabilities = new Dictionary<PlaneSize, double>
    {
        { PlaneSize.Small, 0.45 },
        { PlaneSize.Medium, 0.40 },
        { PlaneSize.Large, 0.15 }
    };
    
    public FlightSpawnProfile(IRandomGenerator random)
    {
        this.random = random;
    }
    
    /// <summary>
    /// Selects a flight type based on probabilities adjusted for airport level
    /// </summary>
    public FlightType GetRandomFlightType(int airportLevel)
    {
        // Adjust probabilities based on airport level
        Dictionary<FlightType, double> adjustedProbabilities = AdjustFlightTypeProbabilities(airportLevel);
        
        // Select a random flight type using the adjusted probabilities
        return SelectRandomWithProbability(adjustedProbabilities);
    }
    
    /// <summary>
    /// Selects a flight priority based on probabilities adjusted for airport level
    /// </summary>
    public FlightPriority GetRandomFlightPriority(int airportLevel)
    {
        // Adjust probabilities based on airport level
        Dictionary<FlightPriority, double> adjustedProbabilities = AdjustFlightPriorityProbabilities(airportLevel);
        
        // Select a random flight priority using the adjusted probabilities
        return SelectRandomWithProbability(adjustedProbabilities);
    }
    
    /// <summary>
    /// Selects a plane size based on probabilities adjusted for airport level
    /// </summary>
    public PlaneSize GetRandomPlaneSize(int airportLevel)
    {
        // Adjust probabilities based on airport level
        Dictionary<PlaneSize, double> adjustedProbabilities = AdjustPlaneSizeProbabilities(airportLevel);
        
        // Select a random plane size using the adjusted probabilities
        return SelectRandomWithProbability(adjustedProbabilities);
    }
    
    /// <summary>
    /// Determines how many flights to generate in a batch based on airport level
    /// </summary>
    public int GetFlightBatchSize(int airportLevel)
    {
        // Base size is 1
        int baseSize = 1;
        
        // Add chance for additional flights based on level
        double additionalFlightChance = Math.Min(0.1 + (airportLevel * 0.05), 0.8);
        
        // Calculate size by checking chance for each additional flight
        int batchSize = baseSize;
        while (random.NextDouble() < additionalFlightChance && batchSize < (airportLevel / 2) + 2)
        {
            batchSize++;
            // Reduce chance for next additional flight
            additionalFlightChance *= 0.7;
        }
        
        return batchSize;
    }
    public PlaneSize GetRandomPlaneSizeWithAvailability(int airportLevel, bool smallAvailable, 
        bool mediumAvailable, bool largeAvailable)
    {
        // Create adjusted probabilities based on availability
        Dictionary<PlaneSize, double> adjustedProbabilities = new Dictionary<PlaneSize, double>();
    
        if (smallAvailable) adjustedProbabilities[PlaneSize.Small] = planeSizeProbabilities[PlaneSize.Small];
        if (mediumAvailable) adjustedProbabilities[PlaneSize.Medium] = planeSizeProbabilities[PlaneSize.Medium];
        if (largeAvailable) adjustedProbabilities[PlaneSize.Large] = planeSizeProbabilities[PlaneSize.Large];
    
        // Normalize probabilities
        NormalizeProbabilities(adjustedProbabilities);
    
        // Return random plane size based on adjusted probabilities
        return SelectRandomWithProbability(adjustedProbabilities);
    }
    /// <summary>
    /// Calculates the frequency of flight generation based on airport level
    /// Lower number means more frequent flights
    /// </summary>
    public int GetFlightGenerationFrequency(int airportLevel)
    {
        // Start with a base frequency of 15 ticks between flights
        int baseFrequency = 15;
        
        // Reduce by 0.5 per level, with a minimum of 3 ticks
        int frequency = Math.Max(baseFrequency - (airportLevel / 2), 3);
        
        return frequency;
    }
    
    /// <summary>
    /// Calculates passenger count for a flight based on plane size and airport level
    /// </summary>
    public int GetPassengerCount(PlaneSize planeSize, int airportLevel)
    {
        // Base passenger counts by plane size
        int baseCount = planeSize switch
        {
            PlaneSize.Small => random.Next(30, 80),
            PlaneSize.Medium => random.Next(100, 200),
            PlaneSize.Large => random.Next(250, 400),
            _ => 50
        };
        
        // Increase passenger count by airport level (0-20% increase)
        double levelMultiplier = 1.0 + (airportLevel * 0.01);
        
        return (int)(baseCount * levelMultiplier);
    }
    
    #region Private Helper Methods
    
    private Dictionary<FlightType, double> AdjustFlightTypeProbabilities(int airportLevel)
    {
        // Create a copy of base probabilities
        Dictionary<FlightType, double> adjusted = new Dictionary<FlightType, double>(flightTypeProbabilities);
        
        // Adjust based on airport level (higher levels get more challenging flights)
        double levelFactor = Math.Min(airportLevel * 0.02, 0.3);
        
        // Reduce commercial flights
        adjusted[FlightType.Commercial] = Math.Max(0.4, flightTypeProbabilities[FlightType.Commercial] - levelFactor);
        
        // Increase others proportionally
        double increasePerType = levelFactor / 3.0;
        adjusted[FlightType.Cargo] = Math.Min(0.35, flightTypeProbabilities[FlightType.Cargo] + increasePerType);
        adjusted[FlightType.VIP] = Math.Min(0.25, flightTypeProbabilities[FlightType.VIP] + increasePerType);
        adjusted[FlightType.Emergency] = Math.Min(0.20, flightTypeProbabilities[FlightType.Emergency] + increasePerType);
        
        // Normalize probabilities to ensure they sum to 1
        NormalizeProbabilities(adjusted);
        
        return adjusted;
    }
    
    private Dictionary<FlightPriority, double> AdjustFlightPriorityProbabilities(int airportLevel)
    {
        // Create a copy of base probabilities
        Dictionary<FlightPriority, double> adjusted = new Dictionary<FlightPriority, double>(flightPriorityProbabilities);
        
        // Adjust based on airport level (higher levels get more priority flights)
        double levelFactor = Math.Min(airportLevel * 0.015, 0.25);
        
        // Reduce standard priorities
        adjusted[FlightPriority.Standard] = Math.Max(0.5, flightPriorityProbabilities[FlightPriority.Standard] - levelFactor);
        
        // Increase others proportionally
        adjusted[FlightPriority.VIP] = Math.Min(0.35, flightPriorityProbabilities[FlightPriority.VIP] + (levelFactor * 0.7));
        adjusted[FlightPriority.Emergency] = Math.Min(0.15, flightPriorityProbabilities[FlightPriority.Emergency] + (levelFactor * 0.3));
        
        // Normalize probabilities to ensure they sum to 1
        NormalizeProbabilities(adjusted);
        
        return adjusted;
    }
    
    private Dictionary<PlaneSize, double> AdjustPlaneSizeProbabilities(int airportLevel)
    {
        // Create a copy of base probabilities
        Dictionary<PlaneSize, double> adjusted = new Dictionary<PlaneSize, double>(planeSizeProbabilities);
        
        // Adjust based on airport level (higher levels get more large planes)
        double levelFactor = Math.Min(airportLevel * 0.02, 0.3);
        
        // Reduce small planes
        adjusted[PlaneSize.Small] = Math.Max(0.2, planeSizeProbabilities[PlaneSize.Small] - levelFactor);
        
        // Increase others proportionally
        adjusted[PlaneSize.Medium] = Math.Min(0.5, planeSizeProbabilities[PlaneSize.Medium] + (levelFactor * 0.5));
        adjusted[PlaneSize.Large] = Math.Min(0.3, planeSizeProbabilities[PlaneSize.Large] + (levelFactor * 0.5));
        
        // Normalize probabilities to ensure they sum to 1
        NormalizeProbabilities(adjusted);
        
        return adjusted;
    }
    
    private void NormalizeProbabilities<T>(Dictionary<T, double> probabilities)
    {
        // Calculate sum of all probabilities
        double sum = 0;
        foreach (var probability in probabilities.Values)
        {
            sum += probability;
        }
        
        // Normalize each probability
        if (Math.Abs(sum - 1.0) > 0.0001) // Only normalize if not already close to 1
        {
            foreach (var key in probabilities.Keys.ToList())
            {
                probabilities[key] /= sum;
            }
        }
    }
    
    private T SelectRandomWithProbability<T>(Dictionary<T, double> probabilities)
    {
        // Generate a random value between 0 and 1
        double randValue = random.NextDouble();
        
        // Track cumulative probability
        double cumulativeProbability = 0;
        
        // Check each item in the dictionary
        foreach (var pair in probabilities)
        {
            cumulativeProbability += pair.Value;
            
            // If random value is less than or equal to the cumulative probability,
            // select this item
            if (randValue <= cumulativeProbability)
            {
                return pair.Key;
            }
        }
        
        // Fallback: return the first item (should rarely happen due to normalization)
        return probabilities.Keys.First();
    }
    
    #endregion
}