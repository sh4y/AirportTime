/// <summary>
/// Represents a buff that increases XP earned from flights
/// </summary>
public class XPBuff : Item
{
    public double XPMultiplier { get; }
    
    public XPBuff(
        int id, 
        string name, 
        string description, 
        double price, 
        double xpMultiplier, 
        int tier) : 
        base(id, name, description, price, ItemType.Upgrade, tier)
    {
        XPMultiplier = xpMultiplier;
    }
    
    public override void OnPurchase(Airport airport)
    {
        // Add XP multiplier to the experience system
        airport.ExperienceSystem.AddXPMultiplier(XPMultiplier);
        
        airport.GameLogger.Log($"✅ Added {(XPMultiplier - 1.0) * 100:F0}% XP bonus for all flights!");
    }
    
    /// <summary>
    /// Factory method to create an XP buff from a Perfect Pilot achievement
    /// </summary>
    public static XPBuff FromAchievement(Achievement achievement, int nextItemId)
    {
        if (achievement.Type != AchievementType.PerfectLandings)
            throw new ArgumentException("Achievement must be of PerfectLandings type");
            
        // Calculate price based on tier
        double price = achievement.Tier * 2000;
        
        // Each tier adds 10%
        double multiplier = 1.0 + (0.1 * achievement.Tier);
        
        return new XPBuff(
            nextItemId,
            $"Perfect Pilot {achievement.Tier}",
            $"Increases XP earned from all flights by {(multiplier - 1.0) * 100:F0}%",
            price,
            multiplier,
            achievement.Tier
        );
    }
}
