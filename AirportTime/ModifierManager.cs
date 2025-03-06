public class ModifierManager
{
    private readonly List<Modifier> modifiers = new();

    public void AddModifier(string name, double value)
    {
        modifiers.Add(new Modifier(name, value));
    }

    public void RemoveModifier(string name)
    {
        modifiers.RemoveAll(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    // Applies all stored modifiers to a base value.
    public double ApplyModifiers(double baseValue)
    {
        double result = baseValue;
        foreach (var modifier in modifiers)
        {
            // For simplicity, assume modifiers are additive.
            result *= modifier.Value;
        }
        return result;
    }

    // Calculates revenue for a flight by applying delay penalties.
    public double CalculateRevenue(Flight flight)
    {
        // Example base revenue calculation.
        double baseRevenue = flight.Passengers * 10;
        double delayMultiplier = GetDelayMultiplier(flight);
        return baseRevenue * delayMultiplier;
    }

    // Every 1200 ticks of delay reduces revenue by 5%, capped at a 40% reduction.
    private double GetDelayMultiplier(Flight flight)
    {
        int delayTicks = flight.GetDelayTicks();
        int penaltyPeriods = delayTicks / 1200;
        double penalty = penaltyPeriods * 0.05;
        if (penalty > 0.40)
            penalty = 0.40;
        return 1.0 - penalty;
    }
}
