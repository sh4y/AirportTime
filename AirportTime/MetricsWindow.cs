namespace AirportTime;

public static class MetricsWindow
{
    private static readonly string horizontalLine = "────────────────────────────────────────────────────────────────────────────────────";
    private static readonly string topBorder = "┌" + horizontalLine + "┐";
    private static readonly string bottomBorder = "└" + horizontalLine + "┘";
    private static readonly string middleBorder = "├" + horizontalLine + "┤";
    
    private static GameMetrics metrics;
    private static int currentPage = 0;
    private static readonly int totalPages = 4;
    
    public static void DisplayMetrics(Airport airport, int currentTick)
    {
        // Initialize metrics if needed
        if (metrics == null)
        {
            metrics = new GameMetrics(airport, airport.GameLogger);
        }
        
        bool exitMetricsWindow = false;
        
        while (!exitMetricsWindow)
        {
            Console.Clear();
            
            // Draw title
            Console.WriteLine(topBorder);
            Console.WriteLine($"│ AIRPORT METRICS DASHBOARD - {airport.Name} - Day {metrics.AirportMetrics.GetTimeInfo(currentTick).Days}                                    │");
            Console.WriteLine(middleBorder);
            
            // Draw current page content
            switch (currentPage)
            {
                case 0:
                    DrawFinancialMetrics(airport, currentTick);
                    break;
                case 1:
                    DrawFlightMetrics(airport, currentTick);
                    break;
                case 2:
                    DrawRunwayMetrics(airport, currentTick);
                    break;
                case 3:
                    DrawAchievementMetrics(airport, currentTick);
                    break;
            }
            
            // Draw navigation
            Console.WriteLine(middleBorder);
            Console.WriteLine($"│ Page {currentPage + 1}/{totalPages}: {GetPageName(currentPage)}                                                      │");
            Console.WriteLine("│ [←/→] Navigate Pages   [A] Return to Game                                         │");
            Console.WriteLine(bottomBorder);
            
            // Handle input
            var key = Console.ReadKey(true).Key;
            
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    currentPage = (currentPage + totalPages - 1) % totalPages;
                    break;
                case ConsoleKey.RightArrow:
                    currentPage = (currentPage + 1) % totalPages;
                    break;
                case ConsoleKey.A:
                    exitMetricsWindow = true;
                    break;
            }
        }
    }
    
    private static string GetPageName(int page)
    {
        return page switch
        {
            0 => "Financial Metrics",
            1 => "Flight Metrics",
            2 => "Runway Metrics",
            3 => "Achievement Metrics",
            _ => "Unknown"
        };
    }
    
    #region Metrics Pages
    
    private static void DrawFinancialMetrics(Airport airport, int currentTick)
    {
        // Calculate financial metrics
        double balance = airport.Treasury.GetBalance();
        double incomePerMinute = airport.Treasury.GoldPerTick * 6; // 6 ticks per minute at normal speed
        double incomePerHour = incomePerMinute * 60;
        
        // Get estimated runway maintenance costs
        double maintenanceCost = EstimateMaintenanceCosts(airport);
        double profit = incomePerMinute - (maintenanceCost / 60); // Divide by 60 for per-minute cost
        
        // Revenue breakdown by flight type
        var flightTypeBreakdown = GetFlightTypeRevenueBreakdown(airport);
        
        // Build historical revenue data
        var recentTransactions = GetRecentTransactions(airport);
        
        Console.WriteLine("│ FINANCIAL SUMMARY                                                                    │");
        Console.WriteLine($"│ Current Balance: ${balance,10:N0}                                                        │");
        Console.WriteLine($"│ Income Rate: ${incomePerMinute,8:N2}/min (${incomePerHour,9:N0}/hour)                                │");
        Console.WriteLine($"│ Maintenance Costs: ${maintenanceCost,8:N2}/hour                                                │");
        Console.WriteLine($"│ Estimated Net Profit: ${profit,8:N2}/min                                                    │");
        Console.WriteLine(middleBorder);
        
        Console.WriteLine("│ REVENUE BREAKDOWN BY FLIGHT TYPE                                                     │");
        foreach (var entry in flightTypeBreakdown)
        {
            Console.WriteLine($"│ {entry.Key,-12}: ${entry.Value,8:N0} ({entry.Value * 100 / flightTypeBreakdown.Values.Sum(),5:N1}%)                                         │");
        }
        Console.WriteLine(middleBorder);
        
        Console.WriteLine("│ RECENT TRANSACTIONS                                                                  │");
        foreach (var transaction in recentTransactions)
        {
            string type = transaction.TransactionType == TransactionType.Add ? "+" : "-";
            Console.WriteLine($"│ {type}${Math.Abs(transaction.Amount),8:N0} | {transaction.SourceOrReason,-52} │");
        }
        
        // Fill remaining space
        for (int i = 0; i < 5 - recentTransactions.Count; i++)
        {
            Console.WriteLine("│                                                                                      │");
        }
    }
    
    private static void DrawFlightMetrics(Airport airport, int currentTick)
    {
        // Get flight related metrics
        var activeFlights = airport.FlightScheduler.GetUnlandedFlights().Count;
        var flightsByType = GetFlightsByType(airport);
        var flightStatistics = GetFlightStatistics(airport);
        
        Console.WriteLine("│ FLIGHT OVERVIEW                                                                      │");
        Console.WriteLine($"│ Active Flights: {activeFlights,3}                                                               │");
        Console.WriteLine($"│ Total Flights Landed: {flightStatistics.TotalFlights,6}                                                      │");
        Console.WriteLine($"│ Total Passengers Served: {flightStatistics.TotalPassengers,8:N0}                                                │");
        Console.WriteLine($"│ Average Passengers per Flight: {flightStatistics.AvgPassengersPerFlight,6:N1}                                           │");
        Console.WriteLine(middleBorder);
        
        Console.WriteLine("│ FLIGHTS BY TYPE                                                                      │");
        foreach (var entry in flightsByType)
        {
            double percentage = flightsByType.Values.Sum() > 0 
                ? entry.Value * 100.0 / flightsByType.Values.Sum() 
                : 0;
                
            Console.WriteLine($"│ {entry.Key,-12}: {entry.Value,5} ({percentage,5:N1}%)                                                 │");
        }
        Console.WriteLine(middleBorder);
        
        Console.WriteLine("│ FLIGHT PERFORMANCE                                                                   │");
        Console.WriteLine($"│ On-Time Performance: {flightStatistics.OnTimePerformance,6:P1}                                                    │");
        Console.WriteLine($"│ Perfect Landings: {flightStatistics.PerfectLandings,6}                                                        │");
        Console.WriteLine($"│ Emergency Landings: {flightStatistics.EmergencyLandings,6}                                                     │");
        Console.WriteLine($"│ Flight Cancellation Rate: {flightStatistics.CancellationRate,6:P1}                                              │");
        Console.WriteLine($"│ Average Delay: {flightStatistics.AvgDelay,6:N1} ticks                                                   │");
        
        // Fill remaining space
        for (int i = 0; i < 8; i++)
        {
            Console.WriteLine("│                                                                                      │");
        }
    }
    
    private static void DrawRunwayMetrics(Airport airport, int currentTick)
    {
        var runwayInfo = metrics.RunwayMetrics.GetRunwayInfo();
        var runwayStats = GetRunwayStatistics(airport);
        
        Console.WriteLine("│ RUNWAY OVERVIEW                                                                      │");
        Console.WriteLine($"│ Total Runways: {runwayInfo.Count,2}                                                                    │");
        Console.WriteLine($"│ Average Wear Level: {runwayStats.AvgWear,5:N1}%                                                        │");
        Console.WriteLine($"│ Runway Utilization Rate: {runwayStats.UtilizationRate,5:P1}                                                 │");
        Console.WriteLine($"│ Average Landing Time: {runwayStats.AvgLandingTime,2} ticks                                                     │");
        Console.WriteLine(middleBorder);
        
        Console.WriteLine("│ RUNWAY STATUS                                                                        │");
        Console.WriteLine("│ Name          | Type     | Length | Wear% | Landings | Utilization                   │");
        Console.WriteLine("│ --------------+----------+--------+-------+----------+------------------------------- │");
        
        foreach (var runway in runwayInfo)
        {
            var stats = runwayStats.RunwayUsage.GetValueOrDefault(runway.Name, new RunwayUsageInfo { TotalLandings = 0, UtilizationRate = 0 });
            Console.WriteLine($"│ {runway.Name,-12} | {runway.Type,-8} | {runway.Length,6} | {runway.WearLevel,5}% | {stats.TotalLandings,8} | {stats.UtilizationRate,5:P1}                       │");
        }
        
        for (int i = 0; i < 4 - runwayInfo.Count; i++)
        {
            Console.WriteLine("│                                                                                      │");
        }
        
        Console.WriteLine(middleBorder);
        Console.WriteLine("│ MAINTENANCE HISTORY                                                                  │");
        Console.WriteLine($"│ Total Maintenance Performed: {runwayStats.TotalMaintenancePerformed,4}                                                 │");
        Console.WriteLine($"│ Average Repair Cost: ${runwayStats.AvgRepairCost,8:N0}                                                  │");
        
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("│                                                                                      │");
        }
    }
    
    private static void DrawAchievementMetrics(Airport airport, int currentTick)
    {
        var achievements = airport.AchievementSystem.GetAllAchievements();
        var achievementsByType = achievements.GroupBy(a => a.Achievement.Type);
        
        int totalAchievements = achievements.Count;
        int unlockedAchievements = achievements.Count(a => a.IsUnlocked);
        double completionRate = totalAchievements > 0 ? (double)unlockedAchievements / totalAchievements : 0;
        
        Console.WriteLine("│ ACHIEVEMENT PROGRESS                                                                 │");
        Console.WriteLine($"│ Unlocked: {unlockedAchievements,3}/{totalAchievements,-3} ({completionRate,6:P1} complete)                                           │");
        Console.WriteLine(middleBorder);
        
        Console.WriteLine("│ ACHIEVEMENT CATEGORIES                                                               │");
        
        foreach (var group in achievementsByType)
        {
            int typeUnlocked = group.Count(a => a.IsUnlocked);
            int typeTotal = group.Count();
            double typeCompletion = (double)typeUnlocked / typeTotal;
            
            Console.WriteLine($"│ {group.Key,-25}: {typeUnlocked,2}/{typeTotal,-2} ({typeCompletion,6:P1})                                    │");
            
            // Draw progress bar for each group (40 chars wide)
            int barWidth = 40;
            int filled = (int)(typeCompletion * barWidth);
            Console.Write("│ [");
            Console.ForegroundColor = GetProgressColor((int)(typeCompletion * 100));
            Console.Write(new string('█', filled));
            Console.ResetColor();
            Console.Write(new string('░', barWidth - filled));
            Console.WriteLine("] │");
        }
        
        // Fill remaining space
        for (int i = 0; i < 16 - (achievementsByType.Count() * 2); i++)
        {
            Console.WriteLine("│                                                                                      │");
        }
    }
    
    #endregion
    
    #region Helper Methods
    
    private static ConsoleColor GetProgressColor(int percentage)
    {
        if (percentage < 30) return ConsoleColor.Red;
        if (percentage < 70) return ConsoleColor.Yellow;
        return ConsoleColor.Green;
    }
    
    private static double EstimateMaintenanceCosts(Airport airport)
    {
        double totalCost = 0;
        var runways = metrics.RunwayMetrics.GetRunwayInfo();
        
        foreach (var runway in runways)
        {
            // Estimate cost based on wear level
            double wearRate = 4.0; // Average wear per hour
            double costPerWear = 11.0; // Cost multiplier per wear point
            
            totalCost += wearRate * costPerWear;
        }
        
        return totalCost;
    }
    
    private static Dictionary<string, double> GetFlightTypeRevenueBreakdown(Airport airport)
    {
        // This would ideally come from actual game data, but we'll simulate it for now
        var result = new Dictionary<string, double>();
        
        foreach (FlightType type in Enum.GetValues(typeof(FlightType)))
        {
            // Simulate revenue values based on flight types
            double revenue = type switch
            {
                FlightType.Commercial => 15000,
                FlightType.Cargo => 8000,
                FlightType.VIP => 12000,
                FlightType.Emergency => 5000,
                _ => 0
            };
            
            result[type.ToString()] = revenue;
        }
        
        return result;
    }
    
    private static List<TreasuryTransaction> GetRecentTransactions(Airport airport)
    {
        // This should actually query from the transaction log store
        // For now, return simulated data
        var transactions = new List<TreasuryTransaction>
        {
            new TreasuryTransaction { 
                TransactionType = TransactionType.Add, 
                Amount = 1250, 
                SourceOrReason = "Flight AA123 (Commercial)" 
            },
            new TreasuryTransaction { 
                TransactionType = TransactionType.Add, 
                Amount = 2340, 
                SourceOrReason = "Flight DL456 (VIP)" 
            },
            new TreasuryTransaction { 
                TransactionType = TransactionType.Deduct, 
                Amount = 450, 
                SourceOrReason = "Runway Maintenance (Small Runway)" 
            },
            new TreasuryTransaction { 
                TransactionType = TransactionType.Add, 
                Amount = 890, 
                SourceOrReason = "Flight UPS789 (Cargo)" 
            },
            new TreasuryTransaction { 
                TransactionType = TransactionType.Add, 
                Amount = 1500, 
                SourceOrReason = "Level Up Bonus (Level 3)" 
            }
        };
        
        return transactions;
    }
    
    private static Dictionary<string, int> GetFlightsByType(Airport airport)
    {
        // Simulated data for flights by type
        return new Dictionary<string, int>
        {
            { "Commercial", 250 },
            { "Cargo", 85 },
            { "VIP", 42 },
            { "Emergency", 18 }
        };
    }
    
    private static FlightStatisticsInfo GetFlightStatistics(Airport airport)
    {
        // Simulated flight statistics
        return new FlightStatisticsInfo
        {
            TotalFlights = 395,
            TotalPassengers = 47600,
            AvgPassengersPerFlight = 120.5,
            OnTimePerformance = 0.82,
            PerfectLandings = 210,
            EmergencyLandings = 18,
            CancellationRate = 0.05,
            AvgDelay = 3.2
        };
    }
    
    private static RunwayStatisticsInfo GetRunwayStatistics(Airport airport)
    {
        var runways = metrics.RunwayMetrics.GetRunwayInfo();
        
        // Simulated runway statistics
        var stats = new RunwayStatisticsInfo
        {
            AvgWear = runways.Average(r => r.WearLevel),
            UtilizationRate = 0.65,
            AvgLandingTime = 8,
            TotalMaintenancePerformed = 42,
            AvgRepairCost = 230,
            RunwayUsage = new Dictionary<string, RunwayUsageInfo>()
        };
        
        // Generate runway usage info for each runway
        foreach (var runway in runways)
        {
            stats.RunwayUsage[runway.Name] = new RunwayUsageInfo
            {
                TotalLandings = runway.Name switch
                {
                    "Small Runway" => 180,
                    "Small Runway2" => 120,
                    "Medium Runway" => 85,
                    "Large Runway" => 10,
                    _ => 0
                },
                UtilizationRate = runway.Name switch
                {
                    "Small Runway" => 0.75,
                    "Small Runway2" => 0.62,
                    "Medium Runway" => 0.48,
                    "Large Runway" => 0.25,
                    _ => 0
                }
            };
        }
        
        return stats;
    }
    
    #endregion
    
    #region Metric Data Structures
    
    private class FlightStatisticsInfo
    {
        public int TotalFlights { get; set; }
        public int TotalPassengers { get; set; }
        public double AvgPassengersPerFlight { get; set; }
        public double OnTimePerformance { get; set; }
        public int PerfectLandings { get; set; }
        public int EmergencyLandings { get; set; }
        public double CancellationRate { get; set; }
        public double AvgDelay { get; set; }
    }
    
    private class RunwayStatisticsInfo
    {
        public double AvgWear { get; set; }
        public double UtilizationRate { get; set; }
        public int AvgLandingTime { get; set; }
        public int TotalMaintenancePerformed { get; set; }
        public double AvgRepairCost { get; set; }
        public Dictionary<string, RunwayUsageInfo> RunwayUsage { get; set; }
    }
    
    private class RunwayUsageInfo
    {
        public int TotalLandings { get; set; }
        public double UtilizationRate { get; set; }
    }
    
    #endregion
}