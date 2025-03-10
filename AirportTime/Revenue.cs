using System;

public class Revenue
{
    private readonly double baseFare;

    // Optionally let users override the default baseFare if desired
    public Revenue(double baseFare = 10)
    {
        this.baseFare = baseFare;
    }

    /// <summary>
    /// Returns how much to multiply the base fare by, depending on the flight type.
    /// Extend this logic for additional FlightTypes as needed.
    /// </summary>
    private double ComputeBaseFareMultiplier(FlightType type)
    {
        return type switch
        {
            FlightType.Commercial => 1.0,
            FlightType.Cargo      => 0.75,
            FlightType.VIP        => 2.0,
            FlightType.Emergency  => 1.5,
            _                     => 1.0
        };
    }

    /// <summary>
    /// Calculates and returns revenue for a single flight, without storing any totals.
    /// </summary>
    public double CalculateFlightRevenue(Flight flight)
    {
        double multiplier = ComputeBaseFareMultiplier(flight.Type);
        return baseFare * flight.Passengers * multiplier;
    }
}