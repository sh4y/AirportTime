using System;
using System.Collections.Generic;
using System.Linq;
using AirportTime;

/// <summary>
/// A clean, simple implementation of the IAirportView interface for console display
/// </summary>
public class ConsoleUI : IAirportView
{
    private const int WIDTH = 100;
    private const int DIVIDER_POSITION = 60;
    
    public ConsoleUI()
    {
        // Set console window properties if needed
    }
    
    /// <summary>
    /// Clears the console display
    /// </summary>
    public void ClearDisplay()
    {
        Console.Clear();
    }
    
    /// <summary>
    /// Draws a horizontal divider line
    /// </summary>
    private void DrawDivider()
    {
        Console.WriteLine(new string('-', Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 100));
    }
    
    /// <summary>
    /// Updates the header information
    /// </summary>
    public void UpdateHeaderInfo(string airportName, GameTimeInfo timeInfo, double gold, string weatherInfo)
    {
        Console.WriteLine($"=== AIRPORT MANAGER: {airportName} ===");
        Console.WriteLine($"Day {timeInfo.Days} {timeInfo.Hours:D2}:{timeInfo.Minutes:D2}  |  Time: {timeInfo.TotalTicks} ticks  |  Gold: ${gold:N0}  |  Weather: {weatherInfo}");
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the landing mode and active flights information
    /// </summary>
    public void UpdateLandingInfo(string landingMode, int activeFlightCount)
    {
        Console.WriteLine($"Landing Mode: {landingMode}  |  Active Flights: {activeFlightCount}");
        DrawDivider();
    }
    
    /// <summary>
    /// Displays emergency flights
    /// </summary>
    public void DisplayEmergencies(List<EmergencyInfo> emergencies, int currentTick)
    {
        if (emergencies.Count == 0)
            return;
            
        Console.WriteLine("!!! EMERGENCY FLIGHTS REQUIRING IMMEDIATE ATTENTION !!!");
        
        foreach (var emergency in emergencies)
        {
            if (emergency.TimeRemaining <= 5) Console.ForegroundColor = ConsoleColor.Red;
            else if (emergency.TimeRemaining <= 10) Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.WriteLine($"  Flight {emergency.FlightNumber} | {emergency.FlightType} | {emergency.Priority} | Response Time: {emergency.TimeRemaining} ticks remaining");
            Console.ResetColor();
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the experience display
    /// </summary>
    public void UpdateExperienceDisplay(int currentLevel, int currentXP, int nextLevelXP, 
                                       int progressPercentage, string timeEstimate, 
                                       int efficiencyBonus, string nextUnlock)
    {
        Console.WriteLine("LEVEL AND EXPERIENCE");
        Console.WriteLine($"  Current: Level {currentLevel}");
        
        // Progress bar
        int barWidth = 40;
        int filledWidth = (int)Math.Round(progressPercentage / 100.0 * barWidth);
        
        Console.Write("  Progress: [");
        Console.ForegroundColor = GetProgressColor(progressPercentage);
        Console.Write(new string('█', filledWidth));
        Console.ResetColor();
        Console.Write(new string('░', barWidth - filledWidth));
        Console.WriteLine($"] {progressPercentage}%");
        
        Console.WriteLine($"  XP: {currentXP}/{nextLevelXP} ({nextLevelXP - currentXP} needed)");
        Console.WriteLine($"  Next Level: {timeEstimate} • Bonus: +{efficiencyBonus}%");
        
        // Truncate next unlock if too long
        if (nextUnlock.Length > 50)
            nextUnlock = nextUnlock.Substring(0, 47) + "...";
            
        Console.WriteLine($"  Unlocks: {nextUnlock}");
    }
    
    /// <summary>
    /// Displays upcoming flights
    /// </summary>
    public void DisplayUpcomingFlights(List<FlightInfo> upcomingFlights)
    {
        Console.WriteLine("UPCOMING FLIGHTS");
        Console.WriteLine("  ID      | Type       | Priority   | Size    | Passengers | Revenue");
        
        if (upcomingFlights.Count == 0)
        {
            Console.WriteLine("  No flights scheduled.");
            return;
        }
        
        // Display up to 4 flights
        int displayCount = Math.Min(upcomingFlights.Count, 4);
        for (int i = 0; i < displayCount; i++)
        {
            var flight = upcomingFlights[i];
            string statusIndicator = flight.IsEmergency ? "⚠️" : (flight.IsDelayed ? "!" : " ");
            
            if (flight.IsEmergency) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  {statusIndicator}{flight.FlightNumber,-8} | {flight.FlightType,-10} | {flight.Priority,-10} | {flight.PlaneSize,-7} | {flight.Passengers,8} pax | ${flight.EstimatedRevenue,7:N0}");
            Console.ResetColor();
        }
        
        // Show more flights message if needed
        if (upcomingFlights.Count > 4)
        {
            Console.WriteLine($"  + {upcomingFlights.Count - 4} more flights scheduled...");
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the failures display
    /// </summary>
    public void UpdateFailuresDisplay(List<FailureInfo> failures)
    {
        Console.WriteLine("FAILURES");
        
        if (failures.Count == 0)
        {
            Console.WriteLine("  No failures recorded.");
            return;
        }
        
        // Display up to 3 failures
        int displayCount = Math.Min(failures.Count, 3);
        for (int i = 0; i < displayCount; i++)
        {
            var failure = failures[i];
            if (failure.Percentage >= 75) Console.ForegroundColor = ConsoleColor.Red;
            else if (failure.Percentage >= 50) Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.WriteLine($"  {failure.Type,-20} | Count: {failure.Count}/{failure.Threshold} | {failure.DangerLevel}");
            Console.ResetColor();
        }
    }
    
    /// <summary>
    /// Updates the modifiers display
    /// </summary>
    public void UpdateModifiersDisplay(List<ModifierInfo> modifiers)
    {
        Console.WriteLine("ACTIVE MODIFIERS");
        
        if (modifiers.Count == 0)
        {
            Console.WriteLine("  No active modifiers.");
            return;
        }
        
        // Display up to 3 modifiers
        int displayCount = Math.Min(modifiers.Count, 3);
        for (int i = 0; i < displayCount; i++)
        {
            var modifier = modifiers[i];
            string name = modifier.Name.Length > 28 ? modifier.Name.Substring(0, 25) + "..." : modifier.Name;
            Console.WriteLine($"  {name,-28} | {modifier.PercentageEffect,5:F1}% {modifier.EffectType,-7}");
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the runways display
    /// </summary>
    public void UpdateRunwaysDisplay(List<RunwayInfo> runways)
    {
        Console.WriteLine("RUNWAYS");
        Console.WriteLine("  ID              | Type      | Length  | Wear  | Status");
        
        if (runways.Count == 0)
        {
            Console.WriteLine("  No runways available. Visit the shop to purchase runways.");
            return;
        }
        
        // Display all runways
        foreach (var runway in runways)
        {
            string wearIndicator = runway.WearLevel >= 80 ? "⚠️" : (runway.WearLevel >= 50 ? "!" : " ");
            string status = runway.DetailedStatus;
            if (status.Length > 30)
                status = status.Substring(0, 27) + "...";
                
            Console.WriteLine($"  {runway.Name,-15} | {runway.Type,-8} | {runway.Length,5}m  | {wearIndicator}{runway.WearLevel,2}%   | {status}");
        }
    }
    
    /// <summary>
    /// Updates the achievements display
    /// </summary>
    public void UpdateAchievementsDisplay(List<Achievement> achievements, int totalUnlocked)
    {
        Console.WriteLine("ACHIEVEMENTS");
        
        if (achievements.Count == 0)
        {
            Console.WriteLine("  No achievements unlocked yet.");
            return;
        }
        
        // Display up to 3 achievements
        int displayCount = Math.Min(achievements.Count, 3);
        for (int i = 0; i < displayCount; i++)
        {
            var achievement = achievements[i];
            string desc = achievement.Description;
            if (desc.Length > 40)
                desc = desc.Substring(0, 37) + "...";
                
            Console.WriteLine($"  🏆 {achievement.Name,-20} | {desc}");
        }
        
        if (totalUnlocked > 0)
        {
            Console.WriteLine($"  Total achievements unlocked: {totalUnlocked}");
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the logs display
    /// </summary>
    public void UpdateLogsDisplay(List<string> recentLogs)
    {
        Console.WriteLine("RECENT ACTIVITY");
        
        // If we have logs, display them
        if (recentLogs.Count > 0)
        {
            int displayCount = Math.Min(recentLogs.Count, 4);
            for (int i = 0; i < displayCount; i++)
            {
                string log = recentLogs[i];
                if (log.Length > 90)
                    log = log.Substring(0, 87) + "...";
                    
                Console.WriteLine($"  {log}");
            }
        }
        else
        {
            // Display some empty space for logs
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine();
            }
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Displays the controls
    /// </summary>
    public void DisplayControls()
    {
        Console.WriteLine("CONTROLS: Q:Pause  F:Flight  S:Shop  R:Repair  M:Mode  T:Metrics  H:Help  X:Exit");
    }
    
    /// <summary>
    /// Shows a notification
    /// </summary>
    public void ShowNotification(string message, bool isImportant = false)
    {
        if (isImportant)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"!!! {message} !!!");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(message);
        }
    }
    
    /// <summary>
    /// Prompts the user to select a runway
    /// </summary>
    public Runway PromptRunwaySelection(Flight flight, List<Runway> availableRunways)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        
        // Highlight emergency flights
        if (flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠️ EMERGENCY FLIGHT {flight.FlightNumber} REQUIRES IMMEDIATE LANDING ⚠️");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Flight {flight.FlightNumber} requires landing. Please assign a runway:");
        }
        
        Console.WriteLine(new string('-', 60));
        
        // Display flight details
        Console.WriteLine($"Flight Details: {flight.FlightNumber} ({flight.Type}, {flight.Priority})");
        Console.WriteLine($"Aircraft: {flight.Plane.PlaneID} (Size: {flight.Plane.Size}, Required Runway Length: {flight.Plane.RequiredRunwayLength}m)");
        Console.WriteLine($"Passengers: {flight.Passengers}");
        Console.WriteLine(new string('-', 60));
        
        // Display runway options
        Console.WriteLine("Available Runways:");
        for (int i = 0; i < availableRunways.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {availableRunways[i].Name} " +
                             $"(Length: {availableRunways[i].Length}m, " +
                             $"Wear: {availableRunways[i].WearLevel}%)");
        }
        Console.WriteLine(new string('-', 60));
        
        // Only show auto-select option for non-emergency flights
        bool isEmergency = flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency;
        if (!isEmergency)
        {
            Console.WriteLine($"{availableRunways.Count + 1}. Auto-select best runway");
        }
        
        Console.WriteLine($"{(isEmergency ? availableRunways.Count + 1 : availableRunways.Count + 2)}. Delay flight (5 ticks)");
        Console.WriteLine(new string('=', 60));
        
        // Get user input
        Console.Write("Enter your selection: ");
        string input = Console.ReadLine();
        
        // Process the selection
        if (int.TryParse(input, out int selection))
        {
            // Auto-select option (only for non-emergency flights)
            if (!isEmergency && selection == availableRunways.Count + 1)
            {
                // Return null to indicate auto-selection
                return null;
            }
            // Delay option
            else if ((isEmergency && selection == availableRunways.Count + 1) || 
                    (!isEmergency && selection == availableRunways.Count + 2))
            {
                // Return null to indicate delay
                return null;
            }
            // Valid runway selection
            else if (selection >= 1 && selection <= availableRunways.Count)
            {
                return availableRunways[selection - 1];
            }
        }
        
        // If we get here, input was invalid, return null to indicate delay
        return null;
    }
    
    /// <summary>
    /// Gets the appropriate color for progress display
    /// </summary>
    private ConsoleColor GetProgressColor(int percentage)
    {
        if (percentage < 30) return ConsoleColor.Red;
        if (percentage < 70) return ConsoleColor.Yellow;
        return ConsoleColor.Green;
    }
}