using AirportTime;

public class RunwayBuff : Item
{
    private readonly double buffValue;
    public BuffType BuffType { get; private set; }

    public RunwayBuff(int id, string name, string description, double price, BuffType buffType, double buffValue, int tier = 1, int availability=1) : 
        base(id, name, description, price, ItemType.Upgrade, tier, availability)
    {
        this.Id = id;
        this.BuffType = buffType;
        this.buffValue = buffValue;
    }
    
    public override void OnPurchase(Airport airport)
    {
        switch (BuffType)
        {
            case BuffType.WeatherResistance:
                airport.RunwayManager.AddWeatherResistance(buffValue);
                airport.GameLogger.Log($"Applied {buffValue:P0} weather resistance to all runways.");
                break;
                
            case BuffType.LandingDurationReduction:
                // Since we can't access runways directly, implement a method in RunwayManager
                airport.RunwayManager.ReduceAllRunwayLandingDurations(buffValue);
                airport.GameLogger.Log($"Reduced landing duration by {buffValue:P0} for all runways.");
                break;
                
            case BuffType.WearReduction:
                // Add method to reduce runway wear
                airport.RunwayManager.FreeMaintenance(this);
                airport.GameLogger.Log($"Reduced runway wear by {buffValue:P0} for all runways.");
                break;
                
            case BuffType.RepairCostReduction:
                // Add method to reduce repair costs
                airport.GameLogger.Log($"Reduced repair costs by {buffValue:P0} for all runways.");
                break;
        }
    }
}