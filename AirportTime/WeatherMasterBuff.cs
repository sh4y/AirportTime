using AirportTime;

public class WeatherMasterBuff : Item
{
    public WeatherType WeatherType { get; }
    public double WeatherResistance { get; }
    
    public WeatherMasterBuff(
        int id, 
        string name, 
        string description, 
        double price, 
        WeatherType weatherType, 
        double weatherResistance, 
        int tier) : 
        base(id, name, description, price, ItemType.Upgrade, tier)
    {
        WeatherType = weatherType;
        WeatherResistance = weatherResistance;
    }
    
    public override void OnPurchase(Airport airport)
    {
        // Add weather resistance to runways
        airport.RunwayManager.AddWeatherResistance(WeatherResistance);
        
        airport.GameLogger.Log($"✅ Added {WeatherResistance:P0} weather resistance for {WeatherType} weather!");
    }
    
    /// <summary>
    /// Factory method to create a weather resistance buff from a Weather Master achievement
    /// </summary>
    public static WeatherMasterBuff FromAchievement(Achievement achievement, int nextItemId)
    {
        if (achievement.Type != AchievementType.WeatherMaster)
            throw new ArgumentException("Achievement must be of WeatherMaster type");
            
        // Determine the weather type from the achievement name
        WeatherType weatherType = WeatherType.Clear;
        foreach (WeatherType type in Enum.GetValues(typeof(WeatherType)))
        {
            if (achievement.Name.Contains(type.ToString()))
            {
                weatherType = type;
                break;
            }
        }
        
        // Calculate price based on tier and weather severity
        double weatherSeverityFactor = weatherType switch
        {
            WeatherType.Rainy => 1.0,
            WeatherType.Foggy => 1.5,
            WeatherType.Snowy => 2.0,
            WeatherType.Stormy => 3.0,
            _ => 1.0
        };
        
        double price = AchievementConfig.WeatherMasterBasePrice * achievement.Tier * weatherSeverityFactor;
        
        // Each tier adds percentage from config
        double resistance = AchievementConfig.WeatherMasterBuffPercentage * achievement.Tier;
        
        return new WeatherMasterBuff(
            nextItemId,
            $"{weatherType} Resistance {achievement.Tier}",
            $"Reduces runway wear from {weatherType} weather by {resistance:P0}",
            price,
            weatherType,
            resistance,
            achievement.Tier
        );
    }
}