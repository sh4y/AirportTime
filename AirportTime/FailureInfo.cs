public class FailureInfo
{
    public FailureType Type { get; set; }
    public int Count { get; set; }
    public int Threshold { get; set; }
    public int Percentage { get; set; }
    public string DangerLevel { get; set; }
}