public static class ConsoleUI
{
    private static readonly string horizontalLine = "───────────────────────────────────────────────────────────────────────────────────────────";
    private static readonly string topBorder = "┌" + horizontalLine + "┐";
    private static readonly string bottomBorder = "└" + horizontalLine + "┘";
    private static readonly string middleBorder = "├" + horizontalLine + "┤";
    private static readonly string sectionBorder = "│                                                                                          │";
    
    public static void DisplayStatus(Airport airport, int currentTick)
    {
        Console.Clear();
        
        // Header section with airport info
        DrawHeader(airport, currentTick);
        
        // XP and level progress
        DrawExperienceSection(airport);
        
        // Runway section
        DrawRunwaySection(airport);
        
        // Upcoming flights section
        DrawFlightSection(airport);
        
        // Log section
        DrawLogSection(airport);
        
        // Control section
        DrawControlSection();
    }
    
    private static void DrawHeader(Airport airport, int currentTick)
    {
        Console.WriteLine(topBorder);
        Console.WriteLine($"│ AIRPORT MANAGER: {airport.Name,-70} │");
        Console.WriteLine($"│ Time: {currentTick,5} ticks     Gold: ${airport.Treasury.GetBalance(),8:N0}     Weather: {GetWeatherInfo(airport),-11} │");
        Console.WriteLine($"│ Landing Mode: {airport.LandingManager.CurrentLandingMode,-68} │");
        Console.WriteLine(middleBorder);
    }
    
    private static string GetWeatherInfo(Airport airport)
    {
        // This is a placeholder - we'd need to expose Weather from Airport
        // For now, return a placeholder
        return "Clear";
    }
    
    private static void DrawExperienceSection(Airport airport)
    {
        int level = airport.ExperienceSystem.CurrentLevel;
        int currentXP = airport.ExperienceSystem.CurrentXP;
        int nextLevelXP = airport.ExperienceSystem.GetRequiredXPForNextLevel();
        int xpNeeded = nextLevelXP - currentXP;
        int progressPercentage = airport.ExperienceSystem.GetLevelProgressPercentage();
        
        Console.WriteLine($"│ LEVEL {level,-80} │");
        
        // Draw progress bar (60 characters wide)
        DrawProgressBar(progressPercentage, 60);
        
        Console.WriteLine($"│ XP: {currentXP}/{nextLevelXP} ({xpNeeded} needed for next level)   •   Airport Efficiency: +{(level * 5)}%          │");
        Console.WriteLine(middleBorder);
    }
    
    private static void DrawProgressBar(int percentage, int width)
    {
        int filledWidth = (int)Math.Round(percentage / 100.0 * width);
        
        Console.Write("│ [");
        
        // Fill progress with different colors based on percentage
        Console.ForegroundColor = GetProgressColor(percentage);
        Console.Write(new string('█', filledWidth));
        Console.ResetColor();
        
        // Empty space
        Console.Write(new string('░', width - filledWidth));
        
        Console.WriteLine($"] {percentage,3}% │");
    }
    
    private static ConsoleColor GetProgressColor(int percentage)
    {
        if (percentage < 30) return ConsoleColor.Red;
        if (percentage < 70) return ConsoleColor.Yellow;
        return ConsoleColor.Green;
    }
    
