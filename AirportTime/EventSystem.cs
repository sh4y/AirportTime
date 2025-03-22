namespace AirportTime;

public class EventSystem : IEventSystem
{
    private readonly IRandomGenerator randomGenerator;
    private readonly List<IEvent> possibleEvents = new List<IEvent>();
    private readonly GameLogger gameLogger;

    // IRandomGenerator is injected.
    public EventSystem(IRandomGenerator rng, GameLogger logger)
    {
        randomGenerator = rng;
        gameLogger = logger;
    }

    public void RegisterEvent(IEvent gameEvent)
    {
        possibleEvents.Add(gameEvent);
    }

    public void TriggerRandomEvent(Flight flight)
    {
        // Choose a random event from the list.
        if (possibleEvents.Count == 0)
            return;

        int index = randomGenerator.Next(0, possibleEvents.Count);
        possibleEvents[index].Trigger();
    }

    public void TriggerDelayEvent(Flight flight, int delayTicks = 5, string reason = "Delay", int currentTick = 0)
    {
        // Create and trigger a FlightDelayEvent with specified delay and reason
        new FlightDelayEvent(flight, gameLogger, delayTicks, reason).Trigger();
    }
}