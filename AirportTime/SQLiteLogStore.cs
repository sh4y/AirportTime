using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

/// <summary>
/// A separate class for SQLite-specific logic, maintaining a single, persistent connection.
/// Uses Microsoft.Data.Sqlite for cross-platform support.
/// </summary>
public class SQLiteLogStore : IDisposable
{
    private readonly string dbPath;
    private SqliteConnection connection;

    /// <summary>
    /// Creates a new <see cref="SQLiteLogStore"/> and opens one SQLite connection.
    /// </summary>
    /// <param name="databasePath">
    /// Path to the SQLite database file (e.g., "GameLogs.db").
    /// </param>
    public SQLiteLogStore(string databasePath)
    {
        dbPath = databasePath;
        InitializeConnection();
        InitializeDatabase();
    }

    /// <summary>
    /// Opens a single SQLite connection for the lifecycle of this object using Microsoft.Data.Sqlite.
    /// </summary>
    private void InitializeConnection()
    {
        // Microsoft.Data.Sqlite typically does not need "Version=3;" or other flags.
        // By default, "Data Source={dbPath}" is enough to create/open the file.
        connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
    }

    /// <summary>
    /// Ensures the Logs table exists in the SQLite database.
    /// </summary>
    private void InitializeDatabase()
    {
        // Microsoft.Data.Sqlite uses SqliteCommand instead of SQLiteCommand.
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Logs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LogEntry TEXT NOT NULL,
                    LogTime TEXT NOT NULL
                )
            ";
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Inserts a log message into the database.
    /// </summary>
    /// <param name="message">The message to insert.</param>
    public void InsertLog(string message)
    {
        // In multithreaded scenarios, consider thread safety or locking around DB operations.
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                INSERT INTO Logs (LogEntry, LogTime) 
                VALUES (@entry, @time)
            ";
            cmd.Parameters.AddWithValue("@entry", message);
            cmd.Parameters.AddWithValue("@time", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Closes the open SQLite connection when disposing.
    /// </summary>
    public void Dispose()
    {
        if (connection != null)
        {
            connection.Close();
            connection.Dispose();
            connection = null;
        }
    }
}
