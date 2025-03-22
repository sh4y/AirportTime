public class FailureMetrics
{
    private readonly Airport airport;

    public FailureMetrics(Airport airport)
    {
        this.airport = airport;
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