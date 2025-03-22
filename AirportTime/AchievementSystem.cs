
// AchievementSystem.cs
public class AchievementSystem : IAchievementSystem
{
    private readonly Dictionary<string, int> flightTypeCounter = new Dictionary<string, int>();
    private readonly Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
    private readonly GameLogger logger;
    private readonly List<Achievement> achievements = new List<Achievement>();

    // Achievement counters
    private int perfectLandingCounter = 0;
    private int wornRunwayLandingCounter = 0;
    private int totalPassengers = 0;
    private readonly Dictionary<WeatherType, int> weatherLandingCounter = new Dictionary<WeatherType, int>();
    private int nightFlightCounter = 0;
    private int consecutiveFlightCounter = 0;
    private int maxSimultaneousFlights = 0;
    private int emergencyLandingCounter = 0;

    // Event for when an achievement is unlocked
    public event Action<Achievement> OnAchievementUnlocked;

    public AchievementSystem(GameLogger logger)
    {
        this.logger = logger;
        InitializeFlightTypeCounters();
        InitializeWeatherCounters();
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

    private void InitializeWeatherCounters()
    {
        // Initialize counters for each weather type
        foreach (WeatherType weatherType in Enum.GetValues(typeof(WeatherType)))
        {
            weatherLandingCounter[weatherType] = 0;
        }
    }

    private void RegisterAchievement(Achievement achievement)
    {
        achievements.Add(achievement);
        unlockedAchievements[achievement.Id] = false;
    }

 // In AchievementSystem.cs, modify:

// Update the RecordFlightLanded method to check active flights count properly
public void RecordFlightLanded(
    Flight flight, 
    bool isPerfectLanding = false, 
    int runwayWear = 0, 
    WeatherType currentWeather = WeatherType.Clear,
    bool isNightTime = false,
    int simultaneousFlights = 0)
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
    
    // Track weather-related landings
    if (currentWeather != WeatherType.Clear)
    {
        weatherLandingCounter[currentWeather]++;
        CheckWeatherMasterAchievements(currentWeather, weatherLandingCounter[currentWeather]);
    }
    
    // Track night flights
    if (isNightTime)
    {
        nightFlightCounter++;
        CheckNightFlightAchievements(nightFlightCounter);
    }
    
    // Track consecutive flights
    consecutiveFlightCounter++;
    CheckConsecutiveFlightAchievements(consecutiveFlightCounter);
    
    // Check if we have a new record for simultaneous flights
    if (simultaneousFlights > maxSimultaneousFlights)
    {
        maxSimultaneousFlights = simultaneousFlights;
        logger.Log($"New record: {maxSimultaneousFlights} flights managed simultaneously!");
        CheckSimultaneousFlightAchievements(maxSimultaneousFlights);
    }
    
    // Track emergency landings
    if (flight.Priority == FlightPriority.Emergency)
    {
        emergencyLandingCounter++;
        CheckEmergencyLandingAchievements(emergencyLandingCounter);
    }
    
    // Check for achievement unlocks
    CheckFlightTypeAchievements(flight.Type, newCount);
    
    // Log the milestone if it's a nice round number
    if (newCount == 10 || newCount == 30 || newCount == 100 || 
        newCount == 500 || newCount % 1000 == 0)
    {
        logger.Log($"Milestone: {newCount} {flightType} flights landed!");
    }
}    // Reset consecutive flights counter (call when a flight is canceled)
    public void ResetConsecutiveFlights()
    {
        consecutiveFlightCounter = 0;
    }

    /// <summary>
    /// Gets the flight count for a specific type
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
    /// Gets all achievements (locked and unlocked) with progress
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

    /// <summary>
    /// Update RegisterAchievements method to use our config
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

        // Register Passenger Milestone achievements
        GeneratePassengerMilestones(
            AchievementConfig.PassengerMilestoneBaseValue,
            AchievementConfig.PassengerMilestoneMinExponent,
            AchievementConfig.PassengerMilestoneMaxExponent);

        // Register Weather Master achievements
        GenerateWeatherMasterAchievements();

        // Register Night Flight achievements
        GenerateNightFlightAchievements();

        // Register Consecutive Flight achievements
        GenerateConsecutiveFlightAchievements();

