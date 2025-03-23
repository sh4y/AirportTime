using System;
using System.Collections.Generic;
using System.Linq;
using AirportTime;

/// <summary>
/// A visually enhanced implementation of the IAirportView interface for console display
/// </summary>
public class ConsoleUI : IAirportView
{
    private const int WIDTH = 100;
    private const int DIVIDER_POSITION = 60;
    private const char BOX_HORIZONTAL = '═';
    private const char BOX_VERTICAL = '║';
    private const char BOX_TOP_LEFT = '╔';
    private const char BOX_TOP_RIGHT = '╗';
    private const char BOX_BOTTOM_LEFT = '╚';
    private const char BOX_BOTTOM_RIGHT = '╝';
    private const char BOX_T_DOWN = '╦';
    private const char BOX_T_UP = '╩';
    private const char BOX_T_RIGHT = '╠';
    private const char BOX_T_LEFT = '╣';
    private const char BOX_CROSS = '╬';
    
    // Status symbols
    private const string SYMBOL_WARNING = "⚠";
    private const string SYMBOL_ALERT = "‼";
    private const string SYMBOL_NORMAL = "•";
    private const string SYMBOL_TROPHY = "🏆";
    private const string SYMBOL_MONEY = "$";
    private const string SYMBOL_CLOCK = "⏱";
    private const string SYMBOL_WEATHER = "☁";
    private const string SYMBOL_PASSENGERS = "👥";
    
    // Status bar characters
    private const char BAR_FILLED = '█';
    private const char BAR_EMPTY = '░';
    
    public ConsoleUI()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Ensure proper rendering of special characters
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
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('─', Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 100));
        Console.ResetColor();
    }
    
    /// <summary>
    /// Draws a styled section header
    /// </summary>
    private void DrawSectionHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"┏━━━━━━━━━━━━━━━━ {title} ━━━━━━━━━━━━━━━━┓");
        Console.ResetColor();
    }
    
    /// <summary>
    /// Updates the header information
    /// </summary>
    public void UpdateHeaderInfo(string airportName, GameTimeInfo timeInfo, double gold, string weatherInfo)
    {
        int width = Console.WindowWidth > 0 ? Console.WindowWidth - 1 : WIDTH;
        
        // Format the header title with airport name
        string title = $" AIRPORT MANAGER: {airportName.ToUpper()} ";
        int paddingTotal = width - title.Length;
        int leftPadding = paddingTotal / 2;
        int rightPadding = paddingTotal - leftPadding;
        
        // Draw top border with title
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(BOX_TOP_LEFT);
        Console.Write(new string(BOX_HORIZONTAL, leftPadding));
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.Write(title);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(new string(BOX_HORIZONTAL, rightPadding));
        Console.WriteLine(BOX_TOP_RIGHT);
        
        // Output game info
        Console.Write(BOX_VERTICAL);
        Console.ForegroundColor = ConsoleColor.White;
        
        // Day and time info
        Console.Write($" {SYMBOL_CLOCK} Day {timeInfo.Days} {timeInfo.Hours:D2}:{timeInfo.Minutes:D2}");
        
        // Total ticks
        Console.Write($" | Time: {timeInfo.TotalTicks} ticks");
        
        // Gold with color
        Console.Write(" | ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{SYMBOL_MONEY}{gold:N0}");
        
        // Weather info
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" | ");
        Console.ForegroundColor = GetWeatherColor(weatherInfo);
        Console.Write($"{SYMBOL_WEATHER} {weatherInfo}");
        
        // Padding and right border
        Console.ForegroundColor = ConsoleColor.White;
        int padding = width - 60; // Approximate length of the content
        if (padding > 0)
            Console.Write(new string(' ', padding));
            
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(BOX_VERTICAL);
        
        // Bottom border
        Console.Write(BOX_BOTTOM_LEFT);
        Console.Write(new string(BOX_HORIZONTAL, width - 2));
        Console.WriteLine(BOX_BOTTOM_RIGHT);
        Console.ResetColor();
        
        Console.WriteLine(); // Add some spacing
    }
    
    /// <summary>
    /// Returns appropriate color for weather condition
    /// </summary>
    private ConsoleColor GetWeatherColor(string weather)
    {
        weather = weather.ToLower();
        if (weather.Contains("storm") || weather.Contains("thunder") || weather.Contains("severe"))
            return ConsoleColor.Red;
        if (weather.Contains("rain") || weather.Contains("snow") || weather.Contains("fog"))
            return ConsoleColor.Yellow;
        if (weather.Contains("cloud"))
            return ConsoleColor.Gray;
        return ConsoleColor.Cyan; // Clear/sunny
    }
    
    /// <summary>
    /// Updates the landing mode and active flights information
    /// </summary>
    public void UpdateLandingInfo(string landingMode, int activeFlightCount)
    {
        Console.Write("Landing Mode: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(landingMode);
        Console.ResetColor();
        
        Console.Write("  |  Active Flights: ");
        Console.ForegroundColor = activeFlightCount > 5 ? ConsoleColor.Yellow : 
                                 (activeFlightCount > 10 ? ConsoleColor.Red : ConsoleColor.Green);
        Console.Write(activeFlightCount);
        Console.ResetColor();
        Console.WriteLine();
        
        DrawDivider();
    }
    
    /// <summary>
    /// Displays emergency flights
    /// </summary>
    public void DisplayEmergencies(List<EmergencyInfo> emergencies, int currentTick)
    {
        if (emergencies.Count == 0)
            return;
            
        // Draw alert box for emergencies
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($" {SYMBOL_WARNING} EMERGENCY FLIGHTS REQUIRING IMMEDIATE ATTENTION {SYMBOL_WARNING} ");
        Console.ResetColor();
        
        foreach (var emergency in emergencies)
        {
            ConsoleColor color = ConsoleColor.White;
            string timePrefix = SYMBOL_NORMAL;
            
            if (emergency.TimeRemaining <= 5) 
            {
                color = ConsoleColor.Red;
                timePrefix = SYMBOL_WARNING;
            }
            else if (emergency.TimeRemaining <= 10)
            {
                color = ConsoleColor.Yellow;
                timePrefix = SYMBOL_ALERT;
            }
            
            Console.ForegroundColor = color;
            Console.WriteLine($"  Flight {emergency.FlightNumber} | {emergency.FlightType} | {emergency.Priority} | {timePrefix} Response Time: {emergency.TimeRemaining} ticks remaining");
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
        DrawSectionHeader("LEVEL AND EXPERIENCE");
        
        // Current level with color
        Console.Write("  Current: ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"Level {currentLevel}");
        Console.ResetColor();
        
        // Progress bar
        int barWidth = 40;
        int filledWidth = (int)Math.Round(progressPercentage / 100.0 * barWidth);
        
        Console.Write("  Progress: [");
        Console.ForegroundColor = GetProgressColor(progressPercentage);
        Console.Write(new string(BAR_FILLED, filledWidth));
        Console.ResetColor();
        Console.Write(new string(BAR_EMPTY, barWidth - filledWidth));
        Console.ForegroundColor = GetProgressColor(progressPercentage);
        Console.WriteLine($"] {progressPercentage}%");
        Console.ResetColor();
        
        // XP info
        Console.Write("  XP: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{currentXP}/{nextLevelXP}");
        Console.ResetColor();
        Console.WriteLine($" ({nextLevelXP - currentXP} needed)");
        
        // Next level info
        Console.Write("  Next Level: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(timeEstimate);
        Console.ResetColor();
        Console.Write(" • Bonus: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"+{efficiencyBonus}%");
        Console.ResetColor();
        
        // Truncate next unlock if too long
        if (nextUnlock.Length > 50)
            nextUnlock = nextUnlock.Substring(0, 47) + "...";
            
        Console.Write("  Unlocks: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(nextUnlock);
        Console.ResetColor();
        
        Console.WriteLine(); // Add spacing
    }
    
    /// <summary>
    /// Displays upcoming flights
    /// </summary>
    public void DisplayUpcomingFlights(List<FlightInfo> upcomingFlights)
    {
        DrawSectionHeader("UPCOMING FLIGHTS");
        
        // Table header with column formatting
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  ID      | Type       | Priority   | Size    | Passengers      | Revenue");
        Console.WriteLine("  " + new string('─', 75));
        Console.ResetColor();
        
        if (upcomingFlights.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  No flights scheduled.");
            Console.ResetColor();
            return;
        }
        
        // Display up to 4 flights
        int displayCount = Math.Min(upcomingFlights.Count, 4);
        for (int i = 0; i < displayCount; i++)
        {
            var flight = upcomingFlights[i];
            string statusIndicator = flight.IsEmergency ? SYMBOL_WARNING : (flight.IsDelayed ? SYMBOL_ALERT : SYMBOL_NORMAL);
            
            if (flight.IsEmergency) 
                Console.ForegroundColor = ConsoleColor.Red;
            else if (flight.IsDelayed)
                Console.ForegroundColor = ConsoleColor.Yellow;
                
            Console.Write($"  {statusIndicator} {flight.FlightNumber,-7} | ");
            Console.Write($"{flight.FlightType,-10} | ");
            Console.Write($"{flight.Priority,-10} | ");
            Console.Write($"{flight.PlaneSize,-7} | ");
            
            // Format passengers with symbol
            Console.Write($"{SYMBOL_PASSENGERS} {flight.Passengers,6} pax | ");
            
            // Format revenue with color
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{SYMBOL_MONEY}{flight.EstimatedRevenue,7:N0}");
            Console.ResetColor();
        }
        
        // Show more flights message if needed
        if (upcomingFlights.Count > 4)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  + {upcomingFlights.Count - 4} more flights scheduled...");
            Console.ResetColor();
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the failures display
    /// </summary>
    public void UpdateFailuresDisplay(List<FailureInfo> failures)
    {
        DrawSectionHeader("FAILURES");
        
        if (failures.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  No failures recorded.");
            Console.ResetColor();
            return;
        }
        
        // Display up to 3 failures
        int displayCount = Math.Min(failures.Count, 3);
        for (int i = 0; i < displayCount; i++)
        {
            var failure = failures[i];
            
            // Set color based on severity
            if (failure.Percentage >= 75) 
                Console.ForegroundColor = ConsoleColor.Red;
            else if (failure.Percentage >= 50) 
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.White;
            
            // Display failure type
            Console.Write($"  {failure.Type,-20} | ");
            
            // Display count with progress bar
            Console.Write("Count: ");
            Console.Write($"{failure.Count}/{failure.Threshold} | ");
            
            // Display danger level with appropriate color
            if (failure.DangerLevel.Contains("Critical"))
                Console.ForegroundColor = ConsoleColor.Red;
            else if (failure.DangerLevel.Contains("High"))
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;
                
            Console.WriteLine(failure.DangerLevel);
            Console.ResetColor();
        }
        
        Console.WriteLine(); // Add spacing
    }
    
    /// <summary>
    /// Updates the modifiers display
    /// </summary>
    public void UpdateModifiersDisplay(List<ModifierInfo> modifiers)
    {
        DrawSectionHeader("ACTIVE MODIFIERS");
        
        if (modifiers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  No active modifiers.");
            Console.ResetColor();
            return;
        }
        
        // Display up to 3 modifiers
        int displayCount = Math.Min(modifiers.Count, 3);
        for (int i = 0; i < displayCount; i++)
        {
            var modifier = modifiers[i];
            string name = modifier.Name.Length > 28 ? modifier.Name.Substring(0, 25) + "..." : modifier.Name;
            
            Console.Write($"  {name,-28} | ");
            
            // Color the percentage effect based on whether it's positive or negative
            bool isPositive = modifier.PercentageEffect > 0 || 
                             (modifier.PercentageEffect == 0 && 
                              (modifier.EffectType.Contains("Bonus") || modifier.EffectType.Contains("Increase")));
                              
            Console.ForegroundColor = isPositive ? ConsoleColor.Green : ConsoleColor.Red;
            string prefix = isPositive ? "+" : "";
            Console.Write($"{prefix}{modifier.PercentageEffect,5:F1}% ");
            
            // Effect type
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{modifier.EffectType,-7}");
            Console.ResetColor();
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the runways display
    /// </summary>
    public void UpdateRunwaysDisplay(List<RunwayInfo> runways)
    {
        DrawSectionHeader("RUNWAYS");
        
        // Table header
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  ID              | Type      | Length  | Wear     | Status");
        Console.WriteLine("  " + new string('─', 75));
        Console.ResetColor();
        
        if (runways.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  No runways available. Visit the shop to purchase runways.");
            Console.ResetColor();
            return;
        }
        
        // Display all runways
        foreach (var runway in runways)
        {
            // Determine wear indicator and color
            string wearIndicator = runway.WearLevel >= 80 ? SYMBOL_WARNING : 
                                   (runway.WearLevel >= 50 ? SYMBOL_ALERT : SYMBOL_NORMAL);
            
            ConsoleColor wearColor = runway.WearLevel >= 80 ? ConsoleColor.Red : 
                                    (runway.WearLevel >= 50 ? ConsoleColor.Yellow : ConsoleColor.Green);
            
            // Truncate status if needed
            string status = runway.DetailedStatus;
            if (status.Length > 30)
                status = status.Substring(0, 27) + "...";
            
            // Display runway ID and type
            Console.Write($"  {runway.Name,-15} | {runway.Type,-8} | ");
            
            // Display runway length
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{runway.Length,5}m");
            Console.ResetColor();
            Console.Write("  | ");
            
            // Display wear level with color
            Console.ForegroundColor = wearColor;
            Console.Write($"{wearIndicator} {runway.WearLevel,2}%");
            Console.ResetColor();
            Console.Write("   | ");
            
            // Display status with appropriate color
            if (status.Contains("Available") || status.Contains("Ready"))
                Console.ForegroundColor = ConsoleColor.Green;
            else if (status.Contains("Occupied") || status.Contains("In Use"))
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (status.Contains("Damaged") || status.Contains("Repair"))
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;
                
            Console.WriteLine(status);
            Console.ResetColor();
        }
        
        Console.WriteLine(); // Add spacing
    }
    
    /// <summary>
    /// Updates the achievements display
    /// </summary>
    public void UpdateAchievementsDisplay(List<Achievement> achievements, int totalUnlocked)
    {
        DrawSectionHeader("ACHIEVEMENTS");
        
        if (achievements.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  No achievements unlocked yet.");
            Console.ResetColor();
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
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  {SYMBOL_TROPHY} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{achievement.Name,-20} | ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(desc);
            Console.ResetColor();
        }
        
        if (totalUnlocked > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  Total achievements unlocked: {totalUnlocked}");
            Console.ResetColor();
        }
        
        DrawDivider();
    }
    
    /// <summary>
    /// Updates the logs display
    /// </summary>
    public void UpdateLogsDisplay(List<string> recentLogs)
    {
        DrawSectionHeader("RECENT ACTIVITY");
        
        // If we have logs, display them
        if (recentLogs.Count > 0)
        {
            int displayCount = Math.Min(recentLogs.Count, 4);
            for (int i = 0; i < displayCount; i++)
            {
                string log = recentLogs[i];
                if (log.Length > 90)
                    log = log.Substring(0, 87) + "...";
                
                // Apply color based on log content
                if (log.Contains("Emergency") || log.Contains("Failure") || log.Contains("Crash"))
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (log.Contains("Warning") || log.Contains("Delayed"))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (log.Contains("Success") || log.Contains("Achieved") || log.Contains("Completed"))
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (i == 0) // Most recent log
                    Console.ForegroundColor = ConsoleColor.White;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;
                    
                Console.WriteLine($"  {log}");
                Console.ResetColor();
            }
        }
        else
        {
            // Display some empty space for logs
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  No recent activity to display.");
            Console.ResetColor();
            // Add remaining empty lines
            for (int i = 0; i < 3; i++)
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
        Console.Write("CONTROLS: ");
        
        // Show each control with a different color to make them stand out
        WriteColoredControl("Q", "Pause", ConsoleColor.Yellow);
        WriteColoredControl("F", "Flight", ConsoleColor.Cyan);
        WriteColoredControl("S", "Shop", ConsoleColor.Green);
        WriteColoredControl("R", "Repair", ConsoleColor.Magenta);
        WriteColoredControl("M", "Mode", ConsoleColor.Blue);
        WriteColoredControl("T", "Metrics", ConsoleColor.Yellow);
        WriteColoredControl("H", "Help", ConsoleColor.Cyan);
        WriteColoredControl("X", "Exit", ConsoleColor.Red);
        
        Console.WriteLine();
    }
    
    /// <summary>
    /// Helper method to write a colored control instruction
    /// </summary>
    private void WriteColoredControl(string key, string action, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(key);
        Console.ResetColor();
        Console.Write(":");
        Console.Write(action);
        Console.Write("  ");
    }
    
    /// <summary>
    /// Shows a notification
    /// </summary>
    public void ShowNotification(string message, bool isImportant = false)
    {
        if (isImportant)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" {SYMBOL_WARNING} {message} {SYMBOL_WARNING} ");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($" {message} ");
            Console.ResetColor();
        }
    }
    
    /// <summary>
    /// Prompts the user to select a runway
    /// </summary>
    public Runway PromptRunwaySelection(Flight flight, List<Runway> availableRunways)
    {
        int width = Console.WindowWidth > 0 ? Console.WindowWidth - 1 : WIDTH;
        
        Console.WriteLine();
        
        // Draw top border
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(BOX_TOP_LEFT);
        Console.Write(new string(BOX_HORIZONTAL, width - 2));
        Console.WriteLine(BOX_TOP_RIGHT);
        
        // Highlight emergency flights
        if (flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(BOX_VERTICAL);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write($" {SYMBOL_WARNING} EMERGENCY FLIGHT {flight.FlightNumber} REQUIRES IMMEDIATE LANDING {SYMBOL_WARNING} ");
            
            // Fill the rest of the line
            int padding = width - 50; // Approximate content length
            if (padding > 0)
                Console.Write(new string(' ', padding));
                
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(BOX_VERTICAL);
        }
        else
        {
            Console.Write(BOX_VERTICAL);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($" Flight {flight.FlightNumber} requires landing. Please assign a runway: ");
            
            // Fill the rest of the line
            int padding = width - 55; // Approximate content length
            if (padding > 0)
                Console.Write(new string(' ', padding));
                
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(BOX_VERTICAL);
        }
        
        // Draw divider
        Console.Write(BOX_T_RIGHT);
        Console.Write(new string(BOX_HORIZONTAL, width - 2));
        Console.WriteLine(BOX_T_LEFT);
        
        // Display flight details
        Console.Write(BOX_VERTICAL);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" Flight Details: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{flight.FlightNumber} ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("(");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{flight.Type}, {flight.Priority}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(")");
        
        // Padding
        int flightDetailsPadding = width - 40; // Approximate content length
        if (flightDetailsPadding > 0)
            Console.Write(new string(' ', flightDetailsPadding));
            
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(BOX_VERTICAL);
        
        // Aircraft info
        Console.Write(BOX_VERTICAL);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" Aircraft: ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write($"{flight.Plane.PlaneID} ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("(Size: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{flight.Plane.Size}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", Required Runway Length: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{flight.Plane.RequiredRunwayLength}m");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(")");
        
        // Padding
        int aircraftPadding = width - 70; // Approximate content length
        if (aircraftPadding > 0)
            Console.Write(new string(' ', aircraftPadding));
            
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(BOX_VERTICAL);
        
        // Passenger info
        Console.Write(BOX_VERTICAL);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" Passengers: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{SYMBOL_PASSENGERS} {flight.Passengers}");
        
        // Padding
        int passengerPadding = width - 25; // Approximate content length
        if (passengerPadding > 0)
            Console.Write(new string(' ', passengerPadding));
            
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(BOX_VERTICAL);
        
        // Draw divider
        Console.Write(BOX_T_RIGHT);
        Console.Write(new string(BOX_HORIZONTAL, width - 2));
        Console.WriteLine(BOX_T_LEFT);
        
        // Display runway options header
        Console.Write(BOX_VERTICAL);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" Available Runways:");
        
        // Padding
        int runwayHeaderPadding = width - 20; // Approximate content length
        if (runwayHeaderPadding > 0)
            Console.Write(new string(' ', runwayHeaderPadding));
            
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(BOX_VERTICAL);
        
        // Display runway options
        for (int i = 0; i < availableRunways.Count; i++)
        {
            Console.Write(BOX_VERTICAL);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {i + 1}. ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{availableRunways[i].Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("(Length: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{availableRunways[i].Length}m");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(", Wear: ");
            
            // Color code wear level
            int wearLevel = availableRunways[i].WearLevel;
            if (wearLevel >= 80)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (wearLevel >= 50)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;
                
            Console.Write($"{wearLevel}%");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(")");
            
            // Padding
            int runwayPadding = width - 50; // Approximate content length
            if (runwayPadding > 0)
                Console.Write(new string(' ', runwayPadding));
                
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(BOX_VERTICAL);
        }
        
        // Only show auto-select option for non-emergency flights
        bool isEmergency = flight.Priority == FlightPriority.Emergency || flight.Type == FlightType.Emergency;
        
        // Draw divider
        Console.Write(BOX_T_RIGHT);
        Console.Write(new string(BOX_HORIZONTAL, width - 2));
        Console.WriteLine(BOX_T_LEFT);
        
        // Auto-select option
        if (!isEmergency)
        {
            Console.Write(BOX_VERTICAL);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {availableRunways.Count + 1}. ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Auto-select best runway");
            
            // Padding
            int autoPadding = width - 30; // Approximate content length
            if (autoPadding > 0)
                Console.Write(new string(' ', autoPadding));
                
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(BOX_VERTICAL);
        }
        
        // Delay option
        Console.Write(BOX_VERTICAL);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" {(isEmergency ? availableRunways.Count + 1 : availableRunways.Count + 2)}. ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Delay flight (5 ticks)");
        
        // Padding
        int delayPadding = width - 30; // Approximate content length
        if (delayPadding > 0)
            Console.Write(new string(' ', delayPadding));
            
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(BOX_VERTICAL);
        
        // Draw bottom border
        Console.Write(BOX_BOTTOM_LEFT);
        Console.Write(new string(BOX_HORIZONTAL, width - 2));
        Console.WriteLine(BOX_BOTTOM_RIGHT);
        Console.ResetColor();
        
        // Get user input
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Enter your selection: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        string input = Console.ReadLine();
        Console.ResetColor();
        
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