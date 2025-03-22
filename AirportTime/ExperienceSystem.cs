using System;
using System.Collections.Generic;

/// <summary>
/// Manages the airport's experience points and level progression
/// </summary>
public class ExperienceSystem
{
    private readonly GameLogger logger;
    
    // Current state
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;
    
    // Experience required for each level
    private readonly Dictionary<int, int> levelRequirements = new Dictionary<int, int>();
    
    // Event for level up notifications
    public event Action<int> OnLevelUp;
    
    public ExperienceSystem(GameLogger logger)
    {
        this.logger = logger;
        InitializeLevelRequirements();
    }
    
    private void InitializeLevelRequirements()
    {
        // Level 1 starts at 0 XP
        levelRequirements[1] = 0;
        levelRequirements[2] = 825;   // ~20 flights to reach level 2 (at ~41 XP per flight)
        levelRequirements[3] = 3500;  // ~50 total flights to reach level 3
        
        // Generate additional levels using the formula: previous × 1.8
        for (int level = 4; level <= 20; level++)
        {
            levelRequirements[level] = (int)(levelRequirements[level - 1] * 1.8);
        }
    }
    
    /// <summary>
    /// Adds experience points and checks for level up
    /// </summary>
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        
        int previousXP = CurrentXP;
        int previousLevel = CurrentLevel;
        
        CurrentXP += amount;
        logger.Log($"Airport gained {amount} XP. Total: {CurrentXP} XP");
        
        // Check for level up
        while (CurrentLevel < levelRequirements.Count && 
               CurrentXP >= GetRequiredXPForNextLevel())
        {
            CurrentLevel++;
            logger.Log($"LEVEL UP! Airport is now level {CurrentLevel}");
            
            // Trigger level up event
            OnLevelUp?.Invoke(CurrentLevel);
        }
        
        // Log progress to next level if not at max level
        if (CurrentLevel < levelRequirements.Count)
        {
            int xpToNextLevel = GetRequiredXPForNextLevel() - CurrentXP;
            logger.Log($"XP needed for level {CurrentLevel + 1}: {xpToNextLevel}");
        }
    }
    
    /// <summary>
    /// Returns XP required to reach the next level
    /// </summary>
    public int GetRequiredXPForNextLevel()
    {
        int nextLevel = CurrentLevel + 1;
        if (levelRequirements.TryGetValue(nextLevel, out int requiredXP))
        {
            return requiredXP;
        }
        
        // If next level isn't defined, use last defined level's requirement
        return levelRequirements[levelRequirements.Count];
    }
    
    /// <summary>
    /// Returns XP required for a specific level
    /// </summary>
    public int GetRequiredXPForLevel(int level)
    {
        if (levelRequirements.TryGetValue(level, out int requiredXP))
        {
            return requiredXP;
        }
        
        // If level isn't defined, return -1
        return -1;
    }
    
    // Add these properties and methods to the ExperienceSystem class

/// <summary>
/// Global XP multiplier applied to all flight XP calculations
/// </summary>
private double xpMultiplier = 1.0;

/// <summary>
/// Adds or updates the global XP multiplier
/// </summary>
/// <param name="multiplier">New multiplier value</param>
public void AddXPMultiplier(double multiplier)
{
    xpMultiplier = multiplier;
    logger.Log($"XP multiplier set to {xpMultiplier:F2}x");
}

// Modify the CalculateFlightXP method to include the XP multiplier:
public int CalculateFlightXP(Flight flight, Weather weather, int runwayWear, bool onTime, bool perfectLanding, int simultaneousFlights)
{
    // Base XP by flight type
    int baseXP = flight.Type switch
    {
        FlightType.Commercial => 10,
        FlightType.Cargo => 15,
        FlightType.VIP => 25,
        FlightType.Emergency => 40,
        _ => 10
    };
    
    // Multipliers
    double sizeMultiplier = flight.Plane.Size switch
    {
        PlaneSize.Small => 1.0,
        PlaneSize.Medium => 1.5,
        PlaneSize.Large => 2.5,
        _ => 1.0
    };
    
    double priorityMultiplier = flight.Priority switch
    {
        FlightPriority.Standard => 1.0,
        FlightPriority.VIP => 1.5,
        FlightPriority.Emergency => 2.0,
        _ => 1.0
    };
    
    double weatherMultiplier = weather.CurrentWeather switch
    {
        WeatherType.Clear => 1.0,
        WeatherType.Rainy => 1.2,
        WeatherType.Foggy => 1.5,
        WeatherType.Snowy => 1.8,
        WeatherType.Stormy => 2.5,
        _ => 1.0
    };
    
    // Runway wear multiplier (scales from 1.0 to 1.3 as wear increases)
    double runwayWearMultiplier = 1.0 + (runwayWear / 100.0 * 0.3);
    
    // Calculate total XP with multipliers
    double calculatedXP = baseXP * sizeMultiplier * priorityMultiplier * weatherMultiplier * runwayWearMultiplier;
    
    // Add situational bonuses
    if (onTime) calculatedXP += 5;
    if (perfectLanding) calculatedXP += 10;
    
    // Bonus for handling multiple flights simultaneously
    calculatedXP += simultaneousFlights * 15;
    
    // Round to nearest integer and ensure minimum of base XP
    int finalXP = Math.Max((int)Math.Round(calculatedXP), baseXP);
    
    // Apply global XP multiplier from achievements
    finalXP = (int)(finalXP * xpMultiplier);
    
    // Log XP calculation details for debugging/transparency
    logger.Log($"XP Earned: {finalXP} [Base: {baseXP}, Size: x{sizeMultiplier:F1}, " +
               $"Priority: x{priorityMultiplier:F1}, Weather: x{weatherMultiplier:F1}, " +
               $"Runway: x{runwayWearMultiplier:F2}, Global: x{xpMultiplier:F2}]");
    
    return finalXP;
}
    
    /// <summary>
    /// Returns progress percentage to next level (0-100)
    /// </summary>
    public int GetLevelProgressPercentage()
    {
        if (CurrentLevel >= levelRequirements.Count)
        {
            return 100; // Max level reached
        }
        
        int currentLevelXP = levelRequirements[CurrentLevel];
        int nextLevelXP = levelRequirements[CurrentLevel + 1];
        int xpInCurrentLevel = CurrentXP - currentLevelXP;
        int xpRequiredForNextLevel = nextLevelXP - currentLevelXP;
        
        return (int)((double)xpInCurrentLevel / xpRequiredForNextLevel * 100);
    }
    
    /// <summary>
    /// Returns a string representation of the current XP and level
    /// </summary>
    public string GetXPStatusString()
    {
        if (CurrentLevel >= levelRequirements.Count)
        {
            return $"Level {CurrentLevel} (MAX) - {CurrentXP} XP";
        }
        
        return $"Level {CurrentLevel} - {CurrentXP}/{GetRequiredXPForNextLevel()} XP ({GetLevelProgressPercentage()}%)";
    }
    
}