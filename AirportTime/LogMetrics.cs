public class LogMetrics
{
    private readonly GameLogger logger;

    public LogMetrics(GameLogger logger)
    {
        this.logger = logger;
    }

    public List<string> GetRecentLogs(int count)
    {
        // We'd need to modify GameLogger to expose the log entries
        // This is a placeholder until that functionality is added
        return new List<string>(); 
    }
}