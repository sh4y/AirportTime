public class FlightDelayEvent : IEvent
{
    public string EventName { get; private set; }
    public Flight Flight { get; private set; }
    public int DelayTicks { get; private set; }
    private readonly GameLogger gameLogger;

    // Optional: Store a reason or cause for clarity in logs
    public string Reason { get; private set; }

    public FlightDelayEvent(Flight flight, GameLogger logger, int delayTicks = 5, string reason = "Delay")
    {
        Flight = flight;
        DelayTicks = delayTicks;
        Reason = reason;
        EventName = "Flight Delay Event";
        gameLogger = logger;
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
        // Log before delay
        gameLogger.Log($"Delaying Flight {Flight.FlightNumber} by {DelayTicks} ticks. " +
                      $"Current scheduled time: {Flight.ScheduledLandingTime}, " +
                      $"Original time: {Flight.OriginalScheduledLandingTime}, " +
                      $"Current status: {Flight.Status}. " +
                      $"Reason: {Reason}");

        // Update the flight's landing schedule
        Flight.Delay(DelayTicks);

        // Log after delay
        gameLogger.Log($"Flight {Flight.FlightNumber} delayed. " +
                      $"New scheduled time: {Flight.ScheduledLandingTime}, " +
                      $"Total delay: {Flight.GetDelayTicks()} ticks, " +
                      $"New status: {Flight.Status}");
    }
}
