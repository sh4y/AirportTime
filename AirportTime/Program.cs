using Microsoft.Extensions.DependencyInjection;
using System;

namespace AirportTime
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create core services
            var tickManager = new TickManager();
            
            // Create the airport with its dependencies (including landing manager)
            var airport = AirportFactory.CreateAirport("International Airport", 2000, tickManager);
            
            // Create flight generator for manual flight creation
            var flightGenerator = new FlightGenerator(new RandomGenerator());
            
            // Create input handlers
            var inputHandler = new InputHandler(airport, flightGenerator, tickManager, airport.GameLogger);
            
            // Setup tick event handler
            tickManager.OnTick += (currentTick) =>
            {
                airport.Tick(currentTick);
                ConsoleUI.DisplayStatus(airport, currentTick);
                inputHandler.HandleInput(currentTick);
            };

            // Start the simulation
            tickManager.SetSpeedMultiplier(1);
            tickManager.Start();

            airport.GameLogger.Log("Simulation started. Press 'Q' to quit.");

            // Keep simulation alive until user quits
            while (tickManager.IsRunning() || tickManager.IsPaused())
            {
                System.Threading.Thread.Sleep(100);
            }

            airport.GameLogger.Log("Simulation ended. Press any key to exit.");
            Console.ReadKey();
        }
    }
}