using System;

namespace AirportTime
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create and configure the tick manager
            var tickManager = new TickManager();
            
            // Create the view
            IAirportView view = new ConsoleUI();
            
            // Create the container (allows for customization if needed)
            var container = new DependencyContainer();
            
            // Register the tick manager and view
            container.Register<ITickManager>(tickManager);
            container.Register<IAirportView>(view);
            
            // Let the factory register other services
            AirportFactory.RegisterCoreServices(container, 2000, tickManager);
            
            // Create the airport
            var airport = AirportFactory.CreateAirportFromContainer(container, "International Airport");
            
            // Create metrics provider
            var metricsProvider = new GameMetricsProvider(airport, container.Get<IGameLogger>());
            container.Register<IGameMetricsProvider>(metricsProvider);
            
            // Create the view controller with proper dependency injection
            var viewController = new AirportViewController(
                container.Get<IAirportView>(),
                airport,
                container.Get<IGameMetricsProvider>()
            );
            container.Register<IAirportViewController>(viewController);
            
            // Set the view controller on the landing manager
            ((IFlightLandingManager)airport.LandingManager).SetViewController(viewController);
            
            // Create input handler (ideally this would also use dependency injection)
            var randomGenerator = container.Get<IRandomGenerator>();
            var flightGenerator = new FlightGenerator(randomGenerator);
            var inputHandler = new InputHandler(airport, flightGenerator, tickManager, airport.GameLogger);
            
            // Setup tick event handler
            tickManager.OnTick += (currentTick) =>
            {
                // Process game logic
                airport.Tick(currentTick);
                
                // Skip display if game is over
                if (!airport.IsGameOver)
                {
                    // Update the view using the controller
                    viewController.UpdateView(currentTick);
                    
                    // Handle input
                    inputHandler.HandleInput(currentTick);
                }
                else
                {
                    // Stop the game when game over is triggered
                    tickManager.Pause();
                    
                    // Wait for key press to exit
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }
                }
            };

            // Start the simulation
            tickManager.SetSpeedMultiplier(1);
            tickManager.Start();

            airport.GameLogger.Log("Simulation started. Press 'Q' to quit.");

            // Keep simulation alive until user quits or game over and exit key pressed
            while (tickManager.IsRunning() || tickManager.IsPaused())
            {
                System.Threading.Thread.Sleep(100);
            }

            airport.GameLogger.Log("Simulation ended. Press any key to exit.");
            Console.ReadKey();
        }
    }
}