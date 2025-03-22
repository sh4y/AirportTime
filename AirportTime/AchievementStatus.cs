using AirportTime;

/// <summary>
/// Represents an achievement with its unlock status and progress
/// </summary>
public class AchievementStatus
{
    public Achievement Achievement { get; }
    public bool IsUnlocked { get; }
    public int CurrentProgress { get; }
    public int RequiredProgress => Achievement.RequiredCount;
    public double ProgressPercentage => Math.Min(100, (double)CurrentProgress / RequiredProgress * 100);
    
    public AchievementStatus(Achievement achievement, bool isUnlocked, int currentProgress)
    {
        Achievement = achievement;
        IsUnlocked = isUnlocked;
        CurrentProgress = currentProgress;
    }
}