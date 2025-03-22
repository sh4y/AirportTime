public class FlightInfo
{
    public string FlightNumber { get; set; }
    public FlightType FlightType { get; set; }
    public FlightPriority Priority { get; set; }
    public PlaneSize PlaneSize { get; set; }
    public int Passengers { get; set; }
    public int ScheduledLandingTime { get; set; }
    public double EstimatedRevenue { get; set; }
    public FlightStatus Status { get; set; }
    public bool IsEmergency { get; set; }
    public bool IsDelayed { get; set; }
}