public class AirportMetrics
{
    private readonly Airport airport;

    public AirportMetrics(Airport airport)
    {
        this.airport = airport;
    }

    public string Name => airport.Name;
    public double GoldBalance => airport.Treasury.GetBalance();
    public string CurrentLandingMode => airport.LandingManager.CurrentLandingMode.ToString();
    public int ActiveFlightsCount => airport.FlightScheduler.GetUnlandedFlights().Count;
    public bool IsGameOver => airport.IsGameOver;
    public FailureType? GameOverReason => airport.GameOverReason;
    
    // New properties
    public double HourlyRevenue => airport.Treasury.GoldPerTick * 6 * 60; // 6 ticks per minute * 60 mins
    public double DailyGrowthRate => CalculateDailyGrowthRate();
    public int AirportLevel => airport.ExperienceSystem.CurrentLevel;
    public string AirportTier => GetAirportTierDescription();
    
    private double CalculateDailyGrowthRate()
    {
        // Implementation based on recent financial data
        return 0.05; // Placeholder 5% daily growth
    }
    
    private string GetAirportTierDescription()
    {
        return airport.ExperienceSystem.CurrentLevel switch
        {
            1 => "Regional Airport",
            2 => "National Airport",
            3 => "International Hub",
            4 => "Continental Gateway",
            5 => "Global Transportation Center",
            _ => "Premium Aviation Network"
        };
    }

    public GameTimeInfo GetTimeInfo(int currentTick)
    {
        int gameDays = currentTick / (24 * 60 / 10);
        int gameHours = (currentTick % (24 * 60 / 10)) / (60 / 10);
        int gameMinutes = (currentTick % (60 / 10)) * 10;
        
        return new GameTimeInfo
        {
            Days = gameDays,
            Hours = gameHours,
            Minutes = gameMinutes,
            FormattedTime = $"{gameDays}d {gameHours:D2}:{gameMinutes:D2}",
            TotalTicks = currentTick
        };
    }

    public string GetWeatherInfo()
    {
        // We'd need to expose Weather from Airport
        return "Clear"; // Placeholder until we properly expose Weather
    }
}