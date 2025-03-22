/// <summary>
/// Main airport class that coordinates all game systems
/// </summary>
public class Airport
{
    public string Name { get; }
    
    // Core services
    public ITreasury Treasury { get; }
    public IRunwayManager RunwayManager { get; }
    public IShop Shop { get; }
    public IGameLogger GameLogger { get; }
    
    // Game systems
    public IFlightScheduler FlightScheduler { get; }
    public IEventSystem EventSystem { get; }
    public IModifierManager ModifierManager { get; }
    public IFlightLandingManager LandingManager { get; }
    public IExperienceSystem ExperienceSystem { get; }
    public IAchievementSystem AchievementSystem { get; }
    
    // New components
    public IFailureTracker FailureTracker { get; }
    public IEmergencyFlightHandler EmergencyFlightHandler { get; }
    
    // Business services
    private readonly IFlightGenerationService _flightGenerationService;
    private readonly IFlightProcessingService _flightProcessingService;
    private readonly IRandomGenerator _randomGenerator;
    
    // Game state
    public bool IsGameOver { get; private set; } = false;
    public FailureType? GameOverReason { get; private set; } = null;

    /// <summary>
    /// Creates a new Airport instance with all dependencies
    /// </summary>
    public Airport(
        string name,
        ITreasury treasury,
        IRunwayManager runwayManager,
        IShop shop,
        IFlightScheduler flightScheduler,
        IEventSystem eventSystem,
        IGameLogger gameLogger,
        IModifierManager modifierManager,
        IExperienceSystem experienceSystem,
        IAchievementSystem achievementSystem,
        IFlightLandingManager landingManager,
        IFlightGenerationService flightGenerationService,
        IFlightProcessingService flightProcessingService,
        IRandomGenerator randomGenerator,
        IFailureTracker failureTracker,
        IEmergencyFlightHandler emergencyFlightHandler)
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
        FailureTracker = failureTracker;
        EmergencyFlightHandler = emergencyFlightHandler;
        
        // Setup event handlers
        ExperienceSystem.OnLevelUp += HandleLevelUp;
        AchievementSystem.OnAchievementUnlocked += HandleAchievementUnlocked;
        LandingManager.OnFlightLanded += HandleFlightLanded;
        FailureTracker.OnGameOver += HandleGameOver;
        
        // Initialize shop achievement handling
        Shop.InitializeAchievementHandling(this);
    }

    /// <summary>
    /// Updates the airport state for the current tick
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    public void Tick(int currentTick)
    {
        // Skip processing if game is over
        if (IsGameOver) return;
        
        // Process emergencies
        EmergencyFlightHandler.ProcessEmergencies(currentTick);
        
        // Accumulate gold at every tick
        Treasury.AccumulateGold();
        
        // Check for financial shortfall
        CheckFinancialStatus();
        
        // Update runway occupation status
        RunwayManager.UpdateRunwaysStatus();
        
        // Track active flights for XP calculation
        _flightProcessingService.UpdateActiveFlights();

        // Process scheduled flights for the current tick
        _flightProcessingService.ProcessScheduledFlights(currentTick);
        
        // Generate new flights if needed
        _flightGenerationService.GenerateFlightsIfNeeded(currentTick);
    }

    /// <summary>
    /// Toggles between automatic and manual landing modes
    /// </summary>
    public void ToggleLandingMode()
    {
        LandingManager.ToggleLandingMode();
    }

    /// <summary>
    /// Handles a flight cancellation
    /// </summary>
    /// <param name="flight">The cancelled flight</param>
    public void HandleFlightCancelled(Flight flight)
    {
        GameLogger.Log($"Flight {flight.FlightNumber} has been cancelled.");
        
        // Record cancellation failure
        FailureTracker.RecordFailure(
            FailureType.FlightCancellation,
            $"Flight {flight.FlightNumber} ({flight.Type}) was cancelled"
        );
        
        // Reset consecutive flights counter in achievement system
        AchievementSystem.ResetConsecutiveFlights();
    }
    
    /// <summary>
    /// Checks for financial shortfall
    /// </summary>
    private void CheckFinancialStatus()
    {
        // Consider it a failure if treasury goes below -5000
        if (Treasury.GetBalance() < -5000)
        {
            FailureTracker.RecordFailure(
                FailureType.FinancialShortfall,
                $"Treasury balance fell below critical threshold: {Treasury.GetBalance():C}"
            );
        }
    }

    /// <summary>
    /// Helper method to determine if it's night time based on game clock
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    /// <returns>True if it's night time</returns>
    private bool IsNightTime(int currentTick)
    {
        // Game time (10 minutes per tick as an example)
        int gameHours = (currentTick % (24 * 60 / 10)) / (60 / 10);
        // Consider night time between 22:00 and 6:00
        return gameHours >= 22 || gameHours < 6;
    }
    
    /// <summary>
    /// Game over handler
    /// </summary>
    private void HandleGameOver(FailureType failureType)
    {
        IsGameOver = true;
        GameOverReason = failureType;
        
        GameLogger.Log($"🚨 GAME OVER: Too many {failureType} failures!");
        
        // Display game over message
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n\n");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"                      GAME OVER");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"Your airport management career has been terminated due to:");
        Console.WriteLine($"\n      Too many {failureType} failures!\n");
        Console.WriteLine($"Level reached: {ExperienceSystem.CurrentLevel}");
        Console.WriteLine($"Final balance: {Treasury.GetBalance():C}");
        Console.WriteLine($"Achievements unlocked: {AchievementSystem.GetUnlockedAchievements().Count}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("\nPress any key to exit...");
        Console.ResetColor();
    }

    #region Event Handlers
    
    /// <summary>
    /// Handles flight landing events
    /// </summary>
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

    /// <summary>
    /// Handles achievement unlocked events
    /// </summary>
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
    
    /// <summary>
    /// Handles level up events
    /// </summary>
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
    
    /// <summary>
    /// Unlocks features based on level
    /// </summary>
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