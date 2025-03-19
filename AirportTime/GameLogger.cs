using System;
using System.Collections.Generic;
using System.Data.SQLite;

/// <summary>
/// A logger that stores messages in memory, prints them to the console,
/// and writes logs to a SQLite database.
/// </summary>
public class GameLogger : IDisposable
{
    private readonly List<string> logEntries;
    private readonly SQLiteLogStore logStore;  // Always used
    /// <summary>
    /// Creates a new <see cref="GameLogger"/> that maintains a persistent SQLite connection.
    /// </summary>
    /// <param name="databasePath">The path to the SQLite database (e.g., "GameLogs.db").</param>
    /// <remarks>
    /// This constructor is not optional; you must provide a valid database path 
    /// to use <see cref="GameLogger"/>.
    /// </remarks>
    public GameLogger(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new ArgumentException("A valid SQLite database path is required.", nameof(databasePath));
        }

        logEntries = new List<string>();
        logStore = new SQLiteLogStore(databasePath);
    }

    /// <summary>
    /// Logs a message to in-memory storage, writes it to the console,
    /// and inserts it into the SQLite database.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        logEntries.Add(message);
        Console.WriteLine(message);
        logStore.InsertLog(message);
    }

    /// <summary>
    /// Displays all logs from in-memory storage (does not query the database).
    /// </summary>
    public void DisplayLogs()
    {
        Console.WriteLine("Game Logs:");
        foreach (var log in logEntries)
        {
            Console.WriteLine(log);
        }
    }

    /// <summary>
    /// Displays the most recent logs from in-memory storage (does not query the database).
    /// </summary>
    /// <param name="count">The number of logs to display.</param>
    public void DisplayRecentLogs(int count)
    {
        int startIndex = logEntries.Count > count ? logEntries.Count - count : 0;
        Console.WriteLine("Recent Logs:");
        for (int i = startIndex; i < logEntries.Count; i++)
        {
            Console.WriteLine(logEntries[i]);
        }
    }

    /// <summary>
    /// Disposes the underlying <see cref="SQLiteLogStore"/> and closes its connection.
    /// </summary>
    public void Dispose()
    {
        logStore.Dispose();
    }
}

