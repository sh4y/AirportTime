public static class ConsoleUI
{
    private static readonly int columnWidth = 60;
    private static readonly int spacing = 14;
    private static GameMetrics metrics;
    
    public static void DisplayStatus(Airport airport, int currentTick)
    {
        // Skip display if game is over
        if (airport.IsGameOver) return;
        
        // Initialize metrics if needed
        if (metrics == null)
        {
            metrics = new GameMetrics(airport, airport.GameLogger);
        }
        
        Console.Clear();
        
        // Draw the different sections
        DrawHeader(airport, currentTick);
        DrawEmergencies(currentTick);
        DrawTwoColumnContent(airport);
        DrawLogs(airport);
        DrawControls();
    }
    
    private static void DrawHeader(Airport airport, int currentTick)
    {
        var airportInfo = metrics.AirportMetrics;
        var timeInfo = metrics.AirportMetrics.GetTimeInfo(currentTick);
        
        Console.WriteLine($"=== AIRPORT MANAGER: {airportInfo.Name} ===");
        Console.WriteLine($"Day {timeInfo.Days} {timeInfo.Hours:D2}:{timeInfo.Minutes:D2}  |  Time: {currentTick} ticks  |  Gold: ${airportInfo.GoldBalance:N0}  |  Weather: {airportInfo.GetWeatherInfo()}");
        Console.WriteLine($"Landing Mode: {airportInfo.CurrentLandingMode}  |  Active Flights: {airportInfo.ActiveFlightsCount}");
        Console.WriteLine(new string('-', Console.WindowWidth - 1));
    }
    
    private static void DrawEmergencies(int currentTick)
    {
        var emergencies = metrics.EmergencyMetrics.GetActiveEmergencies(currentTick);
        
        if (emergencies.Count == 0) return;
        
        Console.WriteLine("!!! EMERGENCY FLIGHTS REQUIRING IMMEDIATE ATTENTION !!!");
        
        foreach (var emergency in emergencies)
        {
            if (emergency.TimeRemaining <= 5) Console.ForegroundColor = ConsoleColor.Red;
            else if (emergency.TimeRemaining <= 10) Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.WriteLine($"  Flight {emergency.FlightNumber} | {emergency.FlightType} | {emergency.Priority} | Response Time: {emergency.TimeRemaining} ticks remaining");
            Console.ResetColor();
        }
        
        Console.WriteLine(new string('-', Console.WindowWidth - 1));
    }
    
    private static void DrawTwoColumnContent(Airport airport)
    {
        // Left and right column pairs
        DrawExperienceAndFlights(airport);
        DrawFailuresAndModifiers(airport);
        DrawRunwaysAndAchievements(airport);
    }
    
    private static void DrawExperienceAndFlights(Airport airport)
    {
        var xpMetrics = metrics.ExperienceMetrics;
        var upcomingFlights = metrics.FlightMetrics.GetUpcomingFlights(4);
        
        // Section headers
        Console.Write("LEVEL AND EXPERIENCE");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        Console.WriteLine("UPCOMING FLIGHTS");
        
        // Level info
        Console.Write($"  Current: Level {xpMetrics.CurrentLevel}");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        Console.WriteLine("  ID      | Type       | Priority   | Size    | Passengers | Revenue");
        
        // Progress bar
        int progressPercentage = xpMetrics.ProgressPercentage;
        int barWidth = 40;
        int filledWidth = (int)Math.Round(progressPercentage / 100.0 * barWidth);
        
        Console.Write("  Progress: [");
        Console.ForegroundColor = GetProgressColor(progressPercentage);
        Console.Write(new string('█', filledWidth));
        Console.ResetColor();
        Console.Write(new string('░', barWidth - filledWidth));
        Console.Write($"] {progressPercentage}%");
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        // First flight
        if (upcomingFlights.Count > 0)
        {
            var flight = upcomingFlights[0];
            string statusIndicator = flight.IsEmergency ? "⚠️" : (flight.IsDelayed ? "!" : " ");
            
            if (flight.IsEmergency) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  {statusIndicator}{flight.FlightNumber,-8} | {flight.FlightType,-10} | {flight.Priority,-10} | {flight.PlaneSize,-7} | {flight.Passengers,8} pax | ${flight.EstimatedRevenue,7:N0}");
            if (flight.IsEmergency) Console.ResetColor();
        }
        else
        {
            Console.WriteLine("  No flights scheduled.");
        }
        
        // XP details
        Console.Write($"  XP: {xpMetrics.CurrentXP}/{xpMetrics.NextLevelXP} ({xpMetrics.XPNeeded} needed)");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        // Second flight
        if (upcomingFlights.Count > 1)
        {
            var flight = upcomingFlights[1];
            string statusIndicator = flight.IsEmergency ? "⚠️" : (flight.IsDelayed ? "!" : " ");
            
            if (flight.IsEmergency) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  {statusIndicator}{flight.FlightNumber,-8} | {flight.FlightType,-10} | {flight.Priority,-10} | {flight.PlaneSize,-7} | {flight.Passengers,8} pax | ${flight.EstimatedRevenue,7:N0}");
            if (flight.IsEmergency) Console.ResetColor();
        }
        else
        {
            Console.WriteLine();
        }
        
        // Next level details
        string timeEstimate = xpMetrics.GetEstimatedTimeToNextLevel();
        Console.Write($"  Next Level: {timeEstimate} • Bonus: +{xpMetrics.EfficiencyBonus}%");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        // Third flight
        if (upcomingFlights.Count > 2)
        {
            var flight = upcomingFlights[2];
            string statusIndicator = flight.IsEmergency ? "⚠️" : (flight.IsDelayed ? "!" : " ");
            
            if (flight.IsEmergency) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  {statusIndicator}{flight.FlightNumber,-8} | {flight.FlightType,-10} | {flight.Priority,-10} | {flight.PlaneSize,-7} | {flight.Passengers,8} pax | ${flight.EstimatedRevenue,7:N0}");
            if (flight.IsEmergency) Console.ResetColor();
        }
        else
        {
            Console.WriteLine();
        }
        
        // Unlocks
        string nextUnlock = xpMetrics.GetNextUnlock();
        if (nextUnlock.Length > columnWidth - 12)
            nextUnlock = nextUnlock.Substring(0, columnWidth - 15) + "...";
            
        Console.Write($"  Unlocks: {nextUnlock}");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        // Fourth flight
        if (upcomingFlights.Count > 3)
        {
            var flight = upcomingFlights[3];
            string statusIndicator = flight.IsEmergency ? "⚠️" : (flight.IsDelayed ? "!" : " ");
            
            if (flight.IsEmergency) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  {statusIndicator}{flight.FlightNumber,-8} | {flight.FlightType,-10} | {flight.Priority,-10} | {flight.PlaneSize,-7} | {flight.Passengers,8} pax | ${flight.EstimatedRevenue,7:N0}");
            if (flight.IsEmergency) Console.ResetColor();
        }
        else
        {
            Console.WriteLine();
        }
        
        // Show more flights message if needed
        if (upcomingFlights.Count > 4)
        {
            Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
            Console.WriteLine($"  + {upcomingFlights.Count - 4} more flights scheduled...");
        }
        
        Console.WriteLine(new string('-', Console.WindowWidth - 1));
    }
    
    private static void DrawFailuresAndModifiers(Airport airport)
    {
        var topFailures = metrics.FailureMetrics.GetTopFailures();
        var modifiers = metrics.ModifierMetrics.GetActiveModifiers();
        
        // Section headers
        Console.Write("FAILURES");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        Console.WriteLine("ACTIVE MODIFIERS");
        
        // First row
        if (topFailures.Count == 0)
        {
            Console.Write("  No failures recorded.");
        }
        else
        {
            var failure = topFailures[0];
            if (failure.Percentage >= 75) Console.ForegroundColor = ConsoleColor.Red;
            else if (failure.Percentage >= 50) Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.Write($"  {failure.Type,-20} | Count: {failure.Count}/{failure.Threshold} | {failure.DangerLevel}");
            Console.ResetColor();
        }
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        if (modifiers.Count == 0)
        {
            Console.WriteLine("  No active modifiers.");
        }
        else
        {
            var modifier = modifiers[0];
            string name = modifier.Name.Length > 28 ? modifier.Name.Substring(0, 25) + "..." : modifier.Name;
            Console.WriteLine($"  {name,-28} | {modifier.PercentageEffect,5:F1}% {modifier.EffectType,-7}");
        }
        
        // Second row
        if (topFailures.Count > 1)
        {
            var failure = topFailures[1];
            if (failure.Percentage >= 75) Console.ForegroundColor = ConsoleColor.Red;
            else if (failure.Percentage >= 50) Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.Write($"  {failure.Type,-20} | Count: {failure.Count}/{failure.Threshold} | {failure.DangerLevel}");
            Console.ResetColor();
        }
        else
        {
            Console.Write("");
        }
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        if (modifiers.Count > 1)
        {
            var modifier = modifiers[1];
            string name = modifier.Name.Length > 28 ? modifier.Name.Substring(0, 25) + "..." : modifier.Name;
            Console.WriteLine($"  {name,-28} | {modifier.PercentageEffect,5:F1}% {modifier.EffectType,-7}");
        }
        else
        {
            Console.WriteLine();
        }
        
        // Third row
        if (topFailures.Count > 2)
        {
            var failure = topFailures[2];
            if (failure.Percentage >= 75) Console.ForegroundColor = ConsoleColor.Red;
            else if (failure.Percentage >= 50) Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.Write($"  {failure.Type,-20} | Count: {failure.Count}/{failure.Threshold} | {failure.DangerLevel}");
            Console.ResetColor();
        }
        else
        {
            Console.Write("");
        }
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        if (modifiers.Count > 2)
        {
            var modifier = modifiers[2];
            string name = modifier.Name.Length > 28 ? modifier.Name.Substring(0, 25) + "..." : modifier.Name;
            Console.WriteLine($"  {name,-28} | {modifier.PercentageEffect,5:F1}% {modifier.EffectType,-7}");
        }
        else
        {
            Console.WriteLine();
        }
        
        Console.WriteLine(new string('-', Console.WindowWidth - 1));
    }
    
    private static void DrawRunwaysAndAchievements(Airport airport)
    {
        var runways = metrics.RunwayMetrics.GetRunwayInfo();
        var unlockedAchievements = metrics.AchievementMetrics.GetRecentAchievements();
        
        // Section headers
        Console.Write("RUNWAYS");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        Console.WriteLine("ACHIEVEMENTS");
        
        // Subtitle row for Runways
        Console.Write("  ID              | Type      | Length  | Wear  | Status");
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        // First achievement
        if (unlockedAchievements.Count == 0)
        {
            Console.WriteLine("  No achievements unlocked yet.");
        }
        else
        {
            var achievement = unlockedAchievements[0];
            string desc = achievement.Description.Length > 32 ? achievement.Description.Substring(0, 29) + "..." : achievement.Description;
            Console.WriteLine($"  🏆 {achievement.Name,-20} | {desc}");
        }
        
        // First runway and second achievement
        if (runways.Count == 0)
        {
            Console.Write("  No runways available. Visit the shop to purchase runways.");
        }
        else
        {
            var runway = runways[0];
            string wearIndicator = runway.WearLevel >= 80 ? "⚠️" : (runway.WearLevel >= 50 ? "!" : " ");
            string status = runway.DetailedStatus.Length > 20 ? runway.DetailedStatus.Substring(0, 17) + "..." : runway.DetailedStatus;
            Console.Write($"  {runway.Name,-15} | {runway.Type,-8} | {runway.Length,5}m  | {wearIndicator}{runway.WearLevel,2}%   | {status}");
        }
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        if (unlockedAchievements.Count > 1)
        {
            var achievement = unlockedAchievements[1];
            string desc = achievement.Description.Length > 32 ? achievement.Description.Substring(0, 29) + "..." : achievement.Description;
            Console.WriteLine($"  🏆 {achievement.Name,-20} | {desc}");
        }
        else
        {
            Console.WriteLine();
        }
        
        // Second runway and third achievement
        if (runways.Count > 1)
        {
            var runway = runways[1];
            string wearIndicator = runway.WearLevel >= 80 ? "⚠️" : (runway.WearLevel >= 50 ? "!" : " ");
            string status = runway.DetailedStatus.Length > 20 ? runway.DetailedStatus.Substring(0, 17) + "..." : runway.DetailedStatus;
            Console.Write($"  {runway.Name,-15} | {runway.Type,-8} | {runway.Length,5}m  | {wearIndicator}{runway.WearLevel,2}%   | {status}");
        }
        else
        {
            Console.Write("");
        }
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        if (unlockedAchievements.Count > 2)
        {
            var achievement = unlockedAchievements[2];
            string desc = achievement.Description.Length > 32 ? achievement.Description.Substring(0, 29) + "..." : achievement.Description;
            Console.WriteLine($"  🏆 {achievement.Name,-20} | {desc}");
        }
        else
        {
            Console.WriteLine();
        }
        
        // Third runway and achievement count
        if (runways.Count > 2)
        {
            var runway = runways[2];
            string wearIndicator = runway.WearLevel >= 80 ? "⚠️" : (runway.WearLevel >= 50 ? "!" : " ");
            string status = runway.DetailedStatus.Length > 20 ? runway.DetailedStatus.Substring(0, 17) + "..." : runway.DetailedStatus;
            Console.Write($"  {runway.Name,-15} | {runway.Type,-8} | {runway.Length,5}m  | {wearIndicator}{runway.WearLevel,2}%   | {status}");
        }
        else
        {
            Console.Write("");
        }
        
        Console.SetCursorPosition(columnWidth + spacing, Console.CursorTop);
        
        int totalUnlocked = metrics.AchievementMetrics.TotalUnlockedAchievements;
        if (totalUnlocked > 0)
        {
            Console.WriteLine($"  Total achievements unlocked: {totalUnlocked}");
        }
        else
        {
            Console.WriteLine();
        }
        
        Console.WriteLine(new string('-', Console.WindowWidth - 1));
    }
    
    private static void DrawLogs(Airport airport)
    {
        Console.WriteLine("RECENT ACTIVITY");
        airport.GameLogger.DisplayRecentLogs(4); // Show last 4 logs
        Console.WriteLine(new string('-', Console.WindowWidth - 1));
    }
    
    private static void DrawControls()
    {
        Console.WriteLine("CONTROLS: Q:Pause  F:Flight  S:Shop  R:Repair  M:Mode  T:Metrics  H:Help  X:Exit");
    }
    
    private static ConsoleColor GetProgressColor(int percentage)
    {
        if (percentage < 30) return ConsoleColor.Red;
        if (percentage < 70) return ConsoleColor.Yellow;
        return ConsoleColor.Green;
    }
}