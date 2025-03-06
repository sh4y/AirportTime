
// Game simulation class to simulate the purchase of items and running the airport
// Flight class to represent flights and handle landing procedure
// Plane class to represent planes that will land on the runways
// Treasury class to manage player's gold balance
public class Treasury
{
    private double balance;
    private double goldPerTick;

    public Treasury(double startingBalance, double goldPerTick = 1)
    {
        balance = startingBalance;
        this.goldPerTick = goldPerTick;  // Amount of gold earned per tick
    }

    public double GetBalance() => balance;

    public void AddFunds(double amount, string source)
    {
        balance += amount;
        Console.WriteLine($"Added {amount:C} to treasury from {source}. Total: {balance:C}");
    }

    public bool DeductFunds(double amount, string reason)
    {
        if (balance >= amount)
        {
            balance -= amount;
            Console.WriteLine($"Deducted {amount:C} for {reason}. Total: {balance:C}");
            return true;
        }
        else
        {
            Console.WriteLine($"Not enough funds to deduct {amount:C}. Total: {balance:C}");
            return false;
        }
    }

    // Accumulate gold over time (per tick)
    public void AccumulateGold()
    {
        AddFunds(goldPerTick, "Gold earned over time");
    }
}
