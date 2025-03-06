
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public static class ConsoleUI
{
    public static void DisplayStatus(Airport airport, int currentTick)
    {
        Console.Clear();
        Console.WriteLine("=== Airport Management ===");
        Console.WriteLine($"Airport: {airport.Name}");
        Console.WriteLine($"Tick: {currentTick}");
        Console.WriteLine($"Gold: {airport.Treasury.GetBalance():C}");

        Console.WriteLine("\n--- Runways ---");
        airport.RunwayManager.DisplayRunwayInfo();

        Console.WriteLine("\n--- Upcoming Flights ---");
        var upcomingFlights = airport.FlightScheduler.GetFlightsAtTick(currentTick + 1);
        if (upcomingFlights.Count == 0)
        {
            Console.WriteLine("No flights scheduled.");
        }
        else
        {
            foreach (var flight in upcomingFlights)
            {
                Console.WriteLine($"- Flight {flight.FlightNumber} ({flight.Type}), Passengers: {flight.Passengers}");
            }
        }

        Console.WriteLine("\n--- Log ---");
        airport.GameLogger.DisplayRecentLogs(5); // Assumes GameLogger has this method

        Console.WriteLine("\nPress any key to proceed to the next tick...");
        Console.ReadKey();
    }
}
