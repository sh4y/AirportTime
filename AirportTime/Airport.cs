public class Airport
{
    public string Name { get; private set; }
    public Treasury Treasury { get; private set; }
    public RunwayManager RunwayManager { get; private set; }
    public Shop Shop { get; private set; }
    public FlightScheduler FlightScheduler { get; private set; }
    public EventSystem EventSystem { get; private set; }
    public GameLogger GameLogger { get; private set; }
    public ModifierManager ModifierManager { get; private set; }
    public FlightLandingManager LandingManager { get; private set; }
    public ExperienceSystem ExperienceSystem { get; private set; }
    public AchievementSystem AchievementSystem { get; private set; }
    private Revenue AirportRevenue { get; set; }
    private readonly IRandomGenerator RandomGenerator;
    private EventScheduler flightEventScheduler = new EventScheduler();

    // Track active flights for XP calculation
    private readonly List<Flight> activeFlights = new List<Flight>();

    public Airport(string name, double startingGold)
    {
        Name = name;
        GameLogger = new GameLogger("GameLogs.db");
        Treasury = new Treasury(GameLogger, new TransactionLogStore());
        RunwayManager = new RunwayManager(new RunwayMaintenanceSystem(), GameLogger);
        Shop = new Shop(Treasury, GameLogger);
        FlightScheduler = new FlightScheduler();
        RandomGenerator = new RandomGenerator();
        EventSystem = new EventSystem(RandomGenerator, GameLogger);
        AirportRevenue = new Revenue();
        ModifierManager = new ModifierManager(AirportRevenue, GameLogger);
        ExperienceSystem = new ExperienceSystem(GameLogger);
        AchievementSystem = new AchievementSystem(GameLogger);
        
        // Setup level-up handler
        ExperienceSystem.OnLevelUp += HandleLevelUp;
        
        // Setup achievement handler
        AchievementSystem.OnAchievementUnlocked += HandleAchievementUnlocked;
        
        // We'll set the LandingManager after TickManager is available
        LandingManager = null;
    }

    // Method to set the LandingManager after TickManager is available
    public void SetLandingManager(TickManager tickManager)
    {
        LandingManager = new FlightLandingManager(
            RunwayManager,
            Treasury,
            ModifierManager,
            GameLogger,
            EventSystem,
            RandomGenerator,
            tickManager
        );
        
        // Connect XP award to successful landings
        LandingManager.OnFlightLanded += HandleFlightLanded;
    }

    public void Tick(int currentTick)
    {
        // Accumulate gold at every tick
        Treasury.AccumulateGold();
        
        // Update runway occupation status
        RunwayManager.UpdateRunwaysStatus();
        
        // Track active flights for XP calculation
        UpdateActiveFlights();

        // Process scheduled flights for the current tick
        ProcessScheduledFlights(currentTick);
        
        // Generate new flights if needed
        GenerateFlightsIfNeeded(currentTick);
    }

    private void ProcessScheduledFlights(int currentTick)
    {
        // Process flights scheduled for this tick
        var scheduledFlights = FlightScheduler.GetFlightsAtTick(currentTick);
        
        // Also process any delayed flights that haven't landed yet
        var delayedFlights = FlightScheduler.GetUnlandedFlights()
            .Where(f => f.ScheduledLandingTime < currentTick);

        // Process all flights
        foreach (var flight in scheduledFlights.Concat(delayedFlights))
        {
            LandingManager.ProcessFlight(flight, currentTick);
        }
    }
    
    private void UpdateActiveFlights()
    {
        // Update our list of active flights (for XP calculations)
        activeFlights.Clear();
        activeFlights.AddRange(FlightScheduler.GetUnlandedFlights());
    }
    
    private void HandleFlightLanded(Flight flight, Runway runway, bool isOnTime, int currentTick)
    {
        // Get the current weather from our instance
        Weather weather = new Weather(RandomGenerator);
        
        // Get the runway wear level
        int runwayWear = RunwayManager.GetMaintenanceSystem().GetWearLevel(runway.Name);
        
        // Check if this was a perfect landing (no delays)
        bool perfectLanding = !flight.IsDelayed() && isOnTime;
        
        // Count how many flights are active (for simultaneous flight bonus)
        int simultaneousFlights = Math.Max(0, activeFlights.Count - 1); // Subtract 1 for the current flight
        
        // Calculate XP for this landing
        int xpEarned = ExperienceSystem.CalculateFlightXP(
            flight, 
            weather, 
            runwayWear, 
            isOnTime, 
            perfectLanding, 
            simultaneousFlights
        );
        
        // Add the earned XP to our experience system
        ExperienceSystem.AddExperience(xpEarned);
        
        // Record the flight in the achievement system
        AchievementSystem.RecordFlightLanded(flight);
    } 
    
private void HandleLevelUp(int newLevel)
{
    // Give a gold bonus for leveling up
    double goldBonus = 1000 * newLevel;
    Treasury.AddFunds(goldBonus, $"Level Up Bonus (Level {newLevel})");
    
    // Log the level up with additional information
    GameLogger.Log($"AIRPORT LEVEL UP! Now Level {newLevel}");
    GameLogger.Log($"Level {newLevel} Benefits: Gold Bonus: {goldBonus:C}, Increased Flight Complexity, New Features Unlocked");
    
    // Check for queued shop items to unlock at this level
    Shop.CheckQueuedItemsForLevel(newLevel);
    
    // Unlock new features based on level
    UnlockFeaturesForLevel(newLevel);
}

private void HandleAchievementUnlocked(Achievement achievement)
{
    // Process the achievement
    GameLogger.Log($"Achievement unlocked: {achievement.Name}");
    
    // Check if it's a flight specialization achievement
    if (achievement.Type == AchievementType.FlightTypeSpecialization)
    {
        // Get the next item ID
        int nextItemId = Shop.GetNextItemId();
        
        // Create a buff from the achievement
        var buff = FlightSpecializationBuff.FromAchievement(achievement, nextItemId);
        
        // Add the buff to the shop
        Shop.AddItemToShop(buff);
        
        GameLogger.Log($"New item added to shop: {buff.Name} - {buff.Description} - Price: {buff.Price:C}");
    }
}

private void UnlockFeaturesForLevel(int level)
{
    // Unlock different features based on the airport level
    // No need to add shop items here anymore - they're handled by the queue system
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

    public void ToggleLandingMode()
    {
        LandingManager.ToggleLandingMode();
    }

    private bool IsRandomEventTick(int currentTick)
    {
        // Random events are triggered every 60 ticks (i.e., once per minute)
        return currentTick % 60 == 0;
    }
    
private void GenerateFlightsIfNeeded(int currentTick)
{
    // Process any scheduled flight generation events
    flightEventScheduler.ProcessEvents(currentTick);

    // Only generate new batches every 12 ticks (approximately 10 seconds at 0.8s per tick)
    if (currentTick % 12 == 0)
    {
        // Get our current airport level
        int airportLevel = ExperienceSystem.CurrentLevel;
        
        // Create a flight generator
        FlightGenerator flightGenerator = new FlightGenerator(RandomGenerator);
        
        // Calculate number of flights based on runway count (1.5 * runwayCount)
        int runwayCount = RunwayManager.GetRunwayCount();
        int flightsToGenerate = (int)Math.Ceiling(runwayCount * 2.5);
        flightsToGenerate = Math.Max(1, flightsToGenerate); // Ensure at least 1 flight
        
        GameLogger.Log($"Generating {flightsToGenerate} flights (2.5 × {runwayCount} runways)");
        
        // Stagger the flights over the next several ticks
        for (int i = 0; i < flightsToGenerate; i++)
        {
            // Calculate stagger interval (rounded up)
            int staggerInterval = (int)Math.Ceiling(12.0 / Math.Max(1, flightsToGenerate));
            int staggerTicks = i * staggerInterval;
            int scheduledTick = currentTick + staggerTicks;
            
            // Schedule the flight generation event
            flightEventScheduler.ScheduleEvent(new ScheduledEvent(scheduledTick, (tick) => {
                // Generate a single flight
                Flight flight = flightGenerator.GenerateRandomFlight(tick, airportLevel);
                
                // Schedule the flight
                FlightScheduler.ScheduleFlight(flight, flight.ScheduledLandingTime);
                GameLogger.Log($"Scheduled {flight.FlightNumber} ({flight.Type}, {flight.Priority}) with {flight.Passengers} passengers for tick {flight.ScheduledLandingTime}");
            }));
        }
    }
}
}