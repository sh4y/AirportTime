using System;
using System.Collections.Generic;
using System.Linq;

public class RunwayManager
{
    private readonly List<Runway> runways = new List<Runway>();
    private readonly RunwayMaintenanceSystem maintenanceSystem;
    private readonly GameLogger logger;

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
    /// Unlocks a <see cref="Runway"/> given a specific <see cref="RunwayTier"/>.
    /// Registers it with the <see cref="RunwayMaintenanceSystem"/> and logs the result.
    /// </summary>
    /// <param name="tier">The tier of runway to unlock.</param>
    /// <exception cref="ArgumentException">Thrown when an unknown tier is provided.</exception>
    public void UnlockRunway(RunwayTier tier)
    {
        // 4. Validate Input: Check if the enum value is valid
        if (!Enum.IsDefined(typeof(RunwayTier), tier))
        {
            logger.Log($"[UnlockRunway] Invalid RunwayTier provided: {tier}");
            throw new ArgumentException($"Unknown runway tier: {tier}");
        }

        Runway newRunway = null;

        switch (tier)
        {
            case RunwayTier.Tier1:
                newRunway = new SmallRunway("Small Runway", 1000, "Basic runway suitable for small aircraft");
                break;
            default:
                // If you add more tiers in the future, handle them here.
                // For now, throw an exception if it's not recognized.
                logger.Log($"[UnlockRunway] Attempted to unlock an unhandled runway tier: {tier}");
                throw new ArgumentException($"Unknown runway tier: {tier}");
        }

        // If not already unlocked, add it
        if (newRunway != null && !runways.Exists(r => r.Name == newRunway.Name))
        {
            runways.Add(newRunway);
            maintenanceSystem.RegisterRunway(newRunway);
            logger.Log($"[UnlockRunway] Unlocked and registered {newRunway.Name} (Tier {newRunway.ItemTier}).");
        }
        else
        {
            logger.Log($"[UnlockRunway] Runway {newRunway?.Name} was already unlocked or null.");
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
            logger.Log($"[DisplayRunwayInfo] Runway {runway.Name}: " +
                       $"Length {runway.Length}m, Tier {runway.ItemTier}, Wear {wear}%");
        }
    }

    /// <summary>
    /// Helper method that returns runways suitable for landing based on plane requirements
    /// and wear threshold from <see cref="RunwayMaintenanceSystem"/>.
    /// </summary>
    /// <param name="plane">The plane requesting a runway.</param>
    /// <returns>A collection of matching runways.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="plane"/> is null.</exception>
    private IEnumerable<Runway> GetRunwaysSuitableForLanding(Plane plane)
    {
        // 4. Validate Input
        if (plane == null)
        {
            logger.Log("[GetRunwaysSuitableForLanding] Plane object is null; cannot proceed.");
            throw new ArgumentNullException(nameof(plane));
        }

        // 7. Use LINQ to filter runways
        return runways.Where(r => r.CanLand(plane) &&
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
        // 5. Enhance Logging
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
    /// Repairs runways in the manager at any wear level. If a runway's wear exceeds 80%, 
    /// the repair cost is multiplied by 1.4. Now delegates repair cost calculation 
    /// to <see cref="RunwayMaintenanceSystem"/> for consistency (see #6).
    /// </summary>
    /// <param name="treasury">The treasury used to check available funds and deduct repair costs.</param>
    /// <remarks>
    /// <para>If a runway's wear level is 0%, it is skipped.</para>
    /// <para>Base repair cost is calculated in <see cref="RunwayMaintenanceSystem.CalculateRepairCost"/>. 
    /// If <c>wearLevel &gt;= 80</c>, the cost is multiplied by <c>1.4</c>.</para>
    /// <para>If the treasury balance can cover the final cost, the runway is repaired (wear reset to 0);
    /// otherwise, an error message is logged.</para>
    /// </remarks>
    public void PerformMaintenance(Treasury treasury)
    {
        foreach (var runway in runways)
        {
            int wearLevel = maintenanceSystem.GetWearLevel(runway.Name);
            if (wearLevel <= 0)
            {
                // 5. Enhance Logging
                logger.Log($"[PerformMaintenance] {runway.Name} has 0% wear; skipping.");
                continue;
            }

            // 6. Move/extend the cost calculation into RunwayMaintenanceSystem
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
    /// Attempts to handle a landing on the specified <paramref name="runwayID"/> by applying wear 
    /// using the <see cref="RunwayMaintenanceSystem"/>. Returns whether the landing was handled successfully.
    /// </summary>
    /// <param name="runwayID">Unique identifier of the runway.</param>
    /// <param name="weather">Weather conditions affecting wear.</param>
    /// <param name="trafficVolume">Traffic volume impacting wear.</param>
    /// <returns>True if the runway is found and wear is applied, otherwise false.</returns>
    public bool HandleLanding(string runwayID, Weather weather, int trafficVolume)
    {
        // 3. Provide feedback if runway is not found
        var runway = runways.FirstOrDefault(r => r.Name.Equals(runwayID));
        if (runway == null)
        {
            logger.Log($"[HandleLanding] Runway {runwayID} not found. Landing aborted.");
            return false;
        }

        maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume);
        logger.Log($"[HandleLanding] Applied wear to {runwayID}. Current wear: {maintenanceSystem.GetWearLevel(runwayID)}%.");
        return true;
    }
}
