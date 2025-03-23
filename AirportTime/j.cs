namespace AirportTime;

/// <summary>
/// Interface for treasury operations
/// </summary>
public interface ITreasury
{
    /// <summary>
    /// Gets the current balance
    /// </summary>
    /// <returns>Current balance</returns>
    double GetBalance();
    
    /// <summary>
    /// Adds funds to the treasury
    /// </summary>
    /// <param name="amount">Amount to add</param>
    /// <param name="source">Source of funds</param>
    void AddFunds(double amount, string source);
    
    /// <summary>
    /// Deducts funds from the treasury
    /// </summary>
    /// <param name="amount">Amount to deduct</param>
    /// <param name="reason">Reason for deduction</param>
    /// <returns>True if successful, false if insufficient funds</returns>
    bool DeductFunds(double amount, string reason);
    
    /// <summary>
    /// Accumulates gold at the standard rate
    /// </summary>
    void AccumulateGold();
    
    /// <summary>
    /// Gets the gold per tick rate
    /// </summary>
    double GoldPerTick { get; }
}

/// <summary>
/// Interface for runway management
/// </summary>
public interface IRunwayManager : IRunwayProvider
{
    /// <summary>
    /// Gets the maintenance system
    /// </summary>
    /// <returns>Runway maintenance system</returns>
    RunwayMaintenanceSystem GetMaintenanceSystem();

    void FreeMaintenance(RunwayBuff req);
    
    /// <summary>
    /// Gets the count of available runways
    /// </summary>
    /// <returns>Number of runways</returns>
    int GetRunwayCount();
    
    /// <summary>
    /// Adds weather resistance to runways
    /// </summary>
    /// <param name="resistance">Resistance value (0.0-1.0)</param>
    void AddWeatherResistance(double resistance);
    
    /// <summary>
    /// Reduces landing duration for all runways
    /// </summary>
    /// <param name="reductionFactor">Reduction factor (0.0-1.0)</param>
    void ReduceAllRunwayLandingDurations(double reductionFactor);
    
    /// <summary>
    /// Unlocks a runway
    /// </summary>
    /// <param name="runway">The runway to unlock</param>
    void UnlockRunway(Runway runway);
    
    /// <summary>
    /// Checks if there's a runway with the specified length
    /// </summary>
    /// <param name="requiredLength">Required length</param>
    /// <returns>True if a suitable runway exists</returns>
    bool HasRunwayOfLength(int requiredLength);
    
    /// <summary>
    /// Checks if a plane can land
    /// </summary>
    /// <param name="plane">The plane to check</param>
    /// <returns>True if the plane can land on any runway</returns>
    bool CanLand(Plane plane);
    
    /// <summary>
    /// Gets an available runway for a plane
    /// </summary>
    /// <param name="plane">The plane needing a runway</param>
    /// <returns>Suitable runway or null</returns>
    Runway GetAvailableRunway(Plane plane);
    
    /// <summary>
    /// Gets all available runways for a plane
    /// </summary>
    /// <param name="plane">The plane needing a runway</param>
    /// <returns>List of suitable runways</returns>
    List<Runway> GetAvailableRunways(Plane plane);
    
    /// <summary>
    /// Handles landing on a runway
    /// </summary>
    /// <param name="runwayID">Runway ID</param>
    /// <param name="weather">Current weather</param>
    /// <param name="trafficVolume">Traffic volume</param>
    /// <param name="flightNumber">Flight number</param>
    /// <returns>True if landing was successful</returns>
    bool HandleLanding(string runwayID, Weather weather, int trafficVolume, string flightNumber = null);
    
    /// <summary>
    /// Updates status of all runways
    /// </summary>
    void UpdateRunwaysStatus();
    
    /// <summary>
    /// Gets a runway by name
    /// </summary>
    /// <param name="runwayName">Runway name</param>
    /// <returns>Runway or null</returns>
    Runway GetRunwayByName(string runwayName);
    
    /// <summary>
    /// Performs maintenance on all runways
    /// </summary>
    /// <param name="treasury">Treasury for funding repairs</param>
    void PerformMaintenance(ITreasury treasury);
    
    /// <summary>
    /// Reduces maintenance time for all runways
    /// </summary>
    /// <param name="reductionFactor">Reduction factor (0.0-1.0)</param>
    void ReduceMaintenanceTime(double reductionFactor);
}

/// <summary>
/// Interface for flight scheduling
/// </summary>
public interface IFlightScheduler
{
    /// <summary>
    /// Schedules a flight
    /// </summary>
    /// <param name="flight">The flight to schedule</param>
    /// <param name="scheduledTick">Tick when the flight is scheduled</param>
    void ScheduleFlight(Flight flight, int scheduledTick);
    
    /// <summary>
    /// Gets flights scheduled for a specific tick
    /// </summary>
    /// <param name="tick">The tick to check</param>
    /// <returns>List of flights at that tick</returns>
    List<Flight> GetFlightsAtTick(int tick);
    
    /// <summary>
    /// Gets all flights that haven't landed yet
    /// </summary>
    /// <returns>List of unlanded flights</returns>
    List<Flight> GetUnlandedFlights();
}

/// <summary>
/// Interface for event system
/// </summary>
public interface IEventSystem
{
    /// <summary>
    /// Registers an event
    /// </summary>
    /// <param name="gameEvent">The event to register</param>
    void RegisterEvent(IEvent gameEvent);
    
    /// <summary>
    /// Triggers a random event
    /// </summary>
    /// <param name="flight">The flight affected</param>
    void TriggerRandomEvent(Flight flight);
    
    /// <summary>
    /// Triggers a delay event
    /// </summary>
    /// <param name="flight">The flight to delay</param>
    /// <param name="delayTicks">Number of ticks to delay</param>
    /// <param name="reason">Reason for delay</param>
    /// <param name="currentTick">Current game tick</param>
    void TriggerDelayEvent(Flight flight, int delayTicks, string reason, int currentTick);
}

/// <summary>
/// Interface for modifier management
/// </summary>
public interface IModifierManager
{
    /// <summary>
    /// Adds a modifier
    /// </summary>
    /// <param name="name">Modifier name</param>
    /// <param name="value">Modifier value</param>
    void AddModifier(string name, double value);
    
    /// <summary>
    /// Adds a flight type-specific modifier
    /// </summary>
    /// <param name="flightType">Flight type</param>
    /// <param name="value">Modifier value</param>
    /// <param name="name">Modifier name</param>
    void AddFlightTypeModifier(FlightType flightType, double value, string name);
    
    /// <summary>
    /// Removes a modifier
    /// </summary>
    /// <param name="name">Modifier name</param>
    void RemoveModifier(string name);
    
    /// <summary>
    /// Applies modifiers to a value
    /// </summary>
    /// <param name="baseValue">Base value</param>
    /// <returns>Modified value</returns>
    double ApplyModifiers(double baseValue);
    
    /// <summary>
    /// Applies flight type-specific modifiers
    /// </summary>
    /// <param name="baseValue">Base value</param>
    /// <param name="flightType">Flight type</param>
    /// <returns>Modified value</returns>
    double ApplyFlightTypeModifiers(double baseValue, FlightType flightType);
    
    /// <summary>
    /// Calculates revenue for a flight
    /// </summary>
    /// <param name="flight">The flight</param>
    /// <param name="currentTick">Current game tick</param>
    /// <returns>Calculated revenue</returns>
    double CalculateRevenue(Flight flight, int currentTick);
    
    /// <summary>
    /// Gets all active modifiers
    /// </summary>
    /// <returns>List of modifiers</returns>
    List<Modifier> GetAllModifiers();
}

/// <summary>
/// Interface for experience system
/// </summary>
public interface IExperienceSystem
{
    /// <summary>
    /// Current experience level
    /// </summary>
    int CurrentLevel { get; }
    
    /// <summary>
    /// Current XP amount
    /// </summary>
    int CurrentXP { get; }
    
    /// <summary>
    /// Event triggered on level up
    /// </summary>
    event Action<int> OnLevelUp;
    
    /// <summary>
    /// Adds experience points
    /// </summary>
    /// <param name="amount">Amount to add</param>
    void AddExperience(int amount);
    
    /// <summary>
    /// Gets required XP for next level
    /// </summary>
    /// <returns>XP required</returns>
    int GetRequiredXPForNextLevel();
    
    /// <summary>
    /// Gets required XP for a specific level
    /// </summary>
    /// <param name="level">The level</param>
    /// <returns>XP required</returns>
    int GetRequiredXPForLevel(int level);
    
    /// <summary>
    /// Adds an XP multiplier
    /// </summary>
    /// <param name="multiplier">Multiplier value</param>
    void AddXPMultiplier(double multiplier);
    
    /// <summary>
    /// Calculates XP for a flight landing
    /// </summary>
    /// <param name="flight">The flight</param>
    /// <param name="weather">Current weather</param>
    /// <param name="runwayWear">Runway wear level</param>
    /// <param name="onTime">Whether landing was on time</param>
    /// <param name="perfectLanding">Whether it was a perfect landing</param>
    /// <param name="simultaneousFlights">Number of simultaneous flights</param>
    /// <returns>Calculated XP</returns>
    int CalculateFlightXP(Flight flight, Weather weather, int runwayWear, bool onTime, bool perfectLanding, int simultaneousFlights);
    
    /// <summary>
    /// Gets progress percentage to next level
    /// </summary>
    /// <returns>Progress percentage (0-100)</returns>
    int GetLevelProgressPercentage();
    
    /// <summary>
    /// Gets a string representation of XP status
    /// </summary>
    /// <returns>Status string</returns>
    string GetXPStatusString();
}

/// <summary>
/// Interface for achievement system
/// </summary>
public interface IAchievementSystem
{
    /// <summary>
    /// Event triggered when an achievement is unlocked
    /// </summary>
    event Action<Achievement> OnAchievementUnlocked;
    
    /// <summary>
    /// Records a flight landing
    /// </summary>
    /// <param name="flight">The flight that landed</param>
    /// <param name="isPerfectLanding">Whether it was a perfect landing</param>
    /// <param name="runwayWear">Runway wear level</param>
    /// <param name="currentWeather">Current weather</param>
    /// <param name="isNightTime">Whether it's night time</param>
    /// <param name="simultaneousFlights">Number of simultaneous flights</param>
    void RecordFlightLanded(Flight flight, bool isPerfectLanding, int runwayWear, WeatherType currentWeather, bool isNightTime, int simultaneousFlights);
    
    /// <summary>
    /// Resets consecutive flights counter
    /// </summary>
    void ResetConsecutiveFlights();
    
    /// <summary>
    /// Gets flight count by type
    /// </summary>
    /// <param name="flightType">Flight type</param>
    /// <returns>Number of flights</returns>
    int GetFlightTypeCount(FlightType flightType);
    
    /// <summary>
    /// Gets all unlocked achievements
    /// </summary>
    /// <returns>List of unlocked achievements</returns>
    List<Achievement> GetUnlockedAchievements();
    
    /// <summary>
    /// Gets all achievements with status
    /// </summary>
    /// <returns>List of achievement status</returns>
    List<AchievementStatus> GetAllAchievements();
}

/// <summary>
/// Interface for flight landing management
/// </summary>
public interface IFlightLandingManager
{
    /// <summary>
    /// Current landing mode
    /// </summary>
    FlightLandingManager.LandingMode CurrentLandingMode { get; set; }
    
    /// <summary>
    /// Event triggered when a flight lands
    /// </summary>
    event Action<Flight, Runway, bool, int> OnFlightLanded;
    
    /// <summary>
    /// Sets the view controller for UI interactions
    /// </summary>
    /// <param name="controller">The view controller</param>
    void SetViewController(AirportViewController controller);
    
    /// <summary>
    /// Processes a flight landing
    /// </summary>
    /// <param name="flight">The flight to process</param>
    /// <param name="currentTick">Current game tick</param>
    /// <returns>True if landing was successful</returns>
    bool ProcessFlight(Flight flight, int currentTick);
    
    /// <summary>
    /// Toggles between automatic and manual landing modes
    /// </summary>
    void ToggleLandingMode();
}

/// <summary>
/// Interface for flight generation service
/// </summary>
public interface IFlightGenerationService
{
    /// <summary>
    /// Generates flights if needed
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    void GenerateFlightsIfNeeded(int currentTick);
}

/// <summary>
/// Interface for flight processing service
/// </summary>
public interface IFlightProcessingService
{
    /// <summary>
    /// Processes scheduled flights
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    void ProcessScheduledFlights(int currentTick);
    
    /// <summary>
    /// Updates active flights list
    /// </summary>
    void UpdateActiveFlights();
    
    /// <summary>
    /// Gets number of active flights
    /// </summary>
    /// <returns>Count of active flights</returns>
    int GetSimultaneousFlightCount();
    
    /// <summary>
    /// Gets all active flights
    /// </summary>
    /// <returns>List of active flights</returns>
    List<Flight> GetActiveFlights();
}

/// <summary>
/// Interface for failure tracking
/// </summary>
public interface IFailureTracker
{
    /// <summary>
    /// Event triggered on game over
    /// </summary>
    event Action<FailureType> OnGameOver;
    
    /// <summary>
    /// Records a failure
    /// </summary>
    /// <param name="failureType">Type of failure</param>
    /// <param name="details">Failure details</param>
    void RecordFailure(FailureType failureType, string details);
    
    /// <summary>
    /// Gets failure count by type
    /// </summary>
    /// <param name="failureType">Type of failure</param>
    /// <returns>Failure count</returns>
    int GetFailureCount(FailureType failureType);
    
    /// <summary>
    /// Gets failure threshold by type
    /// </summary>
    /// <param name="failureType">Type of failure</param>
    /// <returns>Failure threshold</returns>
    int GetFailureThreshold(FailureType failureType);
    
    /// <summary>
    /// Gets all failure counts
    /// </summary>
    /// <returns>Dictionary of failure counts</returns>
    Dictionary<FailureType, int> GetAllFailureCounts();
    
    /// <summary>
    /// Gets percentage to game over
    /// </summary>
    /// <param name="failureType">Type of failure</param>
    /// <returns>Percentage (0-100)</returns>
    int GetFailurePercentage(FailureType failureType);
}

/// <summary>
/// Interface for emergency flight handling
/// </summary>
public interface IEmergencyFlightHandler
{
    /// <summary>
    /// Registers an emergency flight
    /// </summary>
    /// <param name="flight">The emergency flight</param>
    /// <param name="currentTick">Current game tick</param>
    void RegisterEmergencyFlight(Flight flight, int currentTick);
    
    /// <summary>
    /// Marks an emergency as handled
    /// </summary>
    /// <param name="flightNumber">Flight number</param>
    void MarkEmergencyHandled(string flightNumber);
    
    /// <summary>
    /// Processes active emergencies
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    void ProcessEmergencies(int currentTick);
    
    /// <summary>
    /// Gets count of active emergencies
    /// </summary>
    /// <returns>Emergency count</returns>
    int GetActiveEmergencyCount();
    
    /// <summary>
    /// Gets details of active emergencies
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    /// <returns>List of emergency details</returns>
    List<(string flightNumber, int ticksRemaining, Flight flight)> GetActiveEmergencies(int currentTick);
    
    /// <summary>
    /// Checks if a flight is an active emergency
    /// </summary>
    /// <param name="flightNumber">Flight number</param>
    /// <returns>True if it's an active emergency</returns>
    bool IsActiveEmergency(string flightNumber);
}

/// <summary>
/// Interface for shop functionality
/// </summary>
public interface IShop
{
    /// <summary>
    /// Adds an item to the shop
    /// </summary>
    /// <param name="item">Item to add</param>
    void AddItemToShop(IPurchasable item);
    
    /// <summary>
    /// Queues an item for unlocking at a specific level
    /// </summary>
    /// <param name="item">Item to queue</param>
    /// <param name="requiredLevel">Required level</param>
    void QueueItemForLevel(IPurchasable item, int requiredLevel);
    
    /// <summary>
    /// Checks for queued items to unlock
    /// </summary>
    /// <param name="currentLevel">Current level</param>
    void CheckQueuedItemsForLevel(int currentLevel);
    
    /// <summary>
    /// Initializes achievement handling
    /// </summary>
    /// <param name="airport">The airport</param>
    void InitializeAchievementHandling(Airport airport);
    
    /// <summary>
    /// Gets the next item ID
    /// </summary>
    /// <returns>Next item ID</returns>
    int GetNextItemId();
    
    /// <summary>
    /// Displays items for sale
    /// </summary>
    void ViewItemsForSale();
    
    /// <summary>
    /// Buys an item by ID
    /// </summary>
    /// <param name="itemId">Item ID</param>
    /// <param name="airport">The airport</param>
    /// <returns>Purchase result</returns>
    PurchaseResult BuyItem(int itemId, Airport airport);
    
    /// <summary>
    /// Buys an item by name
    /// </summary>
    /// <param name="itemName">Item name</param>
    /// <param name="airport">The airport</param>
    /// <returns>Purchase result</returns>
    PurchaseResult BuyItem(string itemName, Airport airport);
    
    /// <summary>
    /// Buys a runway by tier
    /// </summary>
    /// <param name="tier">Runway tier</param>
    /// <param name="airport">The airport</param>
    /// <returns>Purchase result</returns>
    PurchaseResult BuyItem(RunwayTier tier, Airport airport);
}

/// <summary>
/// Interface for tick management
/// </summary>
public interface ITickManager
{
    /// <summary>
    /// Current tick
    /// </summary>
    int CurrentTick { get; }
    
    /// <summary>
    /// Event triggered on tick
    /// </summary>
    event Action<int> OnTick;
    
    /// <summary>
    /// Starts the tick timer
    /// </summary>
    void Start();
    
    /// <summary>
    /// Pauses the tick timer
    /// </summary>
    void Pause();
    
    /// <summary>
    /// Sets the speed multiplier
    /// </summary>
    /// <param name="multiplier">Speed multiplier</param>
    void SetSpeedMultiplier(double multiplier);
    
    /// <summary>
    /// Checks if timer is running
    /// </summary>
    /// <returns>True if running</returns>
    bool IsRunning();
    
    /// <summary>
    /// Checks if timer is paused
    /// </summary>
    /// <returns>True if paused</returns>
    bool IsPaused();
}

/// <summary>
/// Interface for providing game metrics
/// </summary>
public interface IGameMetricsProvider
{
    /// <summary>
    /// Gets airport metrics
    /// </summary>
    /// <returns>Airport metrics</returns>
    AirportMetrics GetAirportMetrics();
    
    /// <summary>
    /// Gets emergency metrics
    /// </summary>
    /// <returns>Emergency metrics</returns>
    EmergencyMetrics GetEmergencyMetrics();
    
    /// <summary>
    /// Gets experience metrics
    /// </summary>
    /// <returns>Experience metrics</returns>
    ExperienceMetrics GetExperienceMetrics();
    
    /// <summary>
    /// Gets failure metrics
    /// </summary>
    /// <returns>Failure metrics</returns>
    FailureMetrics GetFailureMetrics();
    
    /// <summary>
    /// Gets runway metrics
    /// </summary>
    /// <returns>Runway metrics</returns>
    RunwayMetrics GetRunwayMetrics();
    
    /// <summary>
    /// Gets flight metrics
    /// </summary>
    /// <returns>Flight metrics</returns>
    FlightMetrics GetFlightMetrics();
    
    /// <summary>
    /// Gets modifier metrics
    /// </summary>
    /// <returns>Modifier metrics</returns>
    ModifierMetrics GetModifierMetrics();
    
    /// <summary>
    /// Gets achievement metrics
    /// </summary>
    /// <returns>Achievement metrics</returns>
    AchievementMetrics GetAchievementMetrics();
    
    /// <summary>
    /// Gets log metrics
    /// </summary>
    /// <returns>Log metrics</returns>
    LogMetrics GetLogMetrics();
    
    /// <summary>
    /// Gets game time information
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    /// <returns>Game time info</returns>
    GameTimeInfo GetTimeInfo(int currentTick);
}