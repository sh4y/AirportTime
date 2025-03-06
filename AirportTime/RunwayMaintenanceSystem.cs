
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class RunwayMaintenanceSystem
{
    private Dictionary<string, int> runwayWearLevels;
    private const int WearIncrementPerLanding = 10;
    public const int CriticalWearThreshold = 50;
    public const int FullDegradationThreshold = 100;
    private const int WeatherImpactMultiplier = 5;

    public RunwayMaintenanceSystem()
    {
        runwayWearLevels = new Dictionary<string, int>();
    }

    public void RegisterRunway(string runwayID)
    {
        if (!runwayWearLevels.ContainsKey(runwayID))
        {
            runwayWearLevels[runwayID] = 0; // New runways start at 0% wear
        }
    }

    // Apply wear based on traffic and weather conditions
    public void ApplyWear(string runwayID, Weather weather, int trafficVolume)
    {
        if (runwayWearLevels.ContainsKey(runwayID))
        {
            int wearIncrease = WearIncrementPerLanding + (trafficVolume / 10); // More traffic = more wear
            int weatherImpact = weather.GetWeatherImpact() * WeatherImpactMultiplier;
            runwayWearLevels[runwayID] += wearIncrease + weatherImpact;

            if (runwayWearLevels[runwayID] >= CriticalWearThreshold)
            {
                Console.WriteLine($"⚠️ Warning: Runway {runwayID} is at {runwayWearLevels[runwayID]}% wear and needs maintenance soon!");
            }

            if (runwayWearLevels[runwayID] >= FullDegradationThreshold)
            {
                Console.WriteLine($"❌ Runway {runwayID} is fully degraded and is now CLOSED for use.");
            }
        }
    }

    public void RepairRunway(string runwayID)
    {
        if (runwayWearLevels.ContainsKey(runwayID))
        {
            runwayWearLevels[runwayID] = 0;
            Console.WriteLine($"✅ Runway {runwayID} has been fully repaired and is back in service.");
        }
    }

    public int GetWearLevel(string runwayID)
    {
        return runwayWearLevels.ContainsKey(runwayID) ? runwayWearLevels[runwayID] : 0;
    }
}
