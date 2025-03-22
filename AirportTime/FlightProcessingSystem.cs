using System;
using System.Collections.Generic;
using AirportTime;

public class FlightProcessingSystem
{
    private readonly Queue<Flight> landingQueue;
    private readonly RunwayManager runwayManager;
    private readonly Treasury treasury;
    private readonly ModifierManager modifierManager;
    private readonly GameLogger gameLogger;
    private readonly TickManager tickManager;
    public FlightProcessingSystem(
        Queue<Flight> landingQueue,
        RunwayManager runwayManager,
        Treasury treasury,
        ModifierManager modifierManager,
        GameLogger gameLogger, TickManager tickManager)
    {
        this.landingQueue = landingQueue;
        this.runwayManager = runwayManager;
        this.treasury = treasury;
        this.modifierManager = modifierManager;
        this.gameLogger = gameLogger;
        this.tickManager = tickManager;
    }

    /// <summary>
    /// Processes all flights in the landing queue by requiring the user to manually assign a runway.
    /// </summary>
    public void ProcessLandingQueue()
    {
        while (landingQueue.Count > 0)
        {
            Flight flight = landingQueue.Dequeue();
            ProcessLanding(flight);
        }
    }

    /// <summary>
    /// Lists available runways for the flight's plane and asks the user to choose one.
    /// If a valid selection is made, attempts the landing, calculates revenue, and logs the outcome.
    /// </summary>
    /// <param name="flight">The flight to be landed.</param>
    private void ProcessLanding(Flight flight)
    {
        // Retrieve a list of available runways that can accommodate the plane.
        List<Runway> availableRunways = runwayManager.GetAvailableRunways(flight.Plane);

        if (availableRunways == null || availableRunways.Count == 0)
        {
            gameLogger.Log($"No available runway for Flight {flight.FlightNumber}. Flight delayed.");
            // Optionally, re-enqueue flight or trigger delay event here.
            return;
        }

        Console.WriteLine($"\nFlight {flight.FlightNumber} requires landing. Please assign a runway:");
        for (int i = 0; i < availableRunways.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {availableRunways[i].Name} (Length: {availableRunways[i].Length}m)");
        }
        Console.Write("Enter the number corresponding to your chosen runway: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int selection) && selection >= 1 && selection <= availableRunways.Count)
        {
            Runway selectedRunway = availableRunways[selection - 1];
            // Attempt landing on the selected runway
            flight.AttemptLanding(selectedRunway);

            // Calculate revenue with modifiers
            double revenue = modifierManager.CalculateRevenue(flight, tickManager.CurrentTick);
            treasury.AddFunds(revenue, "Flight Revenue");

            gameLogger.Log($"Flight {flight.FlightNumber} landed successfully on {selectedRunway.Name} and generated {revenue:C} in revenue.");
        }
        else
        {
            gameLogger.Log($"Invalid selection. Flight {flight.FlightNumber} landing aborted.");
            // Optionally, you can re-enqueue the flight or handle it as a delay.
        }
    }
}
