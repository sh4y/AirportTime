/// <summary>
/// Represents a single treasury transaction.
/// </summary>
public class TreasuryTransaction
{
    public CurrencyType Currency { get; set; }
    public double Amount { get; set; }
    public string SourceOrReason { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime TimeStamp { get; set; }
    public double NewBalance { get; set; }
    public bool OverdraftOccurred { get; set; }  // For the overdraft scenario
}