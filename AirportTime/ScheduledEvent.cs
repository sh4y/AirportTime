// Delegate for simulation events

// Represents an event scheduled at a specific tick.
public class ScheduledEvent
{
    public int ScheduledTick { get; }
    public SimulationEventAction Action { get; }

    public ScheduledEvent(int scheduledTick, SimulationEventAction action)
    {
        ScheduledTick = scheduledTick;
        Action = action;
    }
}
