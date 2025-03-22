// AchievementShopHandler.cs
public class AchievementShopHandler
{
    private readonly ShopInventory inventory;
    private readonly GameLogger logger;

    public AchievementShopHandler(ShopInventory inventory, GameLogger logger)
    {
        this.inventory = inventory;
        this.logger = logger;
    }

    /// <summary>
    /// Initialize achievement handling
    /// </summary>
    public void InitializeAchievementHandling(Airport airport)
    {
        if (airport == null) return;
    
        airport.AchievementSystem.OnAchievementUnlocked += (achievement) => 
        {
            // Get the next item ID
            int itemId = inventory.GetNextItemId();
            IPurchasable buff = null;
        
            // Create the appropriate buff based on achievement type
            switch (achievement.Type)
            {
                case AchievementType.FlightTypeSpecialization:
                    buff = FlightSpecializationBuff.FromAchievement(achievement, itemId);
                    break;
                
                case AchievementType.PerfectLandings:
                    buff = XPBuff.FromAchievement(achievement, itemId);
                    break;
                
                case AchievementType.RunwayExpert:
                    buff = RunwayMaintenanceBuff.FromAchievement(achievement, itemId);
                    break;
                
                case AchievementType.PassengerMilestone:
                    buff = GoldIncomeBuff.FromAchievement(achievement, itemId);
                    break;
                    
                case AchievementType.WeatherMaster:
                    buff = WeatherMasterBuff.FromAchievement(achievement, itemId);
                    break;
                    
                case AchievementType.NightFlight:
                    buff = NightFlightBuff.FromAchievement(achievement, itemId);
                    break;
                    
                case AchievementType.ConsecutiveFlights:
                    // Create a specialized buff for consecutive flights
                    buff = new XPBuff(
                        itemId,
                        $"Air Traffic Mastery {achievement.Tier}",
                        $"Increases XP earned from all flights by {achievement.Tier * 7.5:F1}%",
                        achievement.Tier * 2500,
                        1.0 + (achievement.Tier * 0.075),
                        achievement.Tier
                    );
                    break;
                    
                case AchievementType.SimultaneousFlights:
                    // Create a capacity-focused buff
                    buff = new RunwayBuff(
                        itemId,
                        $"Air Traffic Efficiency {achievement.Tier}",
                        $"Reduces landing duration by {achievement.Tier * 5:F0}%",
                        achievement.Tier * 3500,
                        BuffType.LandingDurationReduction,
                        1.0 - (achievement.Tier * 0.05),
                        achievement.Tier
                    );
                    break;
                    
                case AchievementType.EmergencyLandings:
                    // Create an emergency response buff
                    buff = new FlightSpecializationBuff(
                        itemId,
                        $"Emergency Response {achievement.Tier}",
                        $"Increases Emergency flight revenue by {achievement.Tier * 15:F0}%",
                        achievement.Tier * 4000,
                        FlightType.Emergency,
                        1.0 + (achievement.Tier * 0.15),
                        achievement.Tier
                    );
                    break;
            }
            
            // Add the buff to the shop if one was created
            if (buff != null)
            {
                inventory.AddItem(buff);
                logger.Log($"New achievement buff added to shop: {buff.Name} - {buff.Description} - Price: {buff.Price:C}");
            }
        };
    }
}