using System;
using System.Collections.Generic;
using System.Linq;

public class ModifierManager
{
    private readonly List<Modifier> modifiers = new();
    private readonly Revenue revenueCalculator;  // Dependency on Revenue

    // Pass a Revenue instance into the constructor.
    public ModifierManager(Revenue revenueCalculator)
    {
        this.revenueCalculator = revenueCalculator;
    }

    public void AddModifier(string name, double value)
    {
        modifiers.Add(new Modifier(name, value));
    }

    public void RemoveModifier(string name)
    {
        modifiers.RemoveAll(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    // Applies all stored modifiers multiplicatively to a base value.
    public double ApplyModifiers(double baseValue)
    {
        return modifiers.Aggregate(baseValue, (result, modifier) => result * modifier.Value);
    }

    /// <summary>
    /// Calculates revenue for a flight by:
    /// 1. Retrieving the base revenue from the Revenue class.
    /// 2. Applying a delay penalty multiplier.
    /// 3. Adjusting with additional modifiers.
    /// </summary>
    public double CalculateRevenue(Flight flight)
    {
        // Get the base revenue from the Revenue class.
        double baseRevenue = revenueCalculator.CalculateFlightRevenue(flight);

        // Apply delay penalty multiplier.
        double delayMultiplier = GetDelayMultiplier(flight);
        double revenueAfterDelay = baseRevenue * delayMultiplier;

        // Apply any additional modifiers.
        double finalRevenue = ApplyModifiers(revenueAfterDelay);
        return finalRevenue;
    }

    // Every 1200 ticks of delay reduces revenue by 5%, capped at a 40% reduction.
    private double GetDelayMultiplier(Flight flight)
    {
        int delayTicks = flight.GetDelayTicks();
        int penaltyPeriods = delayTicks / 1200;
        double penalty = Math.Min(penaltyPeriods * 0.05, 0.40);
        return 1.0 - penalty;
    }
}