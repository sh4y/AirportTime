/// <summary>
/// Tracks player achievements and unlocks rewards based on progress
/// </summary>
public class AchievementSystem
{
    private readonly Dictionary<string, int> flightTypeCounter = new Dictionary<string, int>();
    private readonly Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
    private readonly GameLogger logger;
    private readonly List<Achievement> achievements = new List<Achievement>();
    
    // Event for when an achievement is unlocked
    public event Action<Achievement> OnAchievementUnlocked;
    
    public AchievementSystem(GameLogger logger)
    {
        this.logger = logger;
        InitializeFlightTypeCounters();
        RegisterAchievements();
    }
    
    private void InitializeFlightTypeCounters()
    {
        // Initialize counters for each flight type
        foreach (FlightType flightType in Enum.GetValues(typeof(FlightType)))
        {
            flightTypeCounter[$"{flightType}"] = 0;
        }
    }
    
    private void RegisterAchievements()
    {
        // Register Flight Type Specialization achievements for each flight type
        foreach (FlightType flightType in Enum.GetValues(typeof(FlightType)))
        {
            // Tier I: 10 flights
            RegisterAchievement(new Achievement(
                $"{flightType}Specialist_I", 
                $"{flightType} Specialist I", 
                $"Land 10 {flightType} flights", 
                10, 
                AchievementType.FlightTypeSpecialization,
                flightType,
                1));
                
            // Tier II: 30 flights
            RegisterAchievement(new Achievement(
                $"{flightType}Specialist_II", 
                $"{flightType} Specialist II", 
                $"Land 30 {flightType} flights", 
                30, 
                AchievementType.FlightTypeSpecialization,
                flightType,
                2));
                
            // Tier III: 100 flights
            RegisterAchievement(new Achievement(
                $"{flightType}Specialist_III", 
                $"{flightType} Specialist III", 
                $"Land 100 {flightType} flights", 
                100, 
                AchievementType.FlightTypeSpecialization,
                flightType,
                3));
                
            // Tier IV: 500 flights
            RegisterAchievement(new Achievement(
                $"{flightType}Specialist_IV", 
                $"{flightType} Specialist IV", 
                $"Land 500 {flightType} flights", 
                500, 
                AchievementType.FlightTypeSpecialization,
                flightType,
                4));
        }
    }
    
    private void RegisterAchievement(Achievement achievement)
    {
        achievements.Add(achievement);
        unlockedAchievements[achievement.Id] = false;
    }
    
    /// <summary>
    /// Records a flight landing and updates counters
    /// </summary>
    public void RecordFlightLanded(Flight flight)
    {
        string flightType = flight.Type.ToString();
        
        // Increment counter for this flight type
        flightTypeCounter[flightType]++;
        int newCount = flightTypeCounter[flightType];
        
        // Check for achievement unlocks
        CheckFlightTypeAchievements(flight.Type, newCount);
        
        // Log the milestone if it's a nice round number
        if (newCount == 10 || newCount == 30 || newCount == 100 || 
            newCount == 500 || newCount % 1000 == 0)
        {
            logger.Log($"Milestone: {newCount} {flightType} flights landed!");
        }
    }
    
    private void CheckFlightTypeAchievements(FlightType flightType, int count)
    {
        // Find all achievements for this flight type that haven't been unlocked yet
        foreach (var achievement in achievements)
        {
            if (achievement.Type == AchievementType.FlightTypeSpecialization && 
                achievement.RelatedFlightType == flightType &&
                !unlockedAchievements[achievement.Id] &&
                count >= achievement.RequiredCount)
            {
                // Unlock the achievement
                UnlockAchievement(achievement);
            }
        }
    }
    
    private void UnlockAchievement(Achievement achievement)
    {
        unlockedAchievements[achievement.Id] = true;
        logger.Log($"🏆 Achievement Unlocked: {achievement.Name} - {achievement.Description}");
        
        // Trigger the event
        OnAchievementUnlocked?.Invoke(achievement);
    }
    
    /// <summary>
    /// Gets the count of flights landed for a specific type
    /// </summary>
    public int GetFlightTypeCount(FlightType flightType)
    {
        return flightTypeCounter[flightType.ToString()];
    }
    
    /// <summary>
    /// Gets all unlocked achievements
    /// </summary>
    public List<Achievement> GetUnlockedAchievements()
    {
        List<Achievement> result = new List<Achievement>();
        
        foreach (var achievement in achievements)
        {
            if (unlockedAchievements[achievement.Id])
            {
                result.Add(achievement);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets all achievements (locked and unlocked)
    /// </summary>
    public List<AchievementStatus> GetAllAchievements()
    {
        List<AchievementStatus> result = new List<AchievementStatus>();
        
        foreach (var achievement in achievements)
        {
            result.Add(new AchievementStatus(
                achievement,
                unlockedAchievements[achievement.Id],
                GetProgressForAchievement(achievement)
            ));
        }
        
        return result;
    }
    
    private int GetProgressForAchievement(Achievement achievement)
    {
        if (achievement.Type == AchievementType.FlightTypeSpecialization)
        {
            return flightTypeCounter[achievement.RelatedFlightType.ToString()];
        }
        
        return 0;
    }
}