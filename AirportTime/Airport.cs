// Airport.cs
public class Airport
{
    public string Name { get; }
    
    // Core services
    public Treasury Treasury { get; }
    public RunwayManager RunwayManager { get; }
    public Shop Shop { get; }
    public GameLogger GameLogger { get; }
    
    // Game systems
    public FlightScheduler FlightScheduler { get; }
    public EventSystem EventSystem { get; }
    public ModifierManager ModifierManager { get; }
    public FlightLandingManager LandingManager { get; }
    public ExperienceSystem ExperienceSystem { get; }
    public AchievementSystem AchievementSystem { get; }
    
    // Business services
    private readonly FlightGenerationService _flightGenerationService;
    private readonly FlightProcessingService _flightProcessingService;
    private readonly IRandomGenerator _randomGenerator;

    public Airport(
        string name,
        Treasury treasury,
        RunwayManager runwayManager,
        Shop shop,
        FlightScheduler flightScheduler,
        EventSystem eventSystem,
        GameLogger gameLogger,
        ModifierManager modifierManager,
        ExperienceSystem experienceSystem,
        AchievementSystem achievementSystem,
        FlightLandingManager landingManager,
        FlightGenerationService flightGenerationService,
        FlightProcessingService flightProcessingService,
        IRandomGenerator randomGenerator)
    {
        Name = name;
        Treasury = treasury;
        RunwayManager = runwayManager;
        Shop = shop;
        FlightScheduler = flightScheduler;
        EventSystem = eventSystem;
        GameLogger = gameLogger;
        ModifierManager = modifierManager;
        ExperienceSystem = experienceSystem;
        AchievementSystem = achievementSystem;
        LandingManager = landingManager;
        _flightGenerationService = flightGenerationService;
        _flightProcessingService = flightProcessingService;
        _randomGenerator = randomGenerator;
        
        // Setup event handlers
        ExperienceSystem.OnLevelUp += HandleLevelUp;
        AchievementSystem.OnAchievementUnlocked += HandleAchievementUnlocked;
        LandingManager.OnFlightLanded += HandleFlightLanded;
        
        // Initialize shop achievement handling
        Shop.InitializeAchievementHandling(this);
    }

    public void Tick(int currentTick)
    {
        // Accumulate gold at every tick
        Treasury.AccumulateGold();
        
        // Update runway occupation status
        RunwayManager.UpdateRunwaysStatus();
        
        // Track active flights for XP calculation
        _flightProcessingService.UpdateActiveFlights();

        // Process scheduled flights for the current tick
        _flightProcessingService.ProcessScheduledFlights(currentTick);
        
        // Generate new flights if needed
        _flightGenerationService.GenerateFlightsIfNeeded(currentTick);
    }

    public void ToggleLandingMode()
    {
        LandingManager.ToggleLandingMode();
    }

    // Handle a flight cancellation
    public void HandleFlightCancelled(Flight flight)
    {
        GameLogger.Log($"Flight {flight.FlightNumber} has been cancelled.");
        
        // Reset consecutive flights counter in achievement system
        AchievementSystem.ResetConsecutiveFlights();
        
        // Other cancellation logic...
    }

    // Helper method to determine if it's night time based on game clock
    private bool IsNightTime(int currentTick)
    {
        // Game time (10 minutes per tick as an example)
        int gameHours = (currentTick % (24 * 60 / 10)) / (60 / 10);
        // Consider night time between 22:00 and 6:00
        return gameHours >= 22 || gameHours < 6;
    }

    #region Event Handlers
    
    // In Airport.cs, update HandleFlightLanded:
    private void HandleFlightLanded(Flight flight, Runway runway, bool isOnTime, int currentTick)
    {
        // Get the current weather
        Weather weather = new Weather(_randomGenerator);
    
        // Get the runway wear level
        int runwayWear = RunwayManager.GetMaintenanceSystem().GetWearLevel(runway.Name);
    
        // Check if this was a perfect landing (no delays)
        bool perfectLanding = !flight.IsDelayed() && isOnTime;
    
        // Get actual simultaneous flight count from the processing service
        int simultaneousFlights = _flightProcessingService.GetActiveFlights().Count;
    
        // Check if it's night time
        bool isNightTime = IsNightTime(currentTick);
    
        // Calculate XP for this landing
        int xpEarned = ExperienceSystem.CalculateFlightXP(
            flight, 
            weather, 
            runwayWear, 
            isOnTime, 
            perfectLanding, 
            simultaneousFlights
        );
    
        // Add the earned XP
        ExperienceSystem.AddExperience(xpEarned);
    
        // Record the flight in the achievement system with simultaneous flights count
        AchievementSystem.RecordFlightLanded(
            flight, 
            perfectLanding, 
            runwayWear,
            weather.CurrentWeather,
            isNightTime,
            simultaneousFlights
        );
    }


