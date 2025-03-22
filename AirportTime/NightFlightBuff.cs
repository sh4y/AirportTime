using AirportTime;

public class NightFlightBuff : Item
{
    public double NightRevenueBonus { get; }
    
    public NightFlightBuff(
        int id, 
        string name, 
        string description, 
        double price, 
        double nightRevenueBonus, 
        int tier) : 
        base(id, name, description, price, ItemType.Upgrade, tier)
    {
        NightRevenueBonus = nightRevenueBonus;
    }
    
    public override void OnPurchase(Airport airport)
    {
        // Add a modifier for night flight revenue
        string modifierName = $"NightFlightBonus_Tier{ItemTier}";
        
        // Add a generic modifier for now - in a real implementation, this would be more specific to night flights
        airport.ModifierManager.AddModifier(modifierName, 1.0 + NightRevenueBonus);
        
        airport.GameLogger.Log($"✅ Added {NightRevenueBonus:P0} revenue bonus for night flights!");
    }
    
    /// <summary>
    /// Factory method to create a night flight buff from a Night Flight achievement
    /// </summary>
    public static NightFlightBuff FromAchievement(Achievement achievement, int nextItemId)
    {
        if (achievement.Type != AchievementType.NightFlight)
            throw new ArgumentException("Achievement must be of NightFlight type");
            
        // Calculate price based on tier
        double price = AchievementConfig.NightFlightBasePrice * achievement.Tier;
        
        // Each tier adds percentage from config
        double bonus = AchievementConfig.NightFlightBuffPercentage * achievement.Tier;
        
        return new NightFlightBuff(
            nextItemId,
            $"Night Operations {achievement.Tier}",
            $"Increases revenue from night flights by {bonus:P0}",
            price,
            bonus,
            achievement.Tier
        );
    }
}