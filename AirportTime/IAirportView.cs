using System.Collections.Generic;

/// <summary>
/// Interface for view components that display airport information.
/// This creates a clean separation between game logic and UI presentation,
/// making it easier to replace the console UI with Unity UI later.
/// </summary>
public interface IAirportView
{
    /// <summary>
    /// Updates the header information display
    /// </summary>
    /// <param name="airportName">Name of the airport</param>
    /// <param name="timeInfo">Current game time information</param>
    /// <param name="gold">Current gold balance</param>
    /// <param name="weatherInfo">Current weather information</param>
    void UpdateHeaderInfo(string airportName, GameTimeInfo timeInfo, double gold, string weatherInfo);

    /// <summary>
    /// Updates the landing mode and active flights information
    /// </summary>
    /// <param name="landingMode">Current landing mode (Automatic/Manual)</param>
    /// <param name="activeFlightCount">Number of active flights</param>
    void UpdateLandingInfo(string landingMode, int activeFlightCount);

    /// <summary>
    /// Displays emergency flights requiring immediate attention
    /// </summary>
    /// <param name="emergencies">List of emergency flight information</param>
    /// <param name="currentTick">Current game tick</param>
    void DisplayEmergencies(List<EmergencyInfo> emergencies, int currentTick);

    /// <summary>
    /// Updates the experience and level display
    /// </summary>
    /// <param name="currentLevel">Current airport level</param>
    /// <param name="currentXP">Current XP amount</param>
    /// <param name="nextLevelXP">XP required for next level</param>
    /// <param name="progressPercentage">Percentage progress to next level</param>
    /// <param name="timeEstimate">Estimated time to next level</param>
    /// <param name="efficiencyBonus">Current efficiency bonus</param>
    /// <param name="nextUnlock">Description of next unlock</param>
    void UpdateExperienceDisplay(int currentLevel, int currentXP, int nextLevelXP, 
                               int progressPercentage, string timeEstimate, 
                               int efficiencyBonus, string nextUnlock);

    /// <summary>
    /// Displays upcoming flights information
    /// </summary>
    /// <param name="upcomingFlights">List of upcoming flights</param>
    void DisplayUpcomingFlights(List<FlightInfo> upcomingFlights);

    /// <summary>
    /// Updates the failures display
    /// </summary>
    /// <param name="failures">List of current failure information</param>
    void UpdateFailuresDisplay(List<FailureInfo> failures);

    /// <summary>
    /// Updates the active modifiers display
    /// </summary>
    /// <param name="modifiers">List of active modifier information</param>
    void UpdateModifiersDisplay(List<ModifierInfo> modifiers);

    /// <summary>
    /// Updates the runways display
    /// </summary>
    /// <param name="runways">List of runway information</param>
    void UpdateRunwaysDisplay(List<RunwayInfo> runways);

    /// <summary>
    /// Updates the achievements display
    /// </summary>
    /// <param name="achievements">List of recent achievements</param>
    /// <param name="totalUnlocked">Total number of unlocked achievements</param>
    void UpdateAchievementsDisplay(List<Achievement> achievements, int totalUnlocked);

    /// <summary>
    /// Updates the recent activity logs display
    /// </summary>
    /// <param name="recentLogs">List of recent log messages</param>
    void UpdateLogsDisplay(List<string> recentLogs);

    /// <summary>
    /// Displays the available controls/commands
    /// </summary>
    void DisplayControls();

    /// <summary>
    /// Shows a notification message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="isImportant">Whether the message is important (e.g., for errors)</param>
    void ShowNotification(string message, bool isImportant = false);

    /// <summary>
    /// Prompts the user to select a runway for manual landing
    /// </summary>
    /// <param name="flight">The flight requiring landing</param>
    /// <param name="availableRunways">List of available runways</param>
    /// <returns>The selected runway or null if delayed</returns>
    Runway PromptRunwaySelection(Flight flight, List<Runway> availableRunways);

    /// <summary>
    /// Clears the display (for refreshing the UI)
    /// </summary>
    void ClearDisplay();
}