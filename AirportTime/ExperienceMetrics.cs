public class ExperienceMetrics
{
    private readonly Airport airport;

    public ExperienceMetrics(Airport airport)
    {
        this.airport = airport;
    }

    public int CurrentLevel => airport.ExperienceSystem.CurrentLevel;
    public int CurrentXP => airport.ExperienceSystem.CurrentXP;
    public int NextLevelXP => airport.ExperienceSystem.GetRequiredXPForNextLevel();
    public int XPNeeded => NextLevelXP - CurrentXP;
    public int ProgressPercentage => airport.ExperienceSystem.GetLevelProgressPercentage();
    public int EfficiencyBonus => (CurrentLevel * 5); // 5% per level

    public string GetEstimatedTimeToNextLevel()
    {
        int flightsNeeded = (int)Math.Ceiling(XPNeeded / 40.0);
        int estimatedTicks = flightsNeeded * 12;
        return estimatedTicks > 1000 ? "a long time" : $"~{estimatedTicks} ticks";
    }

    public string GetNextUnlock()
    {
        return CurrentLevel switch
        {
            1 => "Medium Runway (Level 2)",
            2 => "Runway Speed Upgrade (L3)",
            3 => "Income Boost (Level 4)",
            4 => "Large Runway (Level 5)",
            _ => "Various Upgrades"
        };
    }
}