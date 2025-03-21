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
    private Revenue AirportRevenue { get; set; }
    private readonly IRandomGenerator RandomGenerator;
    
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
        
        // Setup level-up handler
        ExperienceSystem.OnLevelUp += HandleLevelUp;
        
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
    
    private void GenerateFlightsIfNeeded(int currentTick)
    {
        // Get our current airport level
        int airportLevel = ExperienceSystem.CurrentLevel;
        
        // Create a flight generator if we don't have one yet
        FlightGenerator flightGenerator = new FlightGenerator(RandomGenerator);
        
        // Check if we should generate new flights based on level and tick
        if (flightGenerator.ShouldGenerateFlights(currentTick, airportLevel))
        {
            // Generate a batch of flights appropriate for our level
            var newFlights = flightGenerator.GenerateFlightBatch(currentTick, airportLevel);
            
            // Schedule each flight
            foreach (var flight in newFlights)
            {
                FlightScheduler.ScheduleFlight(flight, flight.ScheduledLandingTime);
                GameLogger.Log($"Scheduled {flight.FlightNumber} ({flight.Type}, {flight.Priority}) with {flight.Passengers} passengers for tick {flight.ScheduledLandingTime}");
            }
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
    }
    
    private void HandleLevelUp(int newLevel)
    {
        // Give a gold bonus for leveling up
        double goldBonus = 1000 * newLevel;
        Treasury.AddFunds(goldBonus, $"Level Up Bonus (Level {newLevel})");
        
        // Log the level up with additional information
        GameLogger.Log($"AIRPORT LEVEL UP! Now Level {newLevel}");
        GameLogger.Log($"Level {newLevel} Benefits: Gold Bonus: {goldBonus:C}, Increased Flight Complexity, New Features Unlocked");
        
        // Unlock new features based on level
        UnlockFeaturesForLevel(newLevel);
    }
    
    private void UnlockFeaturesForLevel(int level)
    {
        // Unlock different features based on the airport level
        switch (level)
        {
            case 2:
                Shop.AddItemToShop(new MediumRunway("Medium Runway", 5000, "Capable of handling medium aircraft"));
                GameLogger.Log("New shop item unlocked: Medium Runway");
                break;
                
            case 3:
                Shop.AddItemToShop(new LargeRunway("Large Runway", 12000, "Capable of handling large aircraft"));
                GameLogger.Log("New shop item unlocked: Large Runway");
                break;
                
            case 5:
                // Add a revenue modifier for higher airport reputation
                ModifierManager.AddModifier("High Airport Reputation", 1.25);
                GameLogger.Log("Reputation Bonus: All flights now generate 25% more revenue!");
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
}