/// <summary>
/// Represents a buff that increases gold income from all sources
/// </summary>
public class GoldIncomeBuff : Item
{
    public double GoldMultiplier { get; }
    
    public GoldIncomeBuff(
        int id, 
        string name, 
        string description, 
        double price, 
        double goldMultiplier, 
        int tier) : 
        base(id, name, description, price, ItemType.Upgrade, tier)
    {
        GoldMultiplier = goldMultiplier;
    }
    
    public override void OnPurchase(Airport airport)
    {
        // Add gold income modifier to all flights
        airport.ModifierManager.AddModifier($"PassengerBonus_Tier{ItemTier}", GoldMultiplier);
        
        airport.GameLogger.Log($"✅ Added {(GoldMultiplier - 1.0) * 100:F0}% gold income bonus for all flights!");
    }
    
    /// <summary>
    /// Factory method to create a gold income buff from a Passenger Milestone achievement
    /// </summary>
    public static GoldIncomeBuff FromAchievement(Achievement achievement, int nextItemId)
    {
        if (achievement.Type != AchievementType.PassengerMilestone)
            throw new ArgumentException("Achievement must be of PassengerMilestone type");
            
        // Calculate price based on tier
        double price = achievement.Tier * 5000;
        
        // Each tier adds 5%
        double multiplier = 1.0 + (0.05 * achievement.Tier);
        
        return new GoldIncomeBuff(
            nextItemId,
            achievement.Name,
            $"Increases gold earned from all flights by {(multiplier - 1.0) * 100:F0}%",
            price,
            multiplier,
            achievement.Tier
        );
    }
}