namespace AirportTime;

/// <summary>
/// Represents a buff that increases revenue for a specific flight type
/// </summary>
public class FlightSpecializationBuff : Item
{
    public FlightType FlightType { get; }
    public double RevenueMultiplier { get; }
    
    public FlightSpecializationBuff(
        int id, 
        string name, 
        string description, 
        double price, 
        FlightType flightType, 
        double revenueMultiplier, 
        int tier) : 
        base(id, name, description, price, ItemType.Upgrade, tier)
    {
        FlightType = flightType;
        RevenueMultiplier = revenueMultiplier;
    }
    
    public override void OnPurchase(Airport airport)
    {
        // Add a modifier for this flight type with a descriptive name
        string modifierName = $"{FlightType}Specialist_Tier{ItemTier}";
        
        // This will be implemented in the updated ModifierManager
        airport.ModifierManager.AddFlightTypeModifier(FlightType, RevenueMultiplier, modifierName);
        
        airport.GameLogger.Log($"✅ Added {(RevenueMultiplier - 1.0) * 100:F0}% revenue bonus for {FlightType} flights!");
    }
    
    /// <summary>
    /// Factory method to create a flight specialization buff from an achievement
    /// </summary>
    public static FlightSpecializationBuff FromAchievement(Achievement achievement, int nextItemId)
    {
        if (achievement.Type != AchievementType.FlightTypeSpecialization)
            throw new ArgumentException("Achievement must be of FlightTypeSpecialization type");
            
        // Calculate price based on tier
        double price = achievement.Tier * 1000 * (int)achievement.RelatedFlightType;
        
        // Each tier adds 10%
        double multiplier = 1.0 + (0.1 * achievement.Tier);
        
        return new FlightSpecializationBuff(
            nextItemId,
            $"{achievement.RelatedFlightType} Specialist {achievement.Tier}",
            $"Increases {achievement.RelatedFlightType} flight revenue by {(multiplier - 1.0) * 100:F0}%",
            price,
            achievement.RelatedFlightType,
            multiplier,
            achievement.Tier
        );
    }
}