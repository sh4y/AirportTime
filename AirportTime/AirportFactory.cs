
namespace AirportTime;

/// <summary>
/// Factory for creating Airport instances with proper dependency injection
/// </summary>
public class AirportFactory 
{
    public static int currentId = 0;
    /// <summary>
    /// Creates a new Airport instance with all dependencies initialized
    /// </summary>
    /// <param name="name">Name of the airport</param>
    /// <param name="startingGold">Initial gold amount</param>
    /// <param name="tickManager">The tick manager to use</param>
    /// <returns>A fully configured Airport instance</returns>
    public static Airport CreateAirport(string name, double startingGold, ITickManager tickManager)
    {
        // Create a dependency container
        var container = new DependencyContainer();
        
        // Register core services
        RegisterCoreServices(container, startingGold, tickManager);
        
        // Create the airport
        return CreateAirportFromContainer(container, name);
    }
    
    /// <summary>
    /// Creates a new Airport instance with view controller setup
    /// </summary>
    /// <param name="name">Name of the airport</param>
    /// <param name="startingGold">Initial gold amount</param>
    /// <param name="tickManager">The tick manager to use</param>
    /// <param name="view">The view implementation to use</param>
    /// <returns>A fully configured Airport instance with view controller</returns>
    public static Airport CreateAirportWithView(string name, double startingGold, ITickManager tickManager, IAirportView view)
    {
        // Create a dependency container
        var container = new DependencyContainer();
        
        // Register core services
        RegisterCoreServices(container, startingGold, tickManager);
        
        // Register the view
        container.Register<IAirportView>(view);
        
        // Create the airport
        var airport = CreateAirportFromContainer(container, name);
        
        // Create and register a view controller
        if (container.IsRegistered<IAirportView>())
        {
            var viewController = new AirportViewController(
                container.Get<IAirportView>(),
                airport
            );
            
            // Set the view controller on the landing manager
            ((IFlightLandingManager)airport.LandingManager).SetViewController(viewController);
        }
        
        return airport;
    }
    
    /// <summary>
    /// Creates an airport with a custom dependency setup
    /// </summary>
    /// <param name="container">Pre-configured dependency container</param>
    /// <param name="name">Airport name</param>
    /// <returns>The Airport instance</returns>
    public static Airport CreateAirportFromContainer(DependencyContainer container, string name)
    {
        // Create and return the airport with injected dependencies
        return new Airport(currentId++,
            "International Airport", container.Get<ITreasury>(),
            container.Get<IRunwayManager>(), container.Get<IShop>(),
            container.Get<IFlightScheduler>(), container.Get<IEventSystem>(),
            container.Get<IGameLogger>(), container.Get<IModifierManager>(),
            container.Get<IExperienceSystem>(), container.Get<IAchievementSystem>(),
            container.Get<IFlightLandingManager>(), container.Get<IFlightGenerationService>(),
            container.Get<IFlightProcessingService>(), container.Get<IRandomGenerator>(),
            container.Get<IFailureTracker>(), container.Get<IEmergencyFlightHandler>());
    }
    
    /// <summary>
    /// Registers core services in the dependency container
    /// </summary>
    /// <param name="container">The container to register services in</param>
    /// <param name="startingGold">Initial gold amount</param>
    /// <param name="tickManager">The tick manager to use</param>
    public static void RegisterCoreServices(DependencyContainer container, double startingGold, ITickManager tickManager)
    {
        // Register the tick manager
        container.Register<ITickManager>(tickManager);
        
        // Create and register core dependencies
        var logger = new GameLogger("GameLogs.db");
        container.Register<IGameLogger>(logger);
        
        var transactionLogStore = new TransactionLogStore();
        var randomGenerator = new RandomGenerator();
        container.Register<IRandomGenerator>(randomGenerator);
        
        // Create and register core systems
        var treasury = new Treasury(logger, transactionLogStore, startingGold);
        container.Register<ITreasury>(treasury);
        
        var runwayMaintenanceSystem = new RunwayMaintenanceSystem();
        var runwayManager = new RunwayManager(runwayMaintenanceSystem, logger);
        container.Register<IRunwayManager>(runwayManager);
        
        var shop = new Shop(treasury, logger);
        container.Register<IShop>(shop);
        
        var flightScheduler = new FlightScheduler();
        container.Register<IFlightScheduler>(flightScheduler);
        
        var eventSystem = new EventSystem(randomGenerator, logger);
        container.Register<IEventSystem>(eventSystem);
        
        var revenue = new Revenue();
        var modifierManager = new ModifierManager(revenue, logger);
        container.Register<IModifierManager>(modifierManager);
        
        var experienceSystem = new ExperienceSystem(logger);
        container.Register<IExperienceSystem>(experienceSystem);
        
        var achievementSystem = new AchievementSystem(logger);
        container.Register<IAchievementSystem>(achievementSystem);
        
        // Create and register failure tracking components
        var failureTracker = new FailureTracker(logger);
        container.Register<IFailureTracker>(failureTracker);
        
        var emergencyFlightHandler = new EmergencyFlightHandler(logger, failureTracker, tickManager);
        container.Register<IEmergencyFlightHandler>(emergencyFlightHandler);
        
        // Create and register flight management systems
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
        container.Register<IFlightLandingManager>(landingManager);
        
        // Create and register business services
        var flightGenerationService = new FlightGenerationService(
            flightGenerator,
            flightScheduler,
            logger,
            experienceSystem,
            runwayManager
        );
        container.Register<IFlightGenerationService>(flightGenerationService);
        
        var flightProcessingService = new FlightProcessingService(
            flightScheduler,
            landingManager
        );
        container.Register<IFlightProcessingService>(flightProcessingService);
    }
    
    /// <summary>
    /// Creates a dependency container with mock services for testing
    /// </summary>
    /// <returns>Configured test container</returns>
    public static DependencyContainer CreateTestContainer()
    {
        var container = new DependencyContainer();
        
        // Register mock services for testing
        // This would be implemented with your preferred mocking framework
        
        return container;
    }
}