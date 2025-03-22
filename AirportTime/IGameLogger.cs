namespace AirportTime;

/// <summary>
/// Interface for logging game events
/// </summary>
public interface IGameLogger
{
    /// <summary>
    /// Logs a message
    /// </summary>
    /// <param name="message">The message to log</param>
    void Log(string message);
    
    /// <summary>
    /// Displays the most recent logs
    /// </summary>
    /// <param name="count">Number of logs to display</param>
    void DisplayRecentLogs(int count);
    
    /// <summary>
    /// Gets the most recent log entries
    /// </summary>
    /// <param name="count">Number of logs to retrieve</param>
    /// <returns>List of recent log entries</returns>
    //List<string> GetRecentLogs(int count);
}