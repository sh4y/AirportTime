using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles emergency flights and tracks response deadlines
/// </summary>
public class EmergencyFlightHandler : IEmergencyFlightHandler
{
    private readonly GameLogger logger;
    private readonly FailureTracker failureTracker;
    private readonly ITickManager tickManager;
    
    // Track active emergency flights with their deadlines
    private Dictionary<string, (Flight flight, int tickDetected, int responseDeadline)> activeEmergencyFlights = 
        new Dictionary<string, (Flight, int, int)>();
    
    private readonly int emergencyResponseWindow = 20; // Ticks to respond
    
    public EmergencyFlightHandler(GameLogger logger, FailureTracker failureTracker, ITickManager tickManager)
    {
        this.logger = logger;
        this.failureTracker = failureTracker;
        this.tickManager = tickManager;
    }
    
    /// <summary>
    /// Registers an emergency flight that requires manual handling
    /// </summary>
    public void RegisterEmergencyFlight(Flight flight, int currentTick)
    {
        if (flight.Priority != FlightPriority.Emergency && flight.Type != FlightType.Emergency) 
            return;
            
        string flightNumber = flight.FlightNumber;
        int responseDeadline = currentTick + emergencyResponseWindow;
        
        activeEmergencyFlights[flightNumber] = (flight, currentTick, responseDeadline);
        
        logger.Log($"⚠️ EMERGENCY: Flight {flightNumber} requires immediate attention! Must be handled within {emergencyResponseWindow} ticks.");
    }
    
    /// <summary>
    /// Marks an emergency as handled successfully
    /// </summary>
    public void MarkEmergencyHandled(string flightNumber)
    {
        if (activeEmergencyFlights.ContainsKey(flightNumber))
        {
            activeEmergencyFlights.Remove(flightNumber);
            logger.Log($"✅ Emergency flight {flightNumber} has been handled successfully.");
        }
    }
    
    /// <summary>
    /// Processes all active emergencies and checks for expired response deadlines
    /// </summary>
    public void ProcessEmergencies(int currentTick)
    {
        var expiredEmergencies = activeEmergencyFlights
            .Where(kvp => kvp.Value.responseDeadline < currentTick)
            .ToList();
            
        foreach (var emergency in expiredEmergencies)
        {
            string flightNumber = emergency.Key;
            var (flight, tickDetected, _) = emergency.Value;
            
            failureTracker.RecordFailure(
                FailureType.EmergencyFlightResponse,
                $"Failed to respond to emergency flight {flightNumber} within time limit"
            );
            
            activeEmergencyFlights.Remove(flightNumber);
            flight.CancelFlight("Emergency response time expired");
            
            logger.Log($"🚨 CRITICAL FAILURE: Emergency flight {flightNumber} response time expired! Flight lost.");
        }
    }
    
    /// <summary>
    /// Gets the count of active emergency flights
    /// </summary>
    public int GetActiveEmergencyCount() => activeEmergencyFlights.Count;
    
    /// <summary>
    /// Gets information about all active emergency flights
    /// </summary>
    public List<(string flightNumber, int ticksRemaining, Flight flight)> GetActiveEmergencies(int currentTick) =>
        activeEmergencyFlights.Select(e => 
            (e.Key, Math.Max(0, e.Value.responseDeadline - currentTick), e.Value.flight)
        ).ToList();
        
    /// <summary>
    /// Checks if a flight is an active emergency that needs response
    /// </summary>
    public bool IsActiveEmergency(string flightNumber) => activeEmergencyFlights.ContainsKey(flightNumber);
}