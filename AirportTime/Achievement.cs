namespace AirportTime;

/// <summary>
/// Represents a single achievement that can be unlocked
/// </summary>
public class Achievement
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public int RequiredCount { get; }
    public AchievementType Type { get; }
    public FlightType RelatedFlightType { get; }
    public int Tier { get; }
    
    public Achievement(
        string id, 
        string name, 
        string description, 
        int requiredCount, 
        AchievementType type,
        FlightType relatedFlightType = FlightType.Commercial,
        int tier = 1)
    {
        Id = id;
        Name = name;
        Description = description;
        RequiredCount = requiredCount;
        Type = type;
        RelatedFlightType = relatedFlightType;
        Tier = tier;
    }
}