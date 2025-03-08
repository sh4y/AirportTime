using Microsoft.Extensions.DependencyInjection;
using System;

namespace AirportTime
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            // Core single-instance services
            services.AddSingleton<IRandomGenerator, RandomGenerator>();
            services.AddSingleton<GameLogger>();
            services.AddSingleton<Weather>();
            services.AddSingleton<RunwayMaintenanceSystem>();
            services.AddSingleton<TickManager>();

            // Airport-related services
            services.AddSingleton<Treasury>(); // Manageable starting gold
            services.AddSingleton<RunwayManager>();
            services.AddSingleton<FlightScheduler>();
            services.AddSingleton<Shop>();
            services.AddSingleton<EventSystem>();
            services.AddSingleton<ModifierManager>();

            // Single Airport instance for testing
            services.AddSingleton<Airport>(p => new Airport("test", 90));

            var serviceProvider = services.BuildServiceProvider();

            // Setup single airport and components
            var airport = serviceProvider.GetRequiredService<Airport>();

            var tickManager = serviceProvider.GetRequiredService<TickManager>();
            var weather = serviceProvider.GetRequiredService<Weather>();
            var fs = new FlightGenerator(serviceProvider.GetRequiredService<IRandomGenerator>());

            // Simple tick loop for manageable testing
            tickManager.OnTick += (currentTick) =>
            {
            // Airport processing (flights, runway maintenance, gold)
                airport.Tick(currentTick);

                // Basic console output for clarity
                Console.Clear();
                ConsoleUI.DisplayStatus(airport, currentTick);
                
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Q)
                        tickManager.Pause();

                    else if (key == ConsoleKey.F)
                    {
                        var f = fs.GenerateRandomFlight(currentTick, 10);
                        airport.FlightScheduler.ScheduleFlight(f, currentTick+100);
                    }
                }

            };
            // Properly integrated ConsoleUI call:

            tickManager.SetSpeedMultiplier(1);
            tickManager.Start();

            Console.WriteLine("Game running... Press 'Q' to quit at any time.");

            // Wait until simulation paused by user
            while (tickManager.IsRunning())
            {
                System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine("Simulation ended. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
