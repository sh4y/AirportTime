namespace AirportTime;

public class RunwayMetrics
{
    private readonly Airport airport;
    private readonly Dictionary<string, int> runwayLandings = new Dictionary<string, int>();
    private readonly Dictionary<string, int> runwayMaintenance = new Dictionary<string, int>();
    private readonly Dictionary<string, double> runwayRevenue = new Dictionary<string, double>();

    public RunwayMetrics(Airport airport)
    {
        this.airport = airport;
    }
    
    public void RecordLanding(string runwayName, double revenue)
    {
        if (!runwayLandings.ContainsKey(runwayName))
            runwayLandings[runwayName] = 0;
        if (!runwayRevenue.ContainsKey(runwayName))
            runwayRevenue[runwayName] = 0;
            
        runwayLandings[runwayName]++;
        runwayRevenue[runwayName] += revenue;
    }
    
    public void RecordMaintenance(string runwayName, double cost)
    {
        if (!runwayMaintenance.ContainsKey(runwayName))
            runwayMaintenance[runwayName] = 0;
            
        runwayMaintenance[runwayName]++;
    }
    
    // Enhanced methods
    public Dictionary<string, int> GetRunwayUsageStats() => new Dictionary<string, int>(runwayLandings);
    
    public Dictionary<string, double> GetRunwayEfficiency() => 
        runwayRevenue.ToDictionary(
            kvp => kvp.Key, 
            kvp => runwayLandings.ContainsKey(kvp.Key) && runwayLandings[kvp.Key] > 0 ? 
                kvp.Value / runwayLandings[kvp.Key] : 0);
                   
    public double GetMaintenanceFrequency(string runwayName) => 
        runwayLandings.ContainsKey(runwayName) && runwayMaintenance.ContainsKey(runwayName) && 
        runwayMaintenance[runwayName] > 0 ? 
            (double)runwayLandings[runwayName] / runwayMaintenance[runwayName] : 0;
        
    public Dictionary<string, double> GetRunwayProfitability() 
    {
        var result = new Dictionary<string, double>();
        foreach (var runway in runwayRevenue.Keys)
        {
            double maintenance = runwayMaintenance.ContainsKey(runway) ? runwayMaintenance[runway] * 200 : 0; // Estimate
            result[runway] = runwayRevenue[runway] - maintenance;
        }
        return result;
    }

    public List<RunwayInfo> GetRunwayInfo()
    {
        // Get runways directly using the IRunwayProvider interface
        var runways = GetAllRunways();
        var result = new List<RunwayInfo>();

        foreach (var runway in runways)
        {
            var info = new RunwayInfo
            {
                Name = runway.Name,
                Type = DetermineRunwayType(runway),
                Length = runway.Length,
                WearLevel = airport.RunwayManager.GetMaintenanceSystem().GetWearLevel(runway.Name),
                IsOccupied = runway.IsOccupied,
                OccupiedCountdown = runway.OccupiedCountdown,
                DetailedStatus = runway.GetDetailedOccupationStatus()
            };
            result.Add(info);
        }

        return result;
    }

    /// <summary>
    /// Gets all runways from the airport
    /// </summary>
    private List<Runway> GetAllRunways()
    {
        // First try to use the IRunwayProvider interface
        if (airport.RunwayManager is IRunwayProvider provider)
        {
            return provider.GetRunways();
        }
        
        // Fall back to reflection or known runway names if provider is not available
        var results = new List<Runway>();
        
        // Try to access runways through reflection
        try
        {
            var fieldInfo = typeof(RunwayManager).GetField("runways", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (fieldInfo != null)
            {
                var runwaysList = fieldInfo.GetValue(airport.RunwayManager) as List<Runway>;
                if (runwaysList != null)
                {
                    results.AddRange(runwaysList);
                }
            }
        }
        catch
        {
            // Fallback to known runway names if reflection fails
            string[] knownRunways = { "Small Runway", "Small Runway2", "Medium Runway", "Large Runway" };
            foreach (var name in knownRunways)
            {
                var runway = airport.RunwayManager.GetRunwayByName(name);
                if (runway != null)
                {
                    results.Add(runway);
                }
            }
        }
        
        return results;
    }

    private string DetermineRunwayType(Runway runway)
    {
        // Determine runway type based on length or class
        if (runway is SmallRunway) return "Small";
        if (runway is MediumRunway) return "Medium";
        if (runway is LargeRunway) return "Large";
        
        // Fallback to length-based determination
        return runway.Length switch
        {
            <= 5000 => "Small",
            <= 7500 => "Medium",
            _ => "Large"
        };
    }
}