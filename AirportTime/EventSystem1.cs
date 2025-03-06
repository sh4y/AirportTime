public class EventSystem
{
    private IRandomGenerator randomGenerator;
    private List<IEvent> possibleEvents;

    public EventSystem(IRandomGenerator rng)
    {
        randomGenerator = rng;
        possibleEvents = new List<IEvent>();
    }

    public void RegisterEvent(IEvent gameEvent)
    {
        possibleEvents.Add(gameEvent);
    }

    // Called periodically to trigger random events
    public void TriggerRandomEvent(Airport airport)
    {
        if (possibleEvents.Count == 0) return;

        int eventIndex = randomGenerator.Next(0, possibleEvents.Count);
        IEvent chosenEvent = possibleEvents[eventIndex];

        chosenEvent.Trigger(airport);
    }

    // Directly trigger specific event types (e.g., delays)
    public void TriggerDelayEvent(Flight flight)
    {
        var delayEvent = new FlightDelayEvent(flight);
        delayEvent.Trigger(null); // null if Airport isn't needed here
    }
}