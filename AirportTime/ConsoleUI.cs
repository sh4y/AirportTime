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
        
        // Financial section
        DrawFinancialSection(airport);
        
        // Runway section
        DrawRunwaySection(airport);
        
        // Upcoming flights section
        DrawFlightSection(airport);
        
        // Active Modifiers section
        DrawModifiersSection(airport);
        
        // Log section
        DrawLogSection(airport);
        
        // Control section
        DrawControlSection();
    }
    
    private static void DrawHeader(Airport airport, int currentTick)
    {
        // Calculate game time (10 minutes per tick as an example)
        int gameDays = currentTick / (24 * 60 / 10); // 144 ticks per day if 10 min per tick
        int gameHours = (currentTick % (24 * 60 / 10)) / (60 / 10);
        int gameMinutes = (currentTick % (60 / 10)) * 10;
        string gameTime = $"{gameDays}d {gameHours:D2}:{gameMinutes:D2}";
        
        Console.WriteLine(topBorder);
        Console.WriteLine($"│ AIRPORT MANAGER: {airport.Name,-60} Day {gameDays,3} {gameHours:D2}:{gameMinutes:D2} │");
        Console.WriteLine($"│ Time: {currentTick,5} ticks     Gold: ${airport.Treasury.GetBalance(),8:N0}     Weather: {GetWeatherInfo(airport),-11} │");
        Console.WriteLine($"│ Landing Mode: {airport.LandingManager.CurrentLandingMode,-25} Active Flights: {GetActiveFlightCount(airport),3}              │");
        Console.WriteLine(middleBorder);
    }
    
    private static int GetActiveFlightCount(Airport airport)
    {
        return airport.FlightScheduler.GetUnlandedFlights().Count;
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
        
        // Calculate estimated time to next level (assuming 40 XP per flight on average, 1 flight per 12 ticks)
        int flightsNeeded = (int)Math.Ceiling(xpNeeded / 40.0);
        int estimatedTicks = flightsNeeded * 12;
        string timeEstimate = estimatedTicks > 1000 ? "a long time" : $"~{estimatedTicks} ticks";
        
        Console.WriteLine($"│ LEVEL {level,-10} Next Level: {timeEstimate,-20} Efficiency Bonus: +{(level * 5)}%           │");
        
        // Draw progress bar (60 characters wide)
        DrawProgressBar(progressPercentage, 60);
        
        Console.WriteLine($"│ XP: {currentXP}/{nextLevelXP} ({xpNeeded} needed for next level)   •   Unlocks: {GetNextUnlock(level),-22} │");
        Console.WriteLine(middleBorder);
    }
    
    private static string GetNextUnlock(int currentLevel)
    {
        return currentLevel switch
        {
            1 => "Medium Runway (Level 2)",
            2 => "Runway Speed Upgrade (L3)",
            3 => "Income Boost (Level 4)",
            4 => "Large Runway (Level 5)",
            _ => "Various Upgrades"
        };
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
    
    private static void DrawFinancialSection(Airport airport)
    {
        // Calculate income (gold per tick * 60 ticks for "per minute" rate)
        double incomePerMin = airport.Treasury.GoldPerTick * 60;
        
        // Get runway maintenance costs (rough estimate based on wear)
        double maintenanceCosts = 0;
        for (int i = 0; i < airport.RunwayManager.GetRunwayCount(); i++)
        {
            string runwayName = i == 0 ? "Small Runway" : (i == 1 ? "Small Runway2" : "Medium Runway");
            int wear = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runwayName);
            maintenanceCosts += wear * 11; // Based on the maintenance cost calculation
        }
        
        double profit = incomePerMin - maintenanceCosts;
        
        Console.WriteLine("│ FINANCIAL SUMMARY                                                                        │");
        Console.WriteLine($"│  Income Rate: ${incomePerMin,8:N0}/min    Maintenance Costs: ${maintenanceCosts,8:N0}    Profit: ${profit,8:N0}/min  │");
        Console.WriteLine(middleBorder);
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
                
                // Check if GetRunwayByName exists, otherwise fallback to basic status
                string status;
                try {
                    var runwayObj = airport.RunwayManager.GetRunwayByName(runway);
                    status = runwayObj != null 
                        ? runwayObj.GetDetailedOccupationStatus() 
                        : (availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED");
                } catch {
                    status = availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED";
                }
                
                string wearIndicator = wear >= 80 ? "⚠️" : (wear >= 50 ? "!" : " ");
                Console.WriteLine($"│  {runway,-15} | {"Small",-15} | {5000,5}m  | {wearIndicator}{wear,2}%   | {status,-32} │");
            }
            
            if (runwayCount > 1)
            {
                var runway = "Small Runway2";
                int wear = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runway);
                
                // Check if GetRunwayByName exists, otherwise fallback to basic status
                string status;
                try {
                    var runwayObj = airport.RunwayManager.GetRunwayByName(runway);
                    status = runwayObj != null 
                        ? runwayObj.GetDetailedOccupationStatus() 
                        : (availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED");
                } catch {
                    status = availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED";
                }
                
                string wearIndicator = wear >= 80 ? "⚠️" : (wear >= 50 ? "!" : " ");
                Console.WriteLine($"│  {runway,-15} | {"Small",-15} | {5000,5}m  | {wearIndicator}{wear,2}%   | {status,-32} │");
            }
            
            if (runwayCount > 2)
            {
                var runway = "Medium Runway";
                int wear = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runway);
                
                // Check if GetRunwayByName exists, otherwise fallback to basic status
                string status;
                try {
                    var runwayObj = airport.RunwayManager.GetRunwayByName(runway);
                    status = runwayObj != null 
                        ? runwayObj.GetDetailedOccupationStatus() 
                        : (availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED");
                } catch {
                    status = availableRunways.Contains(runway) ? "AVAILABLE" : "OCCUPIED";
                }
                
                string wearIndicator = wear >= 80 ? "⚠️" : (wear >= 50 ? "!" : " ");
                Console.WriteLine($"│  {runway,-15} | {"Medium",-15} | {7500,5}m  | {wearIndicator}{wear,2}%   | {status,-32} │");
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
        Console.WriteLine("│  ID      | Type       | Priority   | Size    | Passengers | Scheduled | Revenue  | Status │");
        Console.WriteLine("│ ---------+------------+------------+---------+------------+-----------+----------+------- │");
        
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
                
                // Calculate estimated revenue (this is a rough estimate)
                double baseRevenue = flight.Passengers * 10;
                double estRevenue = flight.Type switch
                {
                    FlightType.Commercial => baseRevenue,
                    FlightType.Cargo => baseRevenue * 0.75,
                    FlightType.VIP => baseRevenue * 2.0,
                    FlightType.Emergency => baseRevenue * 1.5,
                    _ => baseRevenue
                };
                
                Console.WriteLine($"│ {statusColor}{flight.FlightNumber,-8} | {flight.Type,-10} | {flight.Priority,-10} | {flight.Plane.Size,-7} | {flight.Passengers,8} pax | T{flight.ScheduledLandingTime,6} | ${estRevenue,7:N0} | {flightStatus} │");
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
    
    private static void DrawModifiersSection(Airport airport)
    {
        Console.WriteLine("│ ACTIVE MODIFIERS                                                                         │");
        
        // Try to access active modifiers via reflection (if available)
        var modifiers = GetActiveModifiers(airport);
        
        if (modifiers.Count == 0)
        {
            Console.WriteLine("│  No active modifiers.                                                                     │");
        }
        else
        {
            foreach (var modifier in modifiers)
            {
                string effectType = modifier.Value > 1.0 ? "bonus" : "penalty";
                double percentage = Math.Abs(modifier.Value - 1.0) * 100;
                
                Console.WriteLine($"│  {modifier.Name,-30} | {percentage,5:F1}% {effectType,-7} | {GetModifierDescription(modifier.Name),-22} │");
            }
            
            // Pad with empty lines if fewer than 2 modifiers
            for (int i = 0; i < 2 - modifiers.Count; i++)
            {
                Console.WriteLine("│                                                                                          │");
            }
        }
        
        Console.WriteLine(middleBorder);
    }
    
    private static List<(string Name, double Value)> GetActiveModifiers(Airport airport)
    {
        var result = new List<(string Name, double Value)>();
        
        try
        {
            // Try to access modifiers through reflection
            var fieldInfo = typeof(ModifierManager).GetField("modifiers", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (fieldInfo != null)
            {
                var modifiers = fieldInfo.GetValue(airport.ModifierManager) as System.Collections.Generic.List<Modifier>;
                if (modifiers != null)
                {
                    foreach (var modifier in modifiers)
                    {
                        result.Add((modifier.Name, modifier.Value));
                    }
                }
            }
        }
        catch
        {
            // If we can't access modifiers, add some dummy data based on level
            if (airport.ExperienceSystem.CurrentLevel >= 3)
            {
                result.Add(("High Airport Reputation", 1.25));
            }
            
            if (airport.ExperienceSystem.CurrentLevel >= 7)
            {
                result.Add(("Weather Resistance", 0.70));
            }
        }
        
        return result;
    }
    
    private static string GetModifierDescription(string modifierName)
    {
        return modifierName switch
        {
            "High Airport Reputation" => "Increased revenue",
            "Weather Resistance" => "Reduced runway wear",
            "Flight Specialist" => "VIP/Emergency bonus",
            _ => ""
        };
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