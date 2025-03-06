using System;
using System.Collections.Generic;

public class RunwayManager
{
    private readonly List<Runway> runways = new List<Runway>();
    private readonly RunwayMaintenanceSystem maintenanceSystem;

    public RunwayManager(RunwayMaintenanceSystem maintenanceSystem)
    {
        this.maintenanceSystem = maintenanceSystem;
    }

    public void UnlockRunway(RunwayTier tier)
    {
        Runway newRunway = null;

        switch (tier)
        {
            case RunwayTier.Tier1:
                newRunway = new SmallRunway("Runway 01L");
                break;
            case RunwayTier.Tier2:
                newRunway = new MediumRunway("Runway 02R");
                break;
            case RunwayTier.Tier3:
                newRunway = new LargeRunway("Runway 03L");
                break;
            default:
                Console.WriteLine("Unknown runway tier specified.");
                break;
        }

        if (newRunway != null)
        {
            runways.Add(newRunway);
            maintenanceSystem.RegisterRunway(newRunway.RunwayID);
            Console.WriteLine($"Unlocked and registered {newRunway.RunwayID} (Tier {newRunway.Tier})");
        }
    }

    public void DisplayRunwayInfo()
    {
        if (runways.Count == 0)
        {
            Console.WriteLine("No runways unlocked yet.");
            return;
        }
        Console.WriteLine("Unlocked Runways:");
        foreach (var runway in runways)
        {
            int wear = maintenanceSystem.GetWearLevel(runway.RunwayID);
            Console.WriteLine($"- {runway.RunwayID} (Length: {runway.Length}m, Tier: {runway.Tier}, Wear: {wear}%)");
        }
    }

    public bool CanLand(Plane plane)
    {
        foreach (var runway in runways)
        {
            int wear = maintenanceSystem.GetWearLevel(runway.RunwayID);
            if (runway.CanLand(plane) && wear < RunwayMaintenanceSystem.FullDegradationThreshold)
            {
                return true;
            }
        }
        return false;
    }

    public Runway GetAvailableRunway(Plane plane)
    {
        foreach (var runway in runways)
        {
            int wear = maintenanceSystem.GetWearLevel(runway.RunwayID);
            if (runway.CanLand(plane) && wear < RunwayMaintenanceSystem.FullDegradationThreshold)
                return runway;
        }
        return null;
    }

    public void PerformMaintenance(Treasury treasury, GameLogger logger)
    {
        foreach (var runway in runways)
        {
            int wearLevel = maintenanceSystem.GetWearLevel(runway.RunwayID);
            if (wearLevel >= RunwayMaintenanceSystem.CriticalWearThreshold)
            {
                double repairCost = wearLevel * 15; // example cost calculation
                if (treasury.GetBalance() >= repairCost)
                {
                    treasury.DeductFunds(repairCost, $"Maintenance for {runway.RunwayID}");
                    maintenanceSystem.RepairRunway(runway.RunwayID);
                    logger.Log($"Runway {runway.RunwayID} repaired at cost {repairCost:C}");
                }
                else
                {
                    logger.Log($"Insufficient funds ({repairCost:C}) to repair runway {runway.RunwayID}");
                }
            }
        }
    }

    public void HandleLanding(string runwayID, Weather weather, int trafficVolume)
    {
        maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume);
    }

    public List<Runway> GetAvailableRunways(Plane plane)
    {
        var availableRunways = new List<Runway>();
        foreach (var runway in runways)
        {
            int wear = maintenanceSystem.GetWearLevel(runway.RunwayID);
            if (runway.CanLand(plane) && wear < RunwayMaintenanceSystem.FullDegradationThreshold)
                availableRunways.Add(runway);
        }
        return availableRunways;
    }
}
