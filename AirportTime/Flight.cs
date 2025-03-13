using System;

public class Flight
{
    public string FlightNumber { get; private set; }
    public Plane Plane { get; private set; }
    public FlightType Type { get; private set; }
    public FlightPriority Priority { get; private set; }
    
    public int ScheduledLandingTime { get; private set; }
    public int OriginalScheduledLandingTime { get; private set; }
    public int Passengers { get; private set; }

    // Flight state
    public FlightStatus Status { get; private set; } = FlightStatus.Scheduled;

    // Example threshold: If total delay ticks exceed this, cancel automatically
    private readonly int _cancelDelayThreshold = 30;

    public Flight(string flightNumber, Plane plane, FlightType type, FlightPriority priority, int scheduledLandingTime, int passengers)
    {
        FlightNumber = flightNumber;
        Plane = plane;
        Type = type;
        Priority = priority;
        ScheduledLandingTime = scheduledLandingTime;
        OriginalScheduledLandingTime = scheduledLandingTime;  // keep track of the original landing time
        Passengers = passengers;
    }

    public bool AttemptLanding(Runway runway)
    {
        // If canceled or already landed, we don't proceed with landing
        if (Status == FlightStatus.Canceled)
        {
            Console.WriteLine($"Flight {FlightNumber} is canceled and cannot land.");
            return false;
        }

        if (Status == FlightStatus.Landed)
        {
            Console.WriteLine($"Flight {FlightNumber} has already landed.");
            return false;
        }

        if (runway.CanLand(Plane))
        {
            Console.WriteLine($"Flight {FlightNumber} can land.");
            Status = FlightStatus.Landed;
            return true;
        }
        else
        {
            Console.WriteLine($"Flight {FlightNumber} cannot land; no suitable runway available.");
            return false;
        }
    }

    /// <summary>
    /// Increments the scheduled landing time by the given delay ticks. 
    /// Automatically cancels the flight if total delay exceeds the threshold.
    /// </summary>
    public void Delay(int delayTicks)
    {
        // If already canceled or landed, no further updates
        if (Status == FlightStatus.Canceled || Status == FlightStatus.Landed) return;

        ScheduledLandingTime += delayTicks;
        Status = FlightStatus.Delayed;  // Set status to Delayed

        // Check if total delay passes the cancellation threshold
        if (GetDelayTicks() > _cancelDelayThreshold)
        {
            CancelFlight($"Exceeded cancellation threshold of {_cancelDelayThreshold} delay ticks.");
        }
    }

    /// <summary>
    /// Cancels the flight and prevents further attempts to land.
    /// </summary>
    public void CancelFlight(string reason)
    {
        if (Status == FlightStatus.Canceled) return;

        Status = FlightStatus.Canceled;
        Console.WriteLine($"Flight {FlightNumber} canceled. Reason: {reason}");
        
        //@todo: add punishment
    }

    // Returns the total number of ticks the flight is delayed.
    public int GetDelayTicks(int currentTick = 0)
    {
        // Get the scheduled delay (from explicit delays)
        int scheduledDelay = ScheduledLandingTime - OriginalScheduledLandingTime;
        
        // If we have a current tick and haven't landed/been canceled, add any additional delay
        if (currentTick > 0 && Status != FlightStatus.Landed && Status != FlightStatus.Canceled)
        {
            int additionalDelay = Math.Max(0, currentTick - ScheduledLandingTime);
            return scheduledDelay + additionalDelay;
        }

        return scheduledDelay;
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
        return $"Flight {FlightNumber} ({Type}, {Priority}) - " +
               $"Scheduled: {ScheduledLandingTime} (Original: {OriginalScheduledLandingTime}), " +
               $"Passengers: {Passengers}, Delay: {GetDelayTicks()} ticks, " +
               $"Status: {Status}";
    }
}
