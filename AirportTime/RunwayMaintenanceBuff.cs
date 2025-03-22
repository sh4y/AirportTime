public class RunwayMaintenanceBuff : Item
{
    public double MaintenanceTimeReduction { get; }
    
    public RunwayMaintenanceBuff(
        int id, 
        string name, 
        string description, 
        double price, 
        double maintenanceTimeReduction, 
        int tier) : 
        base(id, name, description, price, ItemType.Upgrade, tier)
    {
        MaintenanceTimeReduction = maintenanceTimeReduction;
    }
    
    public override void OnPurchase(Airport airport)
    {
        // Reduce maintenance time for all runways
        airport.RunwayManager.ReduceMaintenanceTime(MaintenanceTimeReduction);
        
        airport.GameLogger.Log($"✅ Reduced runway maintenance time by {(1.0 - MaintenanceTimeReduction) * 100:F0}%!");
    }
    
    /// <summary>
    /// Factory method to create a maintenance time reduction buff from a Runway Expert achievement
    /// </summary>
    public static RunwayMaintenanceBuff FromAchievement(Achievement achievement, int nextItemId)
    {
        if (achievement.Type != AchievementType.RunwayExpert)
            throw new ArgumentException("Achievement must be of RunwayExpert type");
            
        // Calculate price based on tier
        double price = AchievementConfig.RunwayExpertBasePrice * achievement.Tier;
        
        // Each tier reduces by percentage from config
        double reduction = 1.0 - (AchievementConfig.RunwayExpertBuffPercentage * achievement.Tier);
        
        return new RunwayMaintenanceBuff(
            nextItemId,
            $"Runway Expert {achievement.Tier}",
            $"Reduces runway maintenance time by {(1.0 - reduction) * 100:F0}%",
            price,
            reduction,
            achievement.Tier
        );
    }
}