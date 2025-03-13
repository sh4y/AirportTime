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
            Console.WriteLine($"Flight {flight.FlightNumber} lost ${revenueLoss:F2} due to {flight.GetDelayTicks(currentTick)} ticks of delay " +
                            $"(Penalty: {((1.0 - delayMultiplier) * 100):F1}%)");
        }

        // Apply any additional modifiers.
        double finalRevenue = ApplyModifiers(revenueAfterDelay);
        return finalRevenue;
    }

    /// <summary>
    /// Calculates the delay penalty multiplier.
    /// Every 'ticksPerPenaltyPeriod' ticks of delay reduces revenue by 'penaltyPerPeriod',
    /// capped at a maximum total penalty.
    /// The returned multiplier will be between 1.0 (no penalty) and 0.6 (maximum penalty).
    /// </summary>
    private double GetDelayMultiplier(Flight flight, int currentTick)
    {
        const int ticksPerPenaltyPeriod = 5;  // Every 5 ticks
        const double penaltyPerPeriod = 0.05; // 5% penalty per period
        const double maxPenalty = 0.40;       // Maximum 40% penalty

        int delayTicks = flight.GetDelayTicks(currentTick);
        int periods = delayTicks / ticksPerPenaltyPeriod;
        double penalty = periods * penaltyPerPeriod;
        if (penalty > maxPenalty) penalty = maxPenalty;
        return 1.0 - penalty;
    }
}