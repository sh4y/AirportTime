public enum FailureType
{
    EmergencyFlightResponse, // Failed to respond to emergency in time
    RunwayClosure,           // Runway became unusable due to extreme wear
    CriticalDelay,           // Flight was critically delayed
    FlightCancellation,      // Flight had to be cancelled
    FinancialShortfall       // Treasury went below critical threshold
}