
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
public class GameLogger
{
    private List<string> logEntries;

    public GameLogger()
    {
        logEntries = new List<string>();
    }

    public void Log(string message)
    {
        logEntries.Add(message);
        Console.WriteLine(message);  // Output to console as well
    }

    // Display all logs
    public void DisplayLogs()
    {
        Console.WriteLine("Game Logs:");
        foreach (var log in logEntries)
        {
            Console.WriteLine(log);
        }
    }

    // Display the most recent x logs
    public void DisplayRecentLogs(int count)
    {
        Console.WriteLine("Recent Logs:");
        int startIndex = logEntries.Count > count ? logEntries.Count - count : 0;
        for (int i = startIndex; i < logEntries.Count; i++)
        {
            Console.WriteLine(logEntries[i]);
        }
    }
}
