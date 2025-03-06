public class Flight
{
    public string FlightNumber { get; private set; }
    public Plane Plane { get; private set; }
    public FlightType Type { get; private set; }
    public FlightPriority Priority { get; private set; }
    public int ScheduledLandingTime { get; private set; }
    public int OriginalScheduledLandingTime { get; private set; }
    public int Passengers { get; private set; }

    public Flight(string flightNumber, Plane plane, FlightType type, FlightPriority priority, int scheduledLandingTime, int passengers)
    {
        FlightNumber = flightNumber;
        Plane = plane;
        Type = type;
        Priority = priority;
        ScheduledLandingTime = scheduledLandingTime;
        OriginalScheduledLandingTime = scheduledLandingTime;  // Capture the original landing time
        Passengers = passengers;
    }

    public void AttemptLanding(Runway runway)
    {
        if (runway.CanLand(Plane))
        {
            Console.WriteLine($"Flight {FlightNumber} can land.");
        }
        else
        {
            Console.WriteLine($"Flight {FlightNumber} cannot land; no suitable runway.");
        }
    }

    // Delay the flight by a number of ticks, leaving the original time unchanged.
    public void Delay(int delayTicks)
    {
        ScheduledLandingTime += delayTicks;
    }

    // Returns the total number of ticks the flight is delayed.
    public int GetDelayTicks()
    {
        return ScheduledLandingTime - OriginalScheduledLandingTime;
    }

    // Returns true if the flight is delayed.
    public bool IsDelayed()
    {
        return GetDelayTicks() > 0;
    }

    /// <summary>
    /// Calculates the delay penalty multiplier.
    /// Every 'ticksPerPenaltyPeriod' ticks of delay reduces revenue by 'penaltyPerPeriod' (e.g., 0.05 for 5%), 
    /// capped at a maximum total penalty of 'maxPenalty' (e.g., 0.40 for 40%).
    /// The returned multiplier will be between 1.0 (no penalty) and 0.6 (maximum penalty).
    /// </summary>
    /// <param name="ticksPerPenaltyPeriod">Number of ticks that constitute one penalty period.</param>
    /// <param name="penaltyPerPeriod">Penalty per period (e.g., 0.05 for 5%).</param>
    /// <param name="maxPenalty">Maximum penalty (e.g., 0.40 for 40%).</param>
    /// <returns>The multiplier to apply to the base revenue.</returns>
    public double GetDelayPenaltyMultiplier(int ticksPerPenaltyPeriod, double penaltyPerPeriod, double maxPenalty)
    {
        int delayTicks = GetDelayTicks();
        int periods = delayTicks / ticksPerPenaltyPeriod;
        double penalty = periods * penaltyPerPeriod;
        if (penalty > maxPenalty) penalty = maxPenalty;
        return 1.0 - penalty;
    }

    // Override ToString for a useful flight summary.
    public override string ToString()
    {
        return $"Flight {FlightNumber} ({Type}, {Priority}) - Scheduled: {ScheduledLandingTime} (Original: {OriginalScheduledLandingTime}), " +
               $"Passengers: {Passengers}, Delay: {GetDelayTicks()} ticks";
    }
}