    private static void DrawRunwaySection(Airport airport)
    {
        Console.WriteLine("│ RUNWAYS                                                                                  │");
        Console.WriteLine("│  ID              | Type            | Length  | Wear  | Status                            │");
        Console.WriteLine("│ -----------------+-----------------+---------+-------+----------------------------------- │");
        
        // Get runways count
        var runwayCount = airport.RunwayManager.GetRunwayCount();
        
        if (runwayCount == 0)
        {
            Console.WriteLine("│  No runways available. Visit the shop to purchase runways.                               │");
            Console.WriteLine("│                                                                                          │");
            Console.WriteLine("│                                                                                          │");
        }
        else
        {
            // Create dummy planes to check runway availability
            var smallPlane = new Plane("DUMMY1", PlaneSize.Small, 20000);
            var availableRunways = airport.RunwayManager.GetAvailableRunways(smallPlane)
                .Select(r => r.Name)
                .ToHashSet();
            
            // Display runways we know exist
            if (runwayCount > 0)
            {
                var runway = "Small Runway";
                int wear = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runway);
                string status = availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED";
                Console.WriteLine($"│  {runway,-15} | {"Small",-15} | {5000,5}m  | {wear,3}%   | {status,-32} │");
            }
            
            if (runwayCount > 1)
            {
                var runway = "Small Runway2";
                int wear = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runway);
                string status = availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED";
                Console.WriteLine($"│  {runway,-15} | {"Small",-15} | {5000,5}m  | {wear,3}%   | {status,-32} │");
            }
            
            if (runwayCount > 2)
            {
                var runway = "Medium Runway";
                int wear = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runway);
                string status = availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED";
                Console.WriteLine($"│  {runway,-15} | {"Medium",-15} | {7500,5}m  | {wear,3}%   | {status,-32} │");
            }
            
            // Fill empty lines if fewer than 3 runways
            for (int i = 0; i < 3 - runwayCount; i++)
            {
                Console.WriteLine("│                                                                                          │");
            }
        }
        
        Console.WriteLine(middleBorder);
    }
    
    private static void DrawFlightSection(Airport airport)
    {
        Console.WriteLine("│ UPCOMING FLIGHTS                                                                         │");
        Console.WriteLine("│  ID      | Type       | Priority   | Size    | Passengers | Scheduled | Status          │");
        Console.WriteLine("│ ---------+------------+------------+---------+------------+-----------+----------------- │");
        
        var upcomingFlights = airport.FlightScheduler.GetUnlandedFlights();
        
        if (upcomingFlights.Count == 0)
        {
            Console.WriteLine("│  No flights scheduled.                                                                    │");
            Console.WriteLine("│                                                                                          │");
            Console.WriteLine("│                                                                                          │");
            Console.WriteLine("│                                                                                          │");
            Console.WriteLine("│                                                                                          │");
        }
        else
        {
            int displayCount = Math.Min(upcomingFlights.Count, 5); // Show max 5 flights
            
            for (int i = 0; i < displayCount; i++)
            {
                var flight = upcomingFlights[i];
                string flightStatus = flight.Status.ToString();
                string statusColor = flightStatus == "Delayed" ? "!" : " ";
                
                Console.WriteLine($"│ {statusColor}{flight.FlightNumber,-8} | {flight.Type,-10} | {flight.Priority,-10} | {flight.Plane.Size,-7} | {flight.Passengers,10} pax | Tick {flight.ScheduledLandingTime,5} | {flightStatus,-15} │");
            }
            
            if (upcomingFlights.Count > displayCount)
            {
                Console.WriteLine($"│  + {upcomingFlights.Count - displayCount} more flights scheduled...                                                             │");
            }
            else
            {
                Console.WriteLine("│                                                                                          │");
            }
            
            // Fill remaining rows if needed
            for (int i = 0; i < 5 - Math.Max(displayCount, 1) - (upcomingFlights.Count > displayCount ? 1 : 0); i++)
            {
                Console.WriteLine("│                                                                                          │");
            }
        }
        
        Console.WriteLine(middleBorder);
    }
    
    private static void DrawLogSection(Airport airport)
    {
        Console.WriteLine("│ RECENT ACTIVITY                                                                          │");
        airport.GameLogger.DisplayRecentLogs(4); // Show last 4 logs
        Console.WriteLine(middleBorder);
    }
    
    private static void DrawControlSection()
    {
        Console.WriteLine("│ CONTROLS                                                                                 │");
        Console.WriteLine("│  Q:Pause/Resume  F:New Flight  S:Shop  R:Repair  M:Toggle Mode  H:Help  X:Exit          │");
        Console.WriteLine(bottomBorder);
    }
}