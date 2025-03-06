// Delegate for simulation events
public delegate void SimulationEventAction(int tick);

// EventScheduler class manages scheduled events.
public class EventScheduler
{
    private List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();

    // Schedule a new event
    public void ScheduleEvent(ScheduledEvent scheduledEvent)
    {
        scheduledEvents.Add(scheduledEvent);
    }

    // Process and execute events scheduled for the current tick
    public void ProcessEvents(int currentTick)
    {
        // Get all events scheduled for the current tick
        var eventsToProcess = scheduledEvents.Where(e => e.ScheduledTick == currentTick).ToList();
        foreach (var ev in eventsToProcess)
        {
            ev.Action(currentTick);
            scheduledEvents.Remove(ev);
        }
    }
}
