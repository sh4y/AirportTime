using System;
using System.Collections.Generic;

public class RunwayManager
{
    private List<Runway> runways = new List<Runway>();
    private RunwayMaintenanceSystem maintenanceSystem;

    public RunwayManager(RunwayMaintenanceSystem maintenanceSystem)
    {
        this.maintenanceSystem = maintenanceSystem;
    }

    public void UnlockRunway(string runwayName)
    {
        Runway newRunway = null;

        if (runwayName == "Tier 1 Runway")
        {
            newRunway = new Runway("Runway 01L", 100);
        }
        else if (runwayName == "Tier 2 Runway")
        {
            newRunway = new Runway("Runway 02R", 2000);
        }
        else if (runwayName == "Tier 3 Runway")
        {
            newRunway = new Runway("Runway 03L", 50000);
        }

        if (newRunway != null)
        {
            runways.Add(newRunway);
            maintenanceSystem.RegisterRunway(newRunway.RunwayID);
            Console.WriteLine($"Unlocked and registered {newRunway.RunwayID}");
        }
    }

    public void DisplayRunwayInfo()
    {
        if (runways.Count == 0)
        {
            Console.WriteLine("No runways unlocked yet.");
        }
        else
        {
            Console.WriteLine("Unlocked Runways:");
            foreach (var runway in runways)
            {
                int wear = maintenanceSystem.GetWearLevel(runway.RunwayID);
                Console.WriteLine($"- {runway.RunwayID} ({runway.Length}m, Wear: {wear}%)");
            }
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
            {
                return runway;
            }
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
                double repairCost = wearLevel * 15; // Example repair cost calculation
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

    // Called when a flight lands to apply wear
    public void HandleLanding(string runwayID, Weather weather, int trafficVolume)
    {
        maintenanceSystem.ApplyWear(runwayID, weather, trafficVolume);
    }
    public List<Runway> GetAvailableRunways(Plane plane)
    {
        List<Runway> availableRunways = new List<Runway>();
        foreach (var runway in runways)
        {
            int wear = maintenanceSystem.GetWearLevel(runway.RunwayID);
            if (runway.CanLand(plane) && wear < RunwayMaintenanceSystem.FullDegradationThreshold)
            {
                availableRunways.Add(runway);
            }
        }
        return availableRunways;
    }
}