        // Register Emergency Landing achievements
        GenerateEmergencyLandingAchievements();
    }

    /// <summary>
    /// Generates flight type specialization achievements for a specific flight type
    /// </summary>
    private void GenerateFlightTypeAchievements(FlightType flightType)
    {
        // Use thresholds from config
        int[] thresholds = AchievementConfig.FlightTypeThresholds;

        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"{flightType}Specialist_{tier}",
                $"{flightType} Specialist {tier}",
                $"Land {thresholds[tier - 1]} {flightType} flights",
                thresholds[tier - 1],
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
        // Use thresholds from config
        int[] thresholds = AchievementConfig.PerfectLandingThresholds;

        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"PerfectPilot_{tier}",
                $"Perfect Pilot {tier}",
                $"Land {thresholds[tier - 1]} flights without any delays",
                thresholds[tier - 1],
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
        // Use thresholds from config
        int[] thresholds = AchievementConfig.RunwayExpertThresholds;

        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"RunwayExpert_{tier}",
                $"Runway Expert {tier}",
                $"Land {thresholds[tier - 1]} flights on runways with high wear (>50%)",
                thresholds[tier - 1],
                AchievementType.RunwayExpert,
                FlightType.Commercial,
                tier));
        }
    }

    /// <summary>
    /// Generates passenger milestone achievements
    /// </summary>
    private void GeneratePassengerMilestones(int baseValue, int minExponent, int maxExponent)
    {
        // Verify exponent range is valid
        if (minExponent > maxExponent)
        {
            logger.Log($"Invalid exponent range: {minExponent} to {maxExponent}");
            return;
        }

        // Generate achievements for each exponent in the range
        for (int exponent = minExponent; exponent <= maxExponent; exponent++)
        {
            // Calculate the passenger count for this milestone
            int passengerCount = (int)Math.Pow(baseValue, exponent);

            // Get a creative title for this milestone or generate a default one
            string title = AchievementConfig.PassengerMilestoneTitles.ContainsKey(exponent)
                ? AchievementConfig.PassengerMilestoneTitles[exponent]
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
                exponent - minExponent + 1 // Tier increases with each exponent
            ));
        }
    }

    /// <summary>
    /// Generates weather master achievements for each weather type
    /// </summary>
    private void GenerateWeatherMasterAchievements()
    {
        foreach (var weatherType in AchievementConfig.WeatherMasterThresholds.Keys)
        {
            int[] thresholds = AchievementConfig.WeatherMasterThresholds[weatherType];

            for (int tier = 1; tier <= thresholds.Length; tier++)
            {
                RegisterAchievement(new Achievement(
                    $"WeatherMaster_{weatherType}_{tier}",
                    $"{weatherType} Master {tier}",
                    $"Land {thresholds[tier - 1]} flights during {weatherType} weather",
                    thresholds[tier - 1],
                    AchievementType.WeatherMaster,
                    FlightType.Commercial,
                    tier
                ));
            }
        }
    }

    /// <summary>
    /// Generates night flight achievements
    /// </summary>
    private void GenerateNightFlightAchievements()
    {
        int[] thresholds = AchievementConfig.NightFlightThresholds;

        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"NightFlight_{tier}",
                $"Night Owl {tier}",
                $"Land {thresholds[tier - 1]} flights during night time",
                thresholds[tier - 1],
                AchievementType.NightFlight,
                FlightType.Commercial,
                tier
            ));
        }
    }

    /// <summary>
    /// Generates consecutive flight achievements
    /// </summary>
    private void GenerateConsecutiveFlightAchievements()
    {
        int[] thresholds = AchievementConfig.ConsecutiveFlightThresholds;

        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"ConsecutiveFlights_{tier}",
                $"Air Traffic Controller {tier}",
                $"Land {thresholds[tier - 1]} flights consecutively without any cancellations",
                thresholds[tier - 1],
                AchievementType.ConsecutiveFlights,
                FlightType.Commercial,
                tier
            ));
        }
    }

    /// <summary>
    /// Generates simultaneous flight achievements
    /// </summary>
// Update the SimultaneousFlightAchievements generation method:
    private void GenerateSimultaneousFlightAchievements()
    {
        int[] thresholds = AchievementConfig.SimultaneousFlightThresholds;
    
        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"SimultaneousFlights_{tier}",
                $"Air Traffic Coordinator {tier}",
                $"Successfully manage {thresholds[tier-1]} flights in the air simultaneously",
                thresholds[tier-1],
                AchievementType.SimultaneousFlights,
                FlightType.Commercial,
                tier
            ));
        }
    }


    /// <summary>
    /// Generates emergency landing achievements
    /// </summary>
    private void GenerateEmergencyLandingAchievements()
    {
        int[] thresholds = AchievementConfig.EmergencyLandingThresholds;

        for (int tier = 1; tier <= thresholds.Length; tier++)
        {
            RegisterAchievement(new Achievement(
                $"EmergencyLandings_{tier}",
                $"Emergency Responder {tier}",
                $"Successfully land {thresholds[tier - 1]} emergency flights",
                thresholds[tier - 1],
                AchievementType.EmergencyLandings,
                FlightType.Emergency,
                tier
            ));
        }
    }

    // Achievement check methods

    private void CheckFlightTypeAchievements(FlightType flightType, int count)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.FlightTypeSpecialization &&
                achievement.RelatedFlightType == flightType &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                UnlockAchievement(achievement);
            }
        }
    }

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

    private void CheckWeatherMasterAchievements(WeatherType weatherType, int count)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.WeatherMaster &&
                achievement.Name.Contains(weatherType.ToString()) &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private void CheckNightFlightAchievements(int count)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.NightFlight &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private void CheckConsecutiveFlightAchievements(int count)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.ConsecutiveFlights &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private void CheckSimultaneousFlightAchievements(int count)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.SimultaneousFlights &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private void CheckEmergencyLandingAchievements(int count)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.EmergencyLandings &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
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
            case AchievementType.WeatherMaster:
                foreach (WeatherType weatherType in Enum.GetValues(typeof(WeatherType)))
                {
                    if (achievement.Name.Contains(weatherType.ToString()))
                    {
                        return weatherLandingCounter[weatherType];
                    }
                }

                return 0;
            case AchievementType.NightFlight:
                return nightFlightCounter;
            case AchievementType.ConsecutiveFlights:
                return consecutiveFlightCounter;
            case AchievementType.SimultaneousFlights:
                return maxSimultaneousFlights;
            case AchievementType.EmergencyLandings:
                return emergencyLandingCounter;
            default:
                return 0;
        }
    }
}