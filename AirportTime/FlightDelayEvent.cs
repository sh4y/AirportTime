public class FlightDelayEvent : IEvent
{
    public string EventName { get; private set; }
    public Flight Flight { get; private set; }
    public int DelayTicks { get; private set; }

    public FlightDelayEvent(Flight flight, int delayTicks = 5)
    {
        Flight = flight;
        DelayTicks = delayTicks;
        EventName = "Flight Delay Event";
    }

    /// <summary>
    /// Trigger the delay event: delays the flight's landing time,
    /// logs the event, and optionally re-schedules the flight.
    /// </summary>
    /// <param name="airport">The airport context for logging and scheduling.</param>
    public void Trigger(Airport airport)
    {
        // Delay the flight by the specified number of ticks.
        Flight.Delay(DelayTicks);

        // Log the delay event.
        airport.GameLogger.Log($"Event: {EventName} triggered. Flight {Flight.FlightNumber} delayed by {DelayTicks} ticks. New landing time: {Flight.ScheduledLandingTime}.");

        // Optionally, re-schedule the flight with its updated landing time.
        airport.FlightScheduler.ScheduleFlight(Flight, Flight.ScheduledLandingTime);
    }
}
