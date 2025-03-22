public class AirportFactory
{
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
        
        // Create flight management systems
        var flightGenerator = new FlightGenerator(randomGenerator);
        
        var landingManager = new FlightLandingManager(
            runwayManager,
            treasury,
            modifierManager,
            logger,
            eventSystem,
            randomGenerator,
            tickManager
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
            randomGenerator
        );
        
        return airport;
    }
}