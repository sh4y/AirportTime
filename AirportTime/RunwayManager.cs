using System;
using System.Collections.Generic;
using System.Linq;

public class RunwayManager
{
    private readonly List<Runway> runways = new List<Runway>();
    private readonly RunwayMaintenanceSystem maintenanceSystem;
    private readonly GameLogger logger;
    private double weatherResistance = 0.0; // 0.0 means no resistance, 1.0 means full resistance

    /// <summary>
    /// Initializes a new instance of <see cref="RunwayManager"/>.
    /// </summary>
    /// <param name="maintenanceSystem">A <see cref="RunwayMaintenanceSystem"/> that handles wear and repairs.</param>
    /// <param name="logger">A <see cref="GameLogger"/> for logging events. If you don't have one, you need to inject it.</param>
    public RunwayManager(RunwayMaintenanceSystem maintenanceSystem, GameLogger logger)
    {
        this.maintenanceSystem = maintenanceSystem;
        this.logger = logger;
    }
    
    /// <summary>
    /// Gets the maintenance system for external access
    /// </summary>
    public RunwayMaintenanceSystem GetMaintenanceSystem()
    {
        return maintenanceSystem;
    }
    
    /// <summary>
    /// Returns the total number of runways currently available
    /// </summary>
    public int GetRunwayCount()
    {
        return runways.Count;
    }
    
    /// <summary>
    /// Adds weather resistance to runways (0.0 to 1.0, where 1.0 means complete protection)
    /// </summary>
    public void AddWeatherResistance(double resistance)
    {
        // Clamp value between 0.0 and 1.0
        double newResistance = Math.Clamp(weatherResistance + resistance, 0.0, 1.0);
        
        // Log the change
        if (newResistance > weatherResistance)
        {
            logger.Log($"[AddWeatherResistance] Weather resistance increased from {weatherResistance:P0} to {newResistance:P0}");
        }
        else if (newResistance < weatherResistance)
        {
            logger.Log($"[AddWeatherResistance] Weather resistance decreased from {weatherResistance:P0} to {newResistance:P0}");
        }
        
        weatherResistance = newResistance;
    }
    /// <summary>
    /// Reduces landing duration for all runways by the specified percentage
    /// </summary>
    /// <param name="reductionFactor">Percentage to reduce duration by (0.0 to 1.0)</param>
    public void ReduceAllRunwayLandingDurations(double reductionFactor)
    {
        if (runways.Count == 0)
        {
            logger.Log("[ReduceAllRunwayLandingDurations] No runways available to modify.");
            return;
        }
    
        foreach (var runway in runways)
        {
            runway.ReduceLandingDuration(reductionFactor);
        }
    
        logger.Log($"[ReduceAllRunwayLandingDurations] Reduced landing duration by {reductionFactor:P0} for all runways.");
    }
    /// <summary>
    /// Unlocks a <see cref="Runway"/> given a specific <see cref="RunwayTier"/>.
    /// Registers it with the <see cref="RunwayMaintenanceSystem"/> and logs the result.
    /// </summary>
    /// <param name="tier">The tier of runway to unlock.</param>
    /// <exception cref="ArgumentException">Thrown when an unknown tier is provided.</exception>
    public void UnlockRunway(Runway runway)
    {
        var tier = runway.Tier;
        // Validate Input: Check if the enum value is valid
        if (!Enum.IsDefined(typeof(RunwayTier), tier))
        {
            logger.Log($"[UnlockRunway] Invalid RunwayTier provided: {tier}");
            throw new ArgumentException($"Unknown runway tier: {tier}");
        }

        // Handle different runway tiers
        switch (tier)
        {
            case RunwayTier.Tier1:
                // For small runways
                break;
            case RunwayTier.Tier2:
                // For medium runways
                break;
            case RunwayTier.Tier3:
                // For large runways
                break;
            default:
                // If you add more tiers in the future, handle them here.
                logger.Log($"[UnlockRunway] Attempted to unlock an unhandled runway tier: {tier}");
                throw new ArgumentException($"Unknown runway tier: {tier}");
        }

        // If not already unlocked, add it
        if (!runways.Exists(r => r.Name == runway.Name))
        {
            runways.Add(runway);
            maintenanceSystem.RegisterRunway(runway);
            logger.Log($"[UnlockRunway] Unlocked and registered {runway.Name} (Tier {runway.ItemTier}).");
        }
        else
        {
            logger.Log($"[UnlockRunway] Runway {runway.Name} was already unlocked.");
        }
    }

    /// <summary>
    /// Displays info about each unlocked runway, including wear level.
    /// Logs a message if none are unlocked.
    /// </summary>
    public void DisplayRunwayInfo()
    {
        if (runways.Count == 0)
        {
            logger.Log("[DisplayRunwayInfo] No runways unlocked yet.");
            return;
        }

        foreach (var runway in runways)
        {
            int wear = maintenanceSystem.GetWearLevel(runway.Name);
            string occupiedStatus = runway.IsOccupied 
                ? $"OCCUPIED ({runway.OccupiedCountdown} ticks remaining)" 
                : "AVAILABLE";
                
            logger.Log($"[DisplayRunwayInfo] Runway {runway.Name}: " +
                      $"Length {runway.Length}m, Tier {runway.ItemTier}, Wear {wear}%, Status: {occupiedStatus}");
        }
        
        // If we have weather resistance, display it
        if (weatherResistance > 0.0)
        {
            logger.Log($"[DisplayRunwayInfo] Weather Resistance: {weatherResistance:P0}");
        }
    }

    /// <summary>
    /// Helper method that returns runways suitable for landing based on plane requirements
    /// and wear threshold from <see cref="RunwayMaintenanceSystem"/>.
    /// </summary>
    /// <param name="plane">The plane requesting a runway.</param>
    /// <returns>A collection of matching runways.</returns>
    private IEnumerable<Runway> GetRunwaysSuitableForLanding(Plane plane)
    {
        // Validate Input
        if (plane == null)
        {
            logger.Log("[GetRunwaysSuitableForLanding] Plane object is null; cannot proceed.");
            throw new ArgumentNullException(nameof(plane));
        }

        // Use LINQ to filter runways
        return runways.Where(r => 
            r.CanLand(plane) && 
            !r.IsOccupied &&  // Check if runway is not occupied
            maintenanceSystem.GetWearLevel(r.Name) < RunwayMaintenanceSystem.FullDegradationThreshold);
    }

    /// <summary>
    /// Checks if there is at least one runway that can land the given <paramref name="plane"/>.
    /// Uses <see cref="GetRunwaysSuitableForLanding(Plane)"/> to consolidate logic.
    /// </summary>
    /// <param name="plane">The plane requesting a runway.</param>
    /// <returns>True if at least one runway is suitable; false otherwise.</returns>
    public bool CanLand(Plane plane)
    {
        var canLand = GetRunwaysSuitableForLanding(plane).Any();
        logger.Log(canLand
            ? $"[CanLand] Plane can land. (Required length: {plane.RequiredRunwayLength})"
            : "[CanLand] No runway currently suitable for landing.");
        return canLand;
    }

    /// <summary>
    /// Finds a single runway that can land the given <paramref name="plane"/>, 
    /// or null if none are suitable.
    /// Uses <see cref="GetRunwaysSuitableForLanding(Plane)"/> to consolidate logic.
    /// </summary>
    /// <param name="plane">The plane requesting a runway.</param>
    /// <returns>A suitable <see cref="Runway"/> or null.</returns>
    public Runway GetAvailableRunway(Plane plane)
    {
        var runway = GetRunwaysSuitableForLanding(plane).FirstOrDefault();
        logger.Log(runway != null
            ? $"[GetAvailableRunway] Found suitable runway: {runway.Name} (Required length: {plane.RequiredRunwayLength})."
            : "[GetAvailableRunway] No suitable runway found.");
        return runway;
    }

    /// <summary>
    /// Finds all runways that can land the given <paramref name="plane"/>.
    /// Uses <see cref="GetRunwaysSuitableForLanding(Plane)"/> to consolidate logic.
    /// </summary>
    /// <param name="plane">The plane requesting a runway.</param>
    /// <returns>A list of suitable <see cref="Runway"/> objects, or an empty list if none are available.</returns>
    public List<Runway> GetAvailableRunways(Plane plane)
    {
        var availableRunways = GetRunwaysSuitableForLanding(plane).ToList();
        logger.Log(availableRunways.Count > 0
            ? $"[GetAvailableRunways] Found {availableRunways.Count} suitable runway(s). (Required length: {plane.RequiredRunwayLength})"
            : "[GetAvailableRunways] No suitable runways found.");
        return availableRunways;
    }

    /// <summary>
    /// Handles landing on a runway, applying weather-resistant wear
    /// </summary>
    public bool HandleLanding(string runwayID, Weather weather, int trafficVolume)
    {
        // Find the runway
        var runway = runways.FirstOrDefault(r => r.Name.Equals(runwayID));
        if (runway == null)
        {
            logger.Log($"[HandleLanding] Runway {runwayID} not found. Landing aborted.");
            return false;
        }

        // Check if runway is already occupied
        if (runway.IsOccupied)
        {
            logger.Log($"[HandleLanding] Runway {runwayID} is currently occupied. Landing aborted.");
            return false;
        }

        // Occupy the runway for landing
        runway.OccupyForLanding();
    
        // Apply wear with weather resistance applied to the weather impact
        if (weatherResistance > 0.0)
        {
            // Calculate reduced weather impact
            int originalImpact = weather.GetWeatherImpact();
            int reducedImpact = (int)(originalImpact * (1.0 - weatherResistance));
        
            // Create a custom traffic volume that includes the reduced weather impact
            int adjustedTrafficVolume = trafficVolume - (originalImpact - reducedImpact);
        
            logger.Log($"[HandleLanding] Weather resistance reduced impact from {originalImpact} to {reducedImpact}");
            // Apply wear with weather resistance
            maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume, weatherResistance);        
        }
        else
        {
            // Apply wear normally
            maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume);
        }
    
        logger.Log($"[HandleLanding] Applied wear to {runwayID}. Current wear: {maintenanceSystem.GetWearLevel(runwayID)}%. " +
                   $"Runway will be occupied for {runway.LandingDuration} ticks.");
    
        return true;
    }
    /// <summary>
    /// Updates all runways on each tick
    /// </summary>
    public void UpdateRunwaysStatus()
    {
        foreach (var runway in runways)
        {
            bool wasOccupied = runway.IsOccupied;
            int previousCountdown = runway.OccupiedCountdown;
            
            runway.UpdateOccupiedStatus();
            
            // Log when a runway becomes free
            if (wasOccupied && !runway.IsOccupied)
            {
                logger.Log($"[UpdateRunwaysStatus] Runway {runway.Name} is now free and available for landings.");
            }
        }
    }
    /// <summary>
    /// Gets a runway by its name
    /// </summary>
    /// <param name="runwayName">The name of the runway to retrieve</param>
    /// <returns>The runway object or null if not found</returns>
    public Runway GetRunwayByName(string runwayName)
    {
        return runways.FirstOrDefault(r => r.Name.Equals(runwayName, StringComparison.OrdinalIgnoreCase));
    }
    /// <summary>
    /// Performs maintenance on all runways
    /// </summary>
    public void PerformMaintenance(Treasury treasury)
    {
        foreach (var runway in runways)
        {
            int wearLevel = maintenanceSystem.GetWearLevel(runway.Name);
            if (wearLevel <= 0)
            {
                // Skip runways with no wear
                logger.Log($"[PerformMaintenance] {runway.Name} has 0% wear; skipping.");
                continue;
            }

            // Calculate repair cost
            double repairCost = maintenanceSystem.CalculateRepairCost(wearLevel);

            if (treasury.GetBalance() >= repairCost)
            {
                treasury.DeductFunds(repairCost, $"Maintenance for {runway.Name}");
                maintenanceSystem.RepairRunway(runway.Name);
                logger.Log($"[PerformMaintenance] {runway.Name} repaired for {repairCost:C} (Wear was {wearLevel}%).");
            }
            else
            {
                logger.Log($"[PerformMaintenance] Insufficient funds ({repairCost:C}) to repair {runway.Name} (Wear: {wearLevel}%).");
            }
        }
    }
    /// <summary>

/// <summary>
/// Handles landing on a runway, applying weather-resistant wear
/// </summary>
public bool HandleLanding(string runwayID, Weather weather, int trafficVolume, string flightNumber = null)
{
    // Find the runway
    var runway = runways.FirstOrDefault(r => r.Name.Equals(runwayID));
    if (runway == null)
    {
        logger.Log($"[HandleLanding] Runway {runwayID} not found. Landing aborted.");
        return false;
    }

    // Check if runway is already occupied
    if (runway.IsOccupied)
    {
        logger.Log($"[HandleLanding] Runway {runwayID} is currently occupied. Landing aborted.");
        return false;
    }

    // Occupy the runway for landing
    if (!string.IsNullOrEmpty(flightNumber))
        runway.OccupyForLanding(flightNumber);
    else
        runway.OccupyForLanding();
    
    // Apply wear with weather resistance applied to the weather impact
    if (weatherResistance > 0.0)
    {
        // Calculate reduced weather impact
        int originalImpact = weather.GetWeatherImpact();
        int reducedImpact = (int)(originalImpact * (1.0 - weatherResistance));
    
        // Create a custom traffic volume that includes the reduced weather impact
        int adjustedTrafficVolume = trafficVolume - (originalImpact - reducedImpact);
    
        logger.Log($"[HandleLanding] Weather resistance reduced impact from {originalImpact} to {reducedImpact}");
        // Apply wear with weather resistance
        maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume, weatherResistance);        
    }
    else
    {
        // Apply wear normally
        maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume);
    }

    logger.Log($"[HandleLanding] Applied wear to {runwayID}. Current wear: {maintenanceSystem.GetWearLevel(runwayID)}%. " +
               $"Runway will be occupied for {runway.LandingDuration} ticks.");

    return true;
}
}