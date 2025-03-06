using Microsoft.Extensions.DependencyInjection;
using System;

namespace AirportTime
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create a new service collection
            var services = new ServiceCollection();

            // Register infrastructure services
            services.AddSingleton<Airport>();

            // Register your higher-level simulation service (TestHarness)
            services.AddSingleton<TestHarness>();

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Resolve and run the TestHarness (entry point for simulation)
            var testHarness = serviceProvider.GetRequiredService<TestHarness>();
            testHarness.StartTest();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
