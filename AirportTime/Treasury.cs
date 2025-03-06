
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
// Treasury class to manage player's gold balance
public class Treasury
{
    private double balance;
    private readonly double goldPerTick;
    private readonly GameLogger gameLogger;

    public Treasury(GameLogger gameLogger, double startingBalance = 90, double goldPerTick = 3)
    {
        this.balance = startingBalance;
        this.goldPerTick = goldPerTick;
        this.gameLogger = gameLogger;
    }

    public double GetBalance() => balance;

    public void AddFunds(double amount, string source)
    {
        balance += amount;
        gameLogger.Log($"Added {amount} gold from {source}. New balance: {balance}");
    }

    public bool DeductFunds(double amount, string reason)
    {
        if (balance >= amount)
        {
            balance -= amount;
            gameLogger.Log($"Deducted {amount} gold for {reason}. New balance: {balance}");
            return true;
        }
        gameLogger.Log($"Failed to deduct {amount} gold for {reason}. Insufficient balance: {balance}");
        return false;
    }

    // Accumulates gold per tick.
    public void AccumulateGold()
    {
        balance += goldPerTick;
        gameLogger.Log($"Accumulated {goldPerTick} gold. New balance: {balance}");
    }
}
