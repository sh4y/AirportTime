public class AchievementMetrics
{
    private readonly Airport airport;

    public AchievementMetrics(Airport airport)
    {
        this.airport = airport;
    }
    
    public Dictionary<AchievementType, int> GetAchievementCounts()
    {
        var achievements = airport.AchievementSystem.GetAllAchievements();
        return achievements
            .GroupBy(a => a.Achievement.Type)
            .ToDictionary(g => g.Key, g => g.Count(a => a.IsUnlocked));
    }
    
    public Dictionary<AchievementType, double> GetAchievementCompletionRate()
    {
        var achievements = airport.AchievementSystem.GetAllAchievements();
        return achievements
            .GroupBy(a => a.Achievement.Type)
            .ToDictionary(
                g => g.Key, 
                g => g.Any() ? (double)g.Count(a => a.IsUnlocked) / g.Count() : 0);
    }
    
    public List<AchievementStatus> GetInProgressAchievements(int count = 5)
    {
        return airport.AchievementSystem.GetAllAchievements()
            .Where(a => !a.IsUnlocked)
            .OrderByDescending(a => a.ProgressPercentage)
            .Take(count)
            .ToList();
    }
    
    public Dictionary<FlightType, int> GetFlightTypeCompletionCounts()
    {
        var result = new Dictionary<FlightType, int>();
        foreach (FlightType type in Enum.GetValues(typeof(FlightType)))
        {
            result[type] = airport.AchievementSystem.GetFlightTypeCount(type);
        }
        return result;
    }

    public List<Achievement> GetRecentAchievements(int count = 3)
    {
        var allAchievements = airport.AchievementSystem.GetUnlockedAchievements();
        
        if (allAchievements.Count == 0)
        {
            return new List<Achievement>();
        }
        
        // Get most recent achievements
        int startIndex = Math.Max(0, allAchievements.Count - count);
        return allAchievements.Skip(startIndex).ToList();
    }

    public int TotalUnlockedAchievements => airport.AchievementSystem.GetUnlockedAchievements().Count;
}