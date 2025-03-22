using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tracks failures and determines when the game is over
/// </summary>
public class FailureTracker
{
    private Dictionary<FailureType, int> failureCounts = new Dictionary<FailureType, int>();
    private Dictionary<FailureType, int> failureThresholds = new Dictionary<FailureType, int>();
    
    public event Action<FailureType> OnGameOver;
    private readonly GameLogger logger;
    
    public FailureTracker(GameLogger logger)
    {
        this.logger = logger;
        InitializeFailureCounts();
        InitializeFailureThresholds();
    }
    
    private void InitializeFailureCounts()
    {
        foreach (FailureType failureType in Enum.GetValues(typeof(FailureType)))
        {
            failureCounts[failureType] = 0;
        }
    }
    
    private void InitializeFailureThresholds()
    {
        failureThresholds[FailureType.EmergencyFlightResponse] = 3;
        failureThresholds[FailureType.RunwayClosure] = 5;
        failureThresholds[FailureType.CriticalDelay] = 10;
        failureThresholds[FailureType.FlightCancellation] = 7;
        failureThresholds[FailureType.FinancialShortfall] = 3;
    }
    
    /// <summary>
    /// Records a failure of a specific type and checks if game over condition is met
    /// </summary>
    public void RecordFailure(FailureType failureType, string details)
    {
        failureCounts[failureType]++;
        
        logger.Log($"❌ Failure recorded: {failureType} - {details}. Total: {failureCounts[failureType]}/{failureThresholds[failureType]}");
        
        if (failureCounts[failureType] >= failureThresholds[failureType])
        {
            OnGameOver?.Invoke(failureType);
        }
    }
    
    /// <summary>
    /// Gets the current count for a specific failure type
    /// </summary>
    public int GetFailureCount(FailureType failureType) => failureCounts[failureType];
    
    /// <summary>
    /// Gets the threshold for a specific failure type
    /// </summary>
    public int GetFailureThreshold(FailureType failureType) => failureThresholds[failureType];
    
    /// <summary>
    /// Gets all current failure counts
    /// </summary>
    public Dictionary<FailureType, int> GetAllFailureCounts() => new Dictionary<FailureType, int>(failureCounts);
    
    /// <summary>
    /// Gets percentage to game over for a specific failure type (0-100)
    /// </summary>
    public int GetFailurePercentage(FailureType failureType)
    {
        int count = GetFailureCount(failureType);
        int threshold = GetFailureThreshold(failureType);
        
        return (int)Math.Min(100, (count * 100.0) / threshold);
    }
}