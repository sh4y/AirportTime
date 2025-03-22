public class AchievementMetrics
{
    private readonly Airport airport;

    public AchievementMetrics(Airport airport)
    {
        this.airport = airport;
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