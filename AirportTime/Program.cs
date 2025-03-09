using Microsoft.Extensions.DependencyInjection;
using System;

namespace AirportTime
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            // Infrastructure & core services
            services.AddSingleton<IRandomGenerator, RandomGenerator>();
            // Here, we supply the parameter "GameLogs.db" to GameLogger's constructor
            services.AddSingleton<GameLogger>(provider => 
                new GameLogger("GameLogs.db"));        
            services.AddSingleton<Weather>();
            services.AddSingleton<RunwayMaintenanceSystem>();
            services.AddSingleton<TickManager>();

            // Airport-related services
            services.AddSingleton<Treasury>(sp => 
                new Treasury(sp.GetRequiredService<GameLogger>(), new TransactionLogStore()));
            services.AddSingleton<RunwayManager>();
            services.AddSingleton<FlightScheduler>();
            services.AddSingleton<FlightGenerator>();
            services.AddSingleton<Shop>(); // Items initialized internally
            services.AddSingleton<EventSystem>();
            services.AddSingleton<ModifierManager>();

            // Single airport instance
            services.AddSingleton<Airport>(p => new Airport("test",90) );

            // Input Handler
            services.AddSingleton<InputHandler>();

            var serviceProvider = services.BuildServiceProvider();

            // Resolve services once before the loop
            var airport = serviceProvider.GetRequiredService<Airport>();
            var tickManager = serviceProvider.GetRequiredService<TickManager>();
            var inputHandler = serviceProvider.GetRequiredService<InputHandler>();
            var logger = serviceProvider.GetRequiredService<GameLogger>();
            var weather = serviceProvider.GetRequiredService<Weather>();
            var flightGen = serviceProvider.GetRequiredService<FlightGenerator>();

            tickManager.OnTick += (currentTick) =>
            {

                airport.Tick(currentTick);

                ConsoleUI.DisplayStatus(airport, currentTick);

                inputHandler.HandleInput(currentTick);
            };

            tickManager.SetSpeedMultiplier(1);
            tickManager.Start();

            logger.Log("Simulation started. Press 'Q' to quit.");

            // Keep simulation alive until user quits
            while (tickManager.IsRunning() || tickManager.IsPaused())
            {
                System.Threading.Thread.Sleep(100);
            }

            logger.Log("Simulation ended. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
