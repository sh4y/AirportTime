namespace AirportTime;

/// <summary>
/// Interface for the airport view controller
/// </summary>
public interface IAirportViewController
{
    /// <summary>
    /// Updates the view with the current state of the airport
    /// </summary>
    /// <param name="currentTick">Current game tick</param>
    void UpdateView(int currentTick);
    
    /// <summary>
    /// Prompts the user to select a runway for a flight
    /// </summary>
    /// <param name="flight">The flight requiring landing</param>
    /// <param name="availableRunways">Available runways</param>
    /// <returns>Selected runway or null</returns>
    Runway PromptRunwaySelection(Flight flight, List<Runway> availableRunways);
    
    /// <summary>
    /// Shows a notification message to the user
    /// </summary>
    /// <param name="message">The message to show</param>
    /// <param name="isImportant">Whether the message is important</param>
    void ShowNotification(string message, bool isImportant = false);
}