// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
// Treasury class to manage player's gold balance
public class Treasury
{
    // 1) Factor out magic numbers
    private const double DefaultStartingBalance = 1000.0;
    private const double DefaultGoldPerTick = 3.0;

    // Instead of a single "balance" field, we keep a dictionary for future multi-currency
    private readonly Dictionary<CurrencyType, double> balances 
        = new Dictionary<CurrencyType, double>();

    // 2) Thread safety lock
    private readonly object balanceLock = new object();

    // Additional references
    private readonly GameLogger gameLogger;
    private readonly TransactionLogStore transactionStore;

    // 6) Overdraft support toggled by a property rather than changing method signatures
    public bool OverdraftEnabled { get; set; } = false;

    // Keep a property for gold-per-tick
    public double GoldPerTick { get; }

    /// <summary>
    /// Creates a new Treasury with an optional starting balance and gold-per-tick rate.
    /// </summary>
    /// <param name="gameLogger">Logs messages to console/DB.</param>
    /// <param name="transactionStore">Stores each transaction in a separate SQLite DB.</param>
    /// <param name="startingBalance">Initial gold amount (defaults to 90).</param>
    /// <param name="goldPerTick">Gold gained each tick (defaults to 3).</param>
    public Treasury(GameLogger gameLogger,
                    TransactionLogStore transactionStore,
                    double startingBalance = DefaultStartingBalance,
                    double goldPerTick = DefaultGoldPerTick)
    {
        this.gameLogger = gameLogger;
        this.transactionStore = transactionStore;
        GoldPerTick = goldPerTick;

        // Initialize only one currency for now (Gold)
        balances[CurrencyType.Gold] = startingBalance;
    }

    /// <summary>
    /// Gets the current Gold balance. (Keeps the existing naming scheme.)
    /// </summary>
    public double GetBalance()
    {
        lock (balanceLock)
        {
            return balances[CurrencyType.Gold];
        }
    }

    /// <summary>
    /// Adds <paramref name="amount"/> gold to the Treasury, logs it, and stores the transaction.
    /// (Keeps the existing naming & signature.)
    /// </summary>
    /// <param name="amount">The amount of gold to add.</param>
    /// <param name="source">A string describing why we're adding funds.</param>
    public void AddFunds(double amount, string source)
    {
        lock (balanceLock)
        {
            var currentBalance = balances[CurrencyType.Gold];
            var newBalance = currentBalance + amount;
            balances[CurrencyType.Gold] = newBalance;

            gameLogger.Log($"Added {amount} gold from {source}. New balance: {newBalance}");

            // 3) Transaction History in separate DB
            var tx = new TreasuryTransaction
            {
                Currency = CurrencyType.Gold,
                Amount = amount,
                SourceOrReason = source,
                TransactionType = TransactionType.Add,
                TimeStamp = DateTime.UtcNow,
                NewBalance = newBalance,
                OverdraftOccurred = false
            };
            transactionStore.InsertTransaction(tx);
        }
    }

    /// <summary>
    /// Deducts <paramref name="amount"/> gold if sufficient funds or if overdraft is enabled.
    /// (Keeps the existing method name & signature.)
    /// </summary>
    /// <param name="amount">Gold to remove.</param>
    /// <param name="reason">Reason for deduction.</param>
    /// <returns>True if deducted (successful), false if not enough balance without overdraft.</returns>
    public bool DeductFunds(double amount, string reason)
    {
        lock (balanceLock)
        {
            var currentBalance = balances[CurrencyType.Gold];
            bool hasEnough = (currentBalance >= amount);

            if (hasEnough)
            {
                // Normal deduction
                double newBalance = currentBalance - amount;
                balances[CurrencyType.Gold] = newBalance;
                gameLogger.Log($"Deducted {amount} gold for {reason}. New balance: {newBalance}");

                var tx = new TreasuryTransaction
                {
                    Currency = CurrencyType.Gold,
                    Amount = amount,
                    SourceOrReason = reason,
                    TransactionType = TransactionType.Deduct,
                    TimeStamp = DateTime.UtcNow,
                    NewBalance = newBalance,
                    OverdraftOccurred = false
                };
                transactionStore.InsertTransaction(tx);

                return true;
            }
            else if (OverdraftEnabled)
            {
                // Overdraft scenario
                double oldBalance = currentBalance;
                double newBalance = currentBalance - amount; // goes negative
                balances[CurrencyType.Gold] = newBalance;

                gameLogger.Log($"[Overdraft] Deducted {amount} gold for {reason}. " +
                               $"New balance: {newBalance} (was {oldBalance}).");

                var tx = new TreasuryTransaction
                {
                    Currency = CurrencyType.Gold,
                    Amount = amount,
                    SourceOrReason = reason,
                    TransactionType = TransactionType.Deduct,
                    TimeStamp = DateTime.UtcNow,
                    NewBalance = newBalance,
                    OverdraftOccurred = true
                };
                transactionStore.InsertTransaction(tx);

                return true;
            }
            else
            {
                // Insufficient funds, no overdraft
                gameLogger.Log($"Failed to deduct {amount} gold for {reason}. Insufficient balance: {currentBalance}");
                return false;
            }
        }
    }

    /// <summary>
    /// Accumulates gold at each tick (Keeps existing naming & usage).
    /// </summary>
    public void AccumulateGold()
    {
        lock (balanceLock)
        {
            double currentBalance = balances[CurrencyType.Gold];
            double newBalance = currentBalance + GoldPerTick;
            balances[CurrencyType.Gold] = newBalance;

            //gameLogger.Log($"Accumulated {GoldPerTick} gold. New balance: {newBalance}");

            // Also store as a transaction
            var tx = new TreasuryTransaction
            {
                Currency = CurrencyType.Gold,
                Amount = GoldPerTick,
                SourceOrReason = "AccumulateGold",
                TransactionType = TransactionType.Add,
                TimeStamp = DateTime.UtcNow,
                NewBalance = newBalance,
                OverdraftOccurred = false
            };
            transactionStore.InsertTransaction(tx);
        }
    }
}
