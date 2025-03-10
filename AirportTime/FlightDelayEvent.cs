public class FlightDelayEvent : IEvent
{
    public string EventName { get; private set; }
    public Flight Flight { get; private set; }
    public int DelayTicks { get; private set; }

    // Optional: Store a reason or cause for clarity in logs
    public string Reason { get; private set; }

    public FlightDelayEvent(Flight flight, int delayTicks = 5, string reason = "Delay")
    {
        Flight = flight;
        DelayTicks = delayTicks;
        Reason = reason;
        EventName = "Flight Delay Event";
    }

    /// <summary>
    /// Trigger the delay:
    ///  - Increases the flight's landing time by DelayTicks
    ///  - Logs the event
    ///  - Re-schedules the flight
    /// </summary>
    /// <param name="airport">The airport for logging and scheduling context.</param>
    public void Trigger()
    {
        // Update the flight’s landing schedule
        Flight.Delay(DelayTicks);
    }
}
