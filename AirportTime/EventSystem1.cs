public class EventSystem
{
    private readonly IRandomGenerator randomGenerator;
    private readonly List<IEvent> possibleEvents = new List<IEvent>();

    // IRandomGenerator is injected.
    public EventSystem(IRandomGenerator rng)
    {
        randomGenerator = rng;
    }

    public void RegisterEvent(IEvent gameEvent)
    {
        possibleEvents.Add(gameEvent);
    }

    public void TriggerRandomEvent(Airport airport)
    {
        // Choose a random event from the list.
        if (possibleEvents.Count == 0)
            return;

        int index = randomGenerator.Next(0, possibleEvents.Count);
        possibleEvents[index].Trigger(airport);
    }

    public void TriggerDelayEvent(Flight flight)
    {
        // For example: create and trigger a FlightDelayEvent.
        new FlightDelayEvent(flight).Trigger(null); // or pass airport if available
    }
}