    private void HandleAchievementUnlocked(Achievement achievement)
    {
        GameLogger.Log($"Achievement unlocked: {achievement.Name}");
        
        // Get the next item ID
        int nextItemId = Shop.GetNextItemId();
        
        // Create appropriate buff based on achievement type
        IPurchasable buff = achievement.Type switch
        {
            AchievementType.FlightTypeSpecialization => FlightSpecializationBuff.FromAchievement(achievement, nextItemId),
            AchievementType.PerfectLandings => XPBuff.FromAchievement(achievement, nextItemId),
            AchievementType.RunwayExpert => RunwayMaintenanceBuff.FromAchievement(achievement, nextItemId),
            AchievementType.PassengerMilestone => GoldIncomeBuff.FromAchievement(achievement, nextItemId),
            AchievementType.WeatherMaster => WeatherMasterBuff.FromAchievement(achievement, nextItemId),
            AchievementType.NightFlight => NightFlightBuff.FromAchievement(achievement, nextItemId),
            AchievementType.ConsecutiveFlights => new XPBuff(
                nextItemId,
                $"Air Traffic Mastery {achievement.Tier}",
                $"Increases XP earned from all flights by {achievement.Tier * 7.5:F1}%",
                achievement.Tier * 2500,
                1.0 + (achievement.Tier * 0.075),
                achievement.Tier
            ),
            AchievementType.SimultaneousFlights => new RunwayBuff(
                nextItemId,
                $"Air Traffic Efficiency {achievement.Tier}",
                $"Reduces landing duration by {achievement.Tier * 5:F0}%",
                achievement.Tier * 3500,
                BuffType.LandingDurationReduction,
                1.0 - (achievement.Tier * 0.05),
                achievement.Tier
            ),
            AchievementType.EmergencyLandings => new FlightSpecializationBuff(
                nextItemId,
                $"Emergency Response {achievement.Tier}",
                $"Increases Emergency flight revenue by {achievement.Tier * 15:F0}%",
                achievement.Tier * 4000,
                FlightType.Emergency,
                1.0 + (achievement.Tier * 0.15),
                achievement.Tier
            ),
            _ => null
        };
        
        // Add the buff to the shop if one was created
        if (buff != null)
        {
            Shop.AddItemToShop(buff);
            GameLogger.Log($"New item added to shop: {buff.Name} - {buff.Description} - Price: {buff.Price:C}");
        }
    }
    
    private void HandleLevelUp(int newLevel)
    {
        // Give a gold bonus for leveling up
        double goldBonus = 1000 * newLevel;
        Treasury.AddFunds(goldBonus, $"Level Up Bonus (Level {newLevel})");
        
        // Log the level up
        GameLogger.Log($"AIRPORT LEVEL UP! Now Level {newLevel}");
        GameLogger.Log($"Level {newLevel} Benefits: Gold Bonus: {goldBonus:C}, Increased Flight Complexity, New Features Unlocked");
        
        // Check for queued shop items to unlock at this level
        Shop.CheckQueuedItemsForLevel(newLevel);
        
        // Unlock new features based on level
        UnlockFeaturesForLevel(newLevel);
    }
    
    private void UnlockFeaturesForLevel(int level)
    {
        switch (level)
        {
            case 3:
                // Add a revenue modifier for higher airport reputation
                ModifierManager.AddModifier("High Airport Reputation", 1.25);
                RunwayManager.ReduceAllRunwayLandingDurations(0.96);
                GameLogger.Log("Reputation Bonus: All flights now generate 25% more revenue!");
                break;
                
            case 4:
            case 5:
            case 6:
                RunwayManager.ReduceAllRunwayLandingDurations(0.96);
                break;
                
            case 7:
                // Add a weather resistance modifier
                RunwayManager.AddWeatherResistance(0.3);
                GameLogger.Log("Weather Resistance: Runways now take 30% less damage from adverse weather!");
                break;
                
            case 10:
                // Add a VIP and emergency flight specialist
                ModifierManager.AddModifier("Flight Specialist", 1.5);
                GameLogger.Log("Flight Specialist: VIP and Emergency flights now generate 50% more revenue!");
                break;
                
            default:
                // For other levels, add a small revenue boost
                if (level > 3 && level % 2 == 0)
                {
                    double boost = 1.0 + (level * 0.01);
                    ModifierManager.AddModifier($"Level {level} Efficiency", boost);
                    GameLogger.Log($"Efficiency Boost: All flights now generate {(boost - 1.0) * 100:F0}% more revenue!");
                }
                break;
        }
    }
    
    #endregion
}
// WeatherMasterBuff.cs

// NightFlightBuff.cs