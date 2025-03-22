public class AirportFactory
{
    /// <summary>
    /// Creates a new Airport instance with all dependencies initialized
    /// </summary>
    /// <param name="name">Name of the airport</param>
    /// <param name="startingGold">Initial gold amount</param>
    /// <param name="tickManager">The tick manager to use</param>
    /// <returns>A fully configured Airport instance</returns>
    public static Airport CreateAirport(string name, double startingGold, TickManager tickManager)
    {
        // Create core dependencies
        var logger = new GameLogger("GameLogs.db");
        var transactionLogStore = new TransactionLogStore();
        var randomGenerator = new RandomGenerator();
        var weather = new Weather(randomGenerator);
        
        // Create core systems
        var treasury = new Treasury(logger, transactionLogStore, startingGold);
        var runwayMaintenanceSystem = new RunwayMaintenanceSystem();
        var runwayManager = new RunwayManager(runwayMaintenanceSystem, logger);
        var shop = new Shop(treasury, logger);
        var flightScheduler = new FlightScheduler();
        var eventSystem = new EventSystem(randomGenerator, logger);
        var revenue = new Revenue();
        var modifierManager = new ModifierManager(revenue, logger);
        var experienceSystem = new ExperienceSystem(logger);
        var achievementSystem = new AchievementSystem(logger);
        
        // Create new failure tracking components
        var failureTracker = new FailureTracker(logger);
        var emergencyFlightHandler = new EmergencyFlightHandler(logger, failureTracker, tickManager);
        
        // Create flight management systems
        var flightGenerator = new FlightGenerator(randomGenerator);
        
        var landingManager = new FlightLandingManager(
            runwayManager,
            treasury,
            modifierManager,
            logger,
            eventSystem,
            randomGenerator,
            tickManager,
            emergencyFlightHandler
        );
        
        // Create business services
        var flightGenerationService = new FlightGenerationService(
            flightGenerator,
            flightScheduler,
            logger,
            experienceSystem,
            runwayManager
        );
        
        var flightProcessingService = new FlightProcessingService(
            flightScheduler,
            landingManager
        );
        
        // Create and configure the airport
        var airport = new Airport(
            name,
            treasury,
            runwayManager,
            shop,
            flightScheduler,
            eventSystem,
            logger,
            modifierManager,
            experienceSystem,
            achievementSystem,
            landingManager,
            flightGenerationService,
            flightProcessingService,
            randomGenerator,
            failureTracker,
            emergencyFlightHandler
        );
        
        return airport;
    }
    
    /// <summary>
    /// Creates a new Airport instance with view controller setup
    /// </summary>
    /// <param name="name">Name of the airport</param>
    /// <param name="startingGold">Initial gold amount</param>
    /// <param name="tickManager">The tick manager to use</param>
    /// <param name="view">The view implementation to use</param>
    /// <returns>A fully configured Airport instance with view controller</returns>
    public static Airport CreateAirportWithView(string name, double startingGold, TickManager tickManager, IAirportView view)
    {
        // Create the airport using the existing method
        var airport = CreateAirport(name, startingGold, tickManager);
        
        // Create a view controller and configure the landing manager to use it
        var viewController = new AirportViewController(view, airport);
        airport.LandingManager.SetViewController(viewController);
        
        return airport;
    }
}