public class ModifierManager
{
    private List<Modifier> modifiers = new List<Modifier>();

    public void AddModifier(string name, double value)
    {
        modifiers.Add(new Modifier(name, value));
    }

    public void RemoveModifier(string name)
    {
        modifiers.RemoveAll(m => m.Name == name);
    }

    // Apply other modifiers, if any.
    public double ApplyModifiers(double baseValue)
    {
        double modifiedValue = baseValue;
        foreach (var modifier in modifiers)
        {
            modifiedValue *= modifier.Value;
        }
        return modifiedValue;
    }

    // Calculate revenue with delay penalties applied.
    public double CalculateRevenue(Flight flight)
    {
        double baseFare = 100;
        double baseRevenue = flight.Passengers * baseFare;

        // Compute the delay penalty multiplier.
        double delayMultiplier = GetDelayMultiplier(flight);

        // Apply any additional modifiers.
        double revenueWithModifiers = ApplyModifiers(baseRevenue);

        // Final revenue is reduced by the delay penalty.
        return revenueWithModifiers * delayMultiplier;
    }

    // Every 30 seconds (assumed 1200 ticks) causes a 5% revenue reduction, capped at 40%.
    private double GetDelayMultiplier(Flight flight)
    {
        const int TicksPerPenaltyPeriod = 1200; // 30 seconds * 40 ticks per second
        int delayTicks = flight.ScheduledLandingTime - flight.OriginalScheduledLandingTime;
        int delayPeriods = delayTicks / TicksPerPenaltyPeriod;

        // Each period reduces revenue by 5%, up to a max of 40%
        double penaltyPercentage = Math.Min(delayPeriods * 0.05, 0.40);
        return 1.0 - penaltyPercentage;
    }
}
