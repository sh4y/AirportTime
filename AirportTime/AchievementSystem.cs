/// <summary>
/// Tracks player achievements and unlocks rewards based on progress
/// </summary>
public class AchievementSystem
{
    private readonly Dictionary<string, int> flightTypeCounter = new Dictionary<string, int>();
    private readonly Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
    private readonly GameLogger logger;
    private readonly List<Achievement> achievements = new List<Achievement>();
    
    // Event for when an achievement is unlocked
    public event Action<Achievement> OnAchievementUnlocked;
    
    public AchievementSystem(GameLogger logger)
    {
        this.logger = logger;
        InitializeFlightTypeCounters();
        RegisterAchievements();
    }
    
    private void InitializeFlightTypeCounters()
    {
        // Initialize counters for each flight type
        foreach (FlightType flightType in Enum.GetValues(typeof(FlightType)))
        {
            flightTypeCounter[$"{flightType}"] = 0;
        }
    }
    
    private void RegisterAchievement(Achievement achievement)
    {
        achievements.Add(achievement);
        unlockedAchievements[achievement.Id] = false;
    }
    
    /// <summary>
    /// Records a flight landing and updates counters
    /// </summary>
    public void RecordFlightLanded(Flight flight)
    {
        string flightType = flight.Type.ToString();
        
        // Increment counter for this flight type
        flightTypeCounter[flightType]++;
        int newCount = flightTypeCounter[flightType];
        
        // Check for achievement unlocks
        CheckFlightTypeAchievements(flight.Type, newCount);
        
        // Log the milestone if it's a nice round number
        if (newCount == 10 || newCount == 30 || newCount == 100 || 
            newCount == 500 || newCount % 1000 == 0)
        {
            logger.Log($"Milestone: {newCount} {flightType} flights landed!");
        }
    }
    
    private void CheckFlightTypeAchievements(FlightType flightType, int count)
    {
        // Find all achievements for this flight type that haven't been unlocked yet
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.FlightTypeSpecialization && 
                achievement.RelatedFlightType == flightType &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                // Unlock the achievement
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void UnlockAchievement(Achievement achievement)
    {
        unlockedAchievements[achievement.Id] = true;
        logger.Log($"🏆 Achievement Unlocked: {achievement.Name} - {achievement.Description}");
        
        // Trigger the event
        OnAchievementUnlocked?.Invoke(achievement);
    }
    
    /// <summary>
    /// Gets the count of flights landed for a specific type
    /// </summary>
    public int GetFlightTypeCount(FlightType flightType)
    {
        return flightTypeCounter[flightType.ToString()];
    }
    
    /// <summary>
    /// Gets all unlocked achievements
    /// </summary>
    public List<Achievement> GetUnlockedAchievements()
    {
        List<Achievement> result = new List<Achievement>();
        
        foreach (var achievement in achievements)
        {
            if (unlockedAchievements[achievement.Id])
            {
                result.Add(achievement);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets all achievements (locked and unlocked)
    /// </summary>
    public List<AchievementStatus> GetAllAchievements()
    {
        List<AchievementStatus> result = new List<AchievementStatus>();
        
        foreach (var achievement in achievements)
        {
            result.Add(new AchievementStatus(
                achievement,
                unlockedAchievements[achievement.Id],
                GetProgressForAchievement(achievement)
            ));
        }
        
        return result;
    }
    
    // Add these fields to track different achievement types
private int perfectLandingCounter = 0;
private int wornRunwayLandingCounter = 0;
private int totalPassengers = 0;

/// <summary>
/// Update RegisterAchievements method to use our new generator
/// </summary>
private void RegisterAchievements()
{
    // Register Flight Type Specialization achievements for each flight type
    foreach (FlightType flightType in Enum.GetValues(typeof(FlightType)))
    {
        GenerateFlightTypeAchievements(flightType);
    }
    
    // Register Perfect Pilot achievements
    GeneratePerfectPilotAchievements();
    
    // Register Runway Expert achievements
    GenerateRunwayExpertAchievements();
    
    // Register Passenger Milestone achievements (powers of 2 from 2^6 to 2^13)
    GeneratePassengerMilestones(2, 13, 22);
}

/// <summary>
/// Generates flight type specialization achievements for a specific flight type
/// </summary>
private void GenerateFlightTypeAchievements(FlightType flightType)
{
    // Threshold values for each tier
    int[] thresholds = { 10, 30, 100, 500 };
    
    for (int tier = 1; tier <= thresholds.Length; tier++)
    {
        RegisterAchievement(new Achievement(
            $"{flightType}Specialist_{tier}", 
            $"{flightType} Specialist {tier}", 
            $"Land {thresholds[tier-1]} {flightType} flights", 
            thresholds[tier-1], 
            AchievementType.FlightTypeSpecialization,
            flightType,
            tier));
    }
}

/// <summary>
/// Generates perfect pilot achievements
/// </summary>
private void GeneratePerfectPilotAchievements()
{
    // Threshold values for each tier
    int[] thresholds = { 5, 20, 50, 200 };
    
    for (int tier = 1; tier <= thresholds.Length; tier++)
    {
        RegisterAchievement(new Achievement(
            $"PerfectPilot_{tier}", 
            $"Perfect Pilot {tier}", 
            $"Land {thresholds[tier-1]} flights without any delays", 
            thresholds[tier-1], 
            AchievementType.PerfectLandings,
            FlightType.Commercial,
            tier));
    }
}

/// <summary>
/// Generates runway expert achievements
/// </summary>
private void GenerateRunwayExpertAchievements()
{
    // Threshold values for each tier
    int[] thresholds = { 10, 30, 60 };
    
    for (int tier = 1; tier <= thresholds.Length; tier++)
    {
        RegisterAchievement(new Achievement(
            $"RunwayExpert_{tier}", 
            $"Runway Expert {tier}", 
            $"Land {thresholds[tier-1]} flights on runways with high wear (>50%)", 
            thresholds[tier-1], 
            AchievementType.RunwayExpert,
            FlightType.Commercial,
            tier));
    }
}

/// <summary>
/// Generates passenger milestone achievements based on powers of a specified base number
/// </summary>
/// <param name="baseValue">The base value (e.g., 2 for powers of 2)</param>
/// <param name="minExponent">The minimum exponent to use</param>
/// <param name="maxExponent">The maximum exponent to use</param>
private void GeneratePassengerMilestones(int baseValue, int minExponent, int maxExponent)
{
    // Verify exponent range is valid
    if (minExponent > maxExponent)
    {
        logger.Log($"Invalid exponent range: {minExponent} to {maxExponent}");
        return;
    }
    
    // Dictionary of creative titles for different milestones
    Dictionary<int, string> milestoneTitles = new Dictionary<int, string>()
    {
        { 6, "First Steps" },
        { 7, "Taking Off" },
        { 8, "Regional Hub" },
        { 9, "People Mover" },
        { 10, "Terminal Bustle" },
        { 11, "Thousand Club" },
        { 12, "People Planet" },
        { 13, "Passenger Paradise" },
        { 14, "Aviation Empire" },
        { 15, "Skybound Metropolis" },
        { 16, "Global Gateway" },
        { 17, "Passenger Kingdom" },
        { 18, "Interstellar Terminal" },
        { 19, "Galactic Transport" },
        { 20, "Universal Transit Hub" }
    };
    
    // Generate achievements for each exponent in the range
    for (int exponent = minExponent; exponent <= maxExponent; exponent++)
    {
        // Calculate the passenger count for this milestone
        int passengerCount = (int)Math.Pow(baseValue, exponent);
        
        // Get a creative title for this milestone or generate a default one
        string title = milestoneTitles.ContainsKey(exponent) 
            ? milestoneTitles[exponent] 
            : $"Passenger Milestone {exponent}";
        
        // Format passenger count with commas for readability in description
        string formattedCount = passengerCount.ToString("N0");
        
        // Create and register the achievement
        RegisterAchievement(new Achievement(
            $"PassengerMilestone_{passengerCount}", 
            title, 
            $"Welcome {formattedCount} passengers to your airport", 
            passengerCount, 
            AchievementType.PassengerMilestone,
            FlightType.Commercial,
            exponent - minExponent + 1  // Tier increases with each exponent
        ));
        
        logger.Log($"Created passenger milestone achievement: {title} ({passengerCount} passengers)");
    }
}


// Update RecordFlightLanded method to track all achievement types
public void RecordFlightLanded(Flight flight, bool isPerfectLanding = false, int runwayWear = 0)
{
    string flightType = flight.Type.ToString();
    
    // Increment counter for this flight type
    flightTypeCounter[flightType]++;
    int newCount = flightTypeCounter[flightType];
    
    // Track perfect landings if applicable
    if (isPerfectLanding)
    {
        perfectLandingCounter++;
        CheckPerfectLandingAchievements(perfectLandingCounter);
    }
    
    // Track landings on worn runways
    if (runwayWear > 50) // More than 50% wear
    {
        wornRunwayLandingCounter++;
        CheckRunwayExpertAchievements(wornRunwayLandingCounter);
    }
    
    // Track total passengers
    totalPassengers += flight.Passengers;
    CheckPassengerMilestoneAchievements(totalPassengers);
    
    // Check for achievement unlocks
    CheckFlightTypeAchievements(flight.Type, newCount);
    
    // Log the milestone if it's a nice round number
    if (newCount == 10 || newCount == 30 || newCount == 100 || 
        newCount == 500 || newCount % 1000 == 0)
    {
        logger.Log($"Milestone: {newCount} {flightType} flights landed!");
    }
}

// Add these methods to check for the new achievement types
private void CheckPerfectLandingAchievements(int count)
{
    foreach (var achievement in achievements)
    {
        if (achievement.Type == AchievementType.PerfectLandings && 
            !unlockedAchievements[achievement.Id] &&
            count >= achievement.RequiredCount)
        {
            UnlockAchievement(achievement);
        }
    }
}

private void CheckRunwayExpertAchievements(int count)
{
    foreach (var achievement in achievements)
    {
        if (achievement.Type == AchievementType.RunwayExpert && 
            !unlockedAchievements[achievement.Id] &&
            count >= achievement.RequiredCount)
        {
            UnlockAchievement(achievement);
        }
    }
}

private void CheckPassengerMilestoneAchievements(int count)
{
    foreach (var achievement in achievements)
    {
        if (achievement.Type == AchievementType.PassengerMilestone && 
            !unlockedAchievements[achievement.Id] &&
            count >= achievement.RequiredCount)
        {
            UnlockAchievement(achievement);
        }
    }
}

// Update GetProgressForAchievement method to handle all achievement types
private int GetProgressForAchievement(Achievement achievement)
{
    switch (achievement.Type)
    {
        case AchievementType.FlightTypeSpecialization:
            return flightTypeCounter[achievement.RelatedFlightType.ToString()];
        case AchievementType.PerfectLandings:
            return perfectLandingCounter;
        case AchievementType.RunwayExpert:
            return wornRunwayLandingCounter;
        case AchievementType.PassengerMilestone:
            return totalPassengers;
        default:
            return 0;
    }
}
}