public class RunwayMaintenanceSystem
{
    private Dictionary<string, Runway> registeredRunways;
    private const int WearIncrementPerLanding = 4;
    public const int CriticalWearThreshold = 50;
    public const int FullDegradationThreshold = 100;
    private const int WeatherImpactMultiplier = 5;

    public RunwayMaintenanceSystem()
    {
        // Instead of storing just wear levels, we now store references to actual Runway objects.
        registeredRunways = new Dictionary<string, Runway>();
    }

    public void RegisterRunway(Runway runway)
    {
        if (!registeredRunways.ContainsKey(runway.Name))
        {
            registeredRunways[runway.Name] = runway;
        }
    }

    // Apply wear based on traffic and weather conditions
    // But the actual increment logic is offloaded to the runway’s own method
    public void ApplyWear(string runwayID, Weather weather, int trafficVolume=0, double weatherResistanceFactor=0.0)
    {
        if (registeredRunways.TryGetValue(runwayID, out var runway))
        {
            // Calculate base wear
            int wearIncrease = WearIncrementPerLanding + new Random().Next(1,7) + (trafficVolume / 10);
        
            // // Calculate weather impact with resistance
            // int weatherImpact = weather.GetWeatherImpact();
            // int resistantWeatherImpact = (int)(weatherImpact * (1.0 - weatherResistanceFactor));
        
            // Calculate total wear
            int totalWear = wearIncrease;

            // Apply the wear
            runway.ApplyWear(totalWear);
        }
    }
    
    /// <summary>
    /// Calculates the total repair cost based on the given <paramref name="wearLevel"/>.
    /// If wear is >= 80%, applies a 1.4x multiplier.
    /// </summary>
    /// <param name="wearLevel">The current wear level of the runway.</param>
    /// <returns>The total repair cost.</returns>
    public double CalculateRepairCost(int wearLevel)
    {
        // Base cost
        double baseCost = wearLevel * 11;

        // Apply a 1.4x multiplier if wear >= 80%
        if (wearLevel >= 80)
            return baseCost * 1.3;
    
        return baseCost;
    }


    public void RepairRunway(string runwayID, bool occupy = true)
    {
        if (registeredRunways.TryGetValue(runwayID, out var runway))
        {
            if (occupy)
            {
                // Repair the runway and occupy it
                runway.Repair();
            }
            else
            {
                // Repair the runway without occupying it
                runway.RepairNoOccupy();
            }
            Console.WriteLine($"✅ Runway {runwayID} has been fully repaired and is back in service.");
        }
    }

    public int GetWearLevel(string runwayID)
    {
        if (registeredRunways.TryGetValue(runwayID, out var runway))
            return runway.WearLevel;
        return 0;
    }
}
