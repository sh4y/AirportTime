public class RunwayMetrics
{
    private readonly Airport airport;

    public RunwayMetrics(Airport airport)
    {
        this.airport = airport;
    }

    public List<RunwayInfo> GetRunwayInfo()
    {
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

    private List<Runway> GetAllRunways()
    {
        var runways = new List<Runway>();
        
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
                    runways.AddRange(runwaysList);
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
                    runways.Add(runway);
                }
            }
        }
        
        return runways;
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