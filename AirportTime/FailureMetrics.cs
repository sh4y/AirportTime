public class FailureMetrics
{
    private readonly Airport airport;
    private readonly Dictionary<FailureType, List<DateTime>> failureTimestamps = new Dictionary<FailureType, List<DateTime>>();

    public FailureMetrics(Airport airport)
    {
        this.airport = airport;
        InitializeFailureTracking();
    }
    
    private void InitializeFailureTracking()
    {
        foreach (FailureType type in Enum.GetValues(typeof(FailureType)))
        {
            failureTimestamps[type] = new List<DateTime>();
        }
    }
    
    public void RecordFailure(FailureType type)
    {
        failureTimestamps[type].Add(DateTime.Now);
    }
    
    // Enhanced methods
    public Dictionary<FailureType, int> GetAllFailureCounts() => 
        failureTimestamps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
        
    public Dictionary<FailureType, List<DateTime>> GetFailureTimeline() => 
        new Dictionary<FailureType, List<DateTime>>(failureTimestamps);
        
    public Dictionary<FailureType, double> GetDailyFailureRates()
    {
        var result = new Dictionary<FailureType, double>();
        DateTime yesterday = DateTime.Now.AddDays(-1);
        
        foreach (var type in failureTimestamps.Keys)
        {
            int recentCount = failureTimestamps[type].Count(dt => dt >= yesterday);
            result[type] = recentCount;
        }
        
        return result;
    }
    
    public FailureType MostCommonFailure => 
        failureTimestamps.OrderByDescending(kvp => kvp.Value.Count).First().Key;
        
    public TimeSpan AverageTimeBetweenFailures(FailureType type)
    {
        var timestamps = failureTimestamps[type].OrderBy(dt => dt).ToList();
        if (timestamps.Count < 2)
            return TimeSpan.Zero;
            
        double totalSeconds = 0;
        for (int i = 1; i < timestamps.Count; i++)
        {
            totalSeconds += (timestamps[i] - timestamps[i-1]).TotalSeconds;
        }
        
        return TimeSpan.FromSeconds(totalSeconds / (timestamps.Count - 1));
    }

    public List<FailureInfo> GetTopFailures(int count = 3)
    {
        var failureTypes = Enum.GetValues(typeof(FailureType))
            .Cast<FailureType>()
            .OrderByDescending(f => airport.FailureTracker.GetFailurePercentage(f))
            .Take(count);

        return failureTypes.Select(type => new FailureInfo
        {
            Type = type,
            Count = airport.FailureTracker.GetFailureCount(type),
            Threshold = airport.FailureTracker.GetFailureThreshold(type),
            Percentage = airport.FailureTracker.GetFailurePercentage(type),
            DangerLevel = GetDangerLevel(airport.FailureTracker.GetFailurePercentage(type))
        }).ToList();
    }

    private string GetDangerLevel(int percentage)
    {
        return percentage switch
        {
            >= 90 => "CRITICAL",
            >= 75 => "High",
            >= 50 => "Moderate",
            >= 25 => "Low",
            _ => "Minimal"
        };
    }
}