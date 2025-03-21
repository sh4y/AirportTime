public static class ConsoleUI
{
    public static void DisplayStatus(Airport airport, int currentTick)
    {
        Console.Clear();
        Console.WriteLine("=== Airport Management ===");
        Console.WriteLine($"Airport: {airport.Name}");
        Console.WriteLine($"Tick: {currentTick}");
        Console.WriteLine($"Gold: {airport.Treasury.GetBalance():C}");
        Console.WriteLine($"Landing Mode: {airport.LandingManager.CurrentLandingMode}");

        Console.WriteLine("\n--- Runways ---");
        airport.RunwayManager.DisplayRunwayInfo();

        Console.WriteLine("\n--- Upcoming Flights ---");
        var upcomingFlights = airport.FlightScheduler.GetUnlandedFlights();
        if (upcomingFlights.Count == 0)
        {
            Console.WriteLine("No flights scheduled.");
        }
        else
        {
            foreach (var flight in upcomingFlights)
            {
                Console.WriteLine($"{flight.ToString()}");
            }
        }

        Console.WriteLine("\n--- Log ---");
        airport.GameLogger.DisplayRecentLogs(5); // Assumes GameLogger has this method
        
        Console.WriteLine("\n--- Controls ---");
        Console.WriteLine("Q:Pause  F:New Flight  S:Shop  R:Repair  M:Toggle Mode  H:Help");
    }
}