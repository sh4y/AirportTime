using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

/// <summary>
/// Stores <see cref="TreasuryTransaction"/> records in a separate SQLite database 
/// using Microsoft.Data.Sqlite for cross-platform compatibility.
/// </summary>
public class TransactionLogStore : IDisposable
{
    private readonly string dbPath;
    private SqliteConnection connection;

    /// <summary>
    /// Creates a new <see cref="TransactionLogStore"/> with a persistent connection.
    /// </summary>
    /// <param name="databasePath">
    /// The path to the SQLite file (e.g., "Transactions.db").
    /// </param>
    public TransactionLogStore(string databasePath = "Transactions.db")
    {
        dbPath = databasePath;
        InitializeConnection();
        InitializeDatabase();
    }

    private void InitializeConnection()
    {
        // Microsoft.Data.Sqlite typically just needs Data Source=...
        // No version or special flags needed.
        connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
    }

    private void InitializeDatabase()
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS TreasuryTransactions (
                    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                    Currency        TEXT    NOT NULL,
                    Amount          REAL    NOT NULL,
                    SourceOrReason  TEXT    NOT NULL,
                    TransactionType TEXT    NOT NULL,
                    TimeStamp       TEXT    NOT NULL,
                    NewBalance      REAL    NOT NULL,
                    Overdraft       INTEGER NOT NULL
                )
            ";
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Inserts a TreasuryTransaction record into the separate DB.
    /// </summary>
    /// <param name="transaction">The transaction to insert.</param>
    public void InsertTransaction(TreasuryTransaction transaction)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                INSERT INTO TreasuryTransactions
                (Currency, Amount, SourceOrReason, TransactionType, TimeStamp, NewBalance, Overdraft)
                VALUES (@currency, @amount, @reason, @txType, @timeStamp, @newBalance, @overdraft)
            ";
            cmd.Parameters.AddWithValue("@currency", transaction.Currency.ToString());
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@reason", transaction.SourceOrReason);
            cmd.Parameters.AddWithValue("@txType", transaction.TransactionType.ToString());
            cmd.Parameters.AddWithValue("@timeStamp", transaction.TimeStamp.ToString("o"));
            cmd.Parameters.AddWithValue("@newBalance", transaction.NewBalance);
            cmd.Parameters.AddWithValue("@overdraft", transaction.OverdraftOccurred ? 1 : 0);

            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// (Optional) Retrieves all transactions from the DB for debugging or display.
    /// </summary>
    public List<TreasuryTransaction> GetAllTransactions()
    {
        var list = new List<TreasuryTransaction>();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM TreasuryTransactions";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var transaction = new TreasuryTransaction
                    {
                        Currency = Enum.Parse<CurrencyType>(reader.GetString(reader.GetOrdinal("Currency"))),
                        Amount = reader.GetDouble(reader.GetOrdinal("Amount")),
                        SourceOrReason = reader.GetString(reader.GetOrdinal("SourceOrReason")),
                        TransactionType = Enum.Parse<TransactionType>(reader.GetString(reader.GetOrdinal("TransactionType"))),
                        TimeStamp = DateTime.Parse(reader.GetString(reader.GetOrdinal("TimeStamp"))),
                        NewBalance = reader.GetDouble(reader.GetOrdinal("NewBalance")),
                        OverdraftOccurred = reader.GetInt32(reader.GetOrdinal("Overdraft")) == 1
                    };
                    list.Add(transaction);
                }
            }
        }
        return list;
    }
    /// <summary>
    /// Retrieves the most recent transactions
    /// </summary>
    /// <param name="count">Number of transactions to retrieve</param>
    /// <returns>List of transactions in reverse chronological order</returns>
    public List<TreasuryTransaction> GetRecentTransactions(int count)
    {
        var transactions = new List<TreasuryTransaction>();
        
        try
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT * FROM TreasuryTransactions
                    ORDER BY TimeStamp DESC
                    LIMIT @count
                ";
                cmd.Parameters.AddWithValue("@count", count);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var transaction = new TreasuryTransaction
                        {
                            Currency = Enum.Parse<CurrencyType>(reader.GetString(reader.GetOrdinal("Currency"))),
                            Amount = reader.GetDouble(reader.GetOrdinal("Amount")),
                            SourceOrReason = reader.GetString(reader.GetOrdinal("SourceOrReason")),
                            TransactionType = Enum.Parse<TransactionType>(reader.GetString(reader.GetOrdinal("TransactionType"))),
                            TimeStamp = DateTime.Parse(reader.GetString(reader.GetOrdinal("TimeStamp"))),
                            NewBalance = reader.GetDouble(reader.GetOrdinal("NewBalance")),
                            OverdraftOccurred = reader.GetInt32(reader.GetOrdinal("Overdraft")) == 1
                        };
                        transactions.Add(transaction);
                    }
                }
            }
        }
        catch
        {
            // Return empty list if there's an error
        }
        
        return transactions;
    }
    /// <summary>
    /// Closes the SQLite connection.
    /// </summary>
    public void Dispose()
    {
        connection?.Close();
        connection?.Dispose();
        connection = null;
    }
}
