/// <summary>
/// Interface for classes that provide runway information
/// </summary>
public interface IRunwayProvider
{
    /// <summary>
    /// Gets all available runways
    /// </summary>
    /// <returns>A list of all runways</returns>
    List<Runway> GetRunways();
}