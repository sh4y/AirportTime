// Update the Flight class to add a IsSpecial property
public class Flight
{
    public string FlightNumber { get; private set; }
    public Plane Plane { get; private set; }
    public FlightType Type { get; private set; }
    public FlightPriority Priority { get; private set; }
    
    public int ScheduledLandingTime { get; private set; }
    public int OriginalScheduledLandingTime { get; private set; }
    public int Passengers { get; private set; }
    
    // New property to mark special flights
    public bool IsSpecial { get; private set; }

    // Flight state
    public FlightStatus Status { get; private set; } = FlightStatus.Scheduled;

    // Example threshold: If total delay ticks exceed this, cancel automatically
    private readonly int _cancelDelayThreshold = 30;

    public Flight(string flightNumber, Plane plane, FlightType type, FlightPriority priority, 
                  int scheduledLandingTime, int passengers, bool isSpecial = false)
    {
        FlightNumber = flightNumber;
        Plane = plane;
        Type = type;
        Priority = priority;
        ScheduledLandingTime = scheduledLandingTime;
        OriginalScheduledLandingTime = scheduledLandingTime;  // keep track of the original landing time
        Passengers = passengers;
        IsSpecial = isSpecial;
    }

    // Rest of the Flight class remains the same
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

    public void CancelFlight(string reason)
    {
        if (Status == FlightStatus.Canceled) return;

        Status = FlightStatus.Canceled;
        Console.WriteLine($"Flight {FlightNumber} canceled. Reason: {reason}");
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

    public double GetDelayPenaltyMultiplier(int ticksPerPenaltyPeriod, double penaltyPerPeriod, double maxPenalty)
    {
        int delayTicks = GetDelayTicks();
        int periods = delayTicks / ticksPerPenaltyPeriod;
        double penalty = periods * penaltyPerPeriod;
        if (penalty > maxPenalty) penalty = maxPenalty;
        return 1.0 - penalty;
    }

    public override string ToString()
    {
        string specialTag = IsSpecial ? " [SPECIAL]" : "";
        return $"Flight {FlightNumber} ({Type}, {Priority}){specialTag} - " +
               $"Scheduled: {ScheduledLandingTime} (Original: {OriginalScheduledLandingTime}), " +
               $"Passengers: {Passengers}, Delay: {GetDelayTicks()} ticks, " +
               $"Status: {Status}";
    }
}