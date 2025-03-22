using System;
using System.Collections.Generic;
using System.Linq;
using AirportTime;

public class ModifierManager : IModifierManager
{
    private readonly List<Modifier> modifiers = new List<Modifier>();
    // New dictionary to store flight type specific modifiers
    private readonly Dictionary<FlightType, List<Modifier>> flightTypeModifiers = new Dictionary<FlightType, List<Modifier>>();
    
    private readonly Revenue revenueCalculator;
    private readonly GameLogger gameLogger;

    public ModifierManager(Revenue revenueCalculator, GameLogger gameLogger)
    {
        this.revenueCalculator = revenueCalculator;
        this.gameLogger = gameLogger;
        
        // Initialize the dictionary for each flight type
        foreach (FlightType type in Enum.GetValues(typeof(FlightType)))
        {
            flightTypeModifiers[type] = new List<Modifier>();
        }
    }

    public void AddModifier(string name, double value)
    {
        modifiers.Add(new Modifier(name, value));
    }
    
    /// <summary>
    /// Adds a modifier specific to a flight type
    /// </summary>
    public void AddFlightTypeModifier(FlightType flightType, double value, string name)
    {
        flightTypeModifiers[flightType].Add(new Modifier(name, value));
        gameLogger.Log($"Added flight-specific modifier: {name} ({value:F2}x) for {flightType} flights");
    }

    public void RemoveModifier(string name)
    {
        modifiers.RemoveAll(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        
        // Also check flight type modifiers
        foreach (var type in flightTypeModifiers.Keys)
        {
            flightTypeModifiers[type].RemoveAll(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Applies all stored modifiers multiplicatively to a base value.
    public double ApplyModifiers(double baseValue)
    {
        return modifiers.Aggregate(baseValue, (result, modifier) => result * modifier.Value);
    }
    
    /// <summary>
    /// Applies modifiers specific to a flight type
    /// </summary>
    public double ApplyFlightTypeModifiers(double baseValue, FlightType flightType)
    {
        if (!flightTypeModifiers.ContainsKey(flightType))
            return baseValue;
            
        return flightTypeModifiers[flightType].Aggregate(baseValue, (result, modifier) => result * modifier.Value);
    }

    /// <summary>
    /// Calculates revenue for a flight by:
    /// 1. Retrieving the base revenue from the Revenue class.
    /// 2. Applying a delay penalty multiplier.
    /// 3. Applying flight-type specific modifiers.
    /// 4. Adjusting with general modifiers.
    /// </summary>
    public double CalculateRevenue(Flight flight, int currentTick)
    {
        // Get the base revenue from the Revenue class.
        double baseRevenue = revenueCalculator.CalculateFlightRevenue(flight);

        // Apply delay penalty multiplier.
        double delayMultiplier = GetDelayMultiplier(flight, currentTick);
        double revenueAfterDelay = baseRevenue * delayMultiplier;

        // Calculate and log revenue loss from delay
        if (delayMultiplier < 1.0)
        {
            double revenueLoss = baseRevenue - revenueAfterDelay;
            gameLogger.Log($"Flight {flight.FlightNumber} lost ${revenueLoss:F2} due to {flight.GetDelayTicks(currentTick)} ticks of delay " +
                            $"(Penalty: {((1.0 - delayMultiplier) * 100):F1}%)");
        }
        
        // Apply flight-type specific modifiers
        double revenueAfterFlightTypeModifiers = ApplyFlightTypeModifiers(revenueAfterDelay, flight.Type);
        
        // If flight type modifiers were applied, log it
        if (revenueAfterFlightTypeModifiers > revenueAfterDelay)
        {
            double bonus = revenueAfterFlightTypeModifiers - revenueAfterDelay;
            gameLogger.Log($"Flight {flight.FlightNumber} earned ${bonus:F2} extra from {flight.Type} specialization bonus!");
        }

        // Apply any additional general modifiers.
        double finalRevenue = ApplyModifiers(revenueAfterFlightTypeModifiers);
        return finalRevenue;
    }
    
    /// <summary>
    /// Retrieves all active modifiers
    /// </summary>
    public List<Modifier> GetAllModifiers()
    {
        // Create a new list to avoid returning the internal list
        List<Modifier> allModifiers = new List<Modifier>(modifiers);
        
        // Add all flight type modifiers
        foreach (var typeModifiers in flightTypeModifiers.Values)
        {
            allModifiers.AddRange(typeModifiers);
        }
        
        return allModifiers;
    }

    /// <summary>
    /// Calculates the delay penalty multiplier.
    /// Every 'ticksPerPenaltyPeriod' ticks of delay reduces revenue by 'penaltyPerPeriod',
    /// capped at a maximum total penalty.
    /// The returned multiplier will be between 1.0 (no penalty) and 0.6 (maximum penalty).
    /// </summary>
    private double GetDelayMultiplier(Flight flight, int currentTick)
    {
        const int ticksPerPenaltyPeriod = 10;  // Every 5 ticks
        const double penaltyPerPeriod = 0.05; // 5% penalty per period
        const double maxPenalty = 0.40;       // Maximum 40% penalty

        int delayTicks = flight.GetDelayTicks(currentTick);
        int periods = delayTicks / ticksPerPenaltyPeriod;
        double penalty = periods * penaltyPerPeriod;
        if (penalty > maxPenalty) penalty = maxPenalty;
        
        return 1.0 - penalty;
    }
}