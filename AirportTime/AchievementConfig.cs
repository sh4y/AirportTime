// AchievementConfig.cs - Configuration parameters for easy editing
public static class AchievementConfig
{
    // Flight Type Achievement thresholds
    public static readonly int[] FlightTypeThresholds = { 10, 30, 100, 500, 1000, 5000 };

    // Perfect Landing Achievement thresholds
    public static readonly int[] PerfectLandingThresholds = { 5, 20, 50, 200, 500, 1000 };

    // Runway Expert Achievement thresholds
    public static readonly int[] RunwayExpertThresholds = { 10, 30, 60, 120, 240, 500 };

    // Passenger Milestone Achievement configuration
    public static readonly int PassengerMilestoneBaseValue = 2;
    public static readonly int PassengerMilestoneMinExponent = 6;  // 2^6 = 64
    public static readonly int PassengerMilestoneMaxExponent = 20; // 2^20 = 1,048,576

    // Weather Master Achievement thresholds
    public static readonly Dictionary<WeatherType, int[]> WeatherMasterThresholds = new Dictionary<WeatherType, int[]>
    {
        { WeatherType.Rainy, new[] { 5, 15, 50, 150 } },
        { WeatherType.Foggy, new[] { 3, 10, 30, 100 } },
        { WeatherType.Snowy, new[] { 3, 10, 30, 100 } },
        { WeatherType.Stormy, new[] { 2, 5, 15, 50 } }
    };

    // Night Flight Achievement thresholds
    public static readonly int[] NightFlightThresholds = { 10, 25, 50, 100, 200 };

    // Consecutive Flight Achievement thresholds
    public static readonly int[] ConsecutiveFlightThresholds = { 5, 15, 30, 50, 100 };

    // Simultaneous Flight Achievement thresholds
    public static readonly int[] SimultaneousFlightThresholds = { 3, 5, 8, 12, 15 };

    // Emergency Landing Achievement thresholds
    public static readonly int[] EmergencyLandingThresholds = { 3, 10, 25, 50, 100 };

    // Milestone titles for passenger achievements
    public static readonly Dictionary<int, string> PassengerMilestoneTitles = new Dictionary<int, string>()
    {
        { 6, "First Steps" },           // 64 passengers
        { 7, "Taking Off" },            // 128 passengers
        { 8, "Regional Hub" },          // 256 passengers
        { 9, "People Mover" },          // 512 passengers
        { 10, "Terminal Bustle" },      // 1,024 passengers
        { 11, "Thousand Club" },        // 2,048 passengers
        { 12, "People Planet" },        // 4,096 passengers
        { 13, "Passenger Paradise" },   // 8,192 passengers
        { 14, "Aviation Empire" },      // 16,384 passengers
        { 15, "Skybound Metropolis" },  // 32,768 passengers
        { 16, "Global Gateway" },       // 65,536 passengers
        { 17, "Passenger Kingdom" },    // 131,072 passengers
        { 18, "Interstellar Terminal" },// 262,144 passengers
        { 19, "Galactic Transport" },   // 524,288 passengers
        { 20, "Universal Transit Hub" } // 1,048,576 passengers
    };
    
    // Achievement Buff Rewards - Easily modify rewards here
    
    // Weather resistance percentage per tier
    public static readonly double WeatherMasterBuffPercentage = 0.05; // 5% per tier
    
    // Night flight revenue bonus percentage per tier
    public static readonly double NightFlightBuffPercentage = 0.075; // 7.5% per tier
    
    // Consecutive flights XP bonus percentage per tier
    public static readonly double ConsecutiveFlightsXPBuffPercentage = 0.075; // 7.5% per tier
    
    // Simultaneous flights landing duration reduction percentage per tier
    public static readonly double SimultaneousFlightsBuffPercentage = 0.05; // 5% per tier
    
    // Emergency landing revenue bonus percentage per tier
    public static readonly double EmergencyLandingBuffPercentage = 0.15; // 15% per tier
    
    // Flight type specialization revenue bonus percentage per tier
    public static readonly double FlightTypeSpecializationBuffPercentage = 0.10; // 10% per tier
    
    // Perfect landing XP bonus percentage per tier
    public static readonly double PerfectLandingXPBuffPercentage = 0.10; // 10% per tier
    
    // Runway expert maintenance time reduction percentage per tier
    public static readonly double RunwayExpertBuffPercentage = 0.10; // 10% per tier
    
    // Passenger milestone gold income bonus percentage per tier
    public static readonly double PassengerMilestoneBuffPercentage = 0.05; // 5% per tier
    
    // Buff prices - base prices for each achievement type
    public static readonly double WeatherMasterBasePrice = 2000;
    public static readonly double NightFlightBasePrice = 3000;
    public static readonly double ConsecutiveFlightsBasePrice = 2500;
    public static readonly double SimultaneousFlightsBasePrice = 3500;
    public static readonly double EmergencyLandingBasePrice = 4000;
    public static readonly double FlightTypeSpecializationBasePrice = 1000;
    public static readonly double PerfectLandingBasePrice = 2000;
    public static readonly double RunwayExpertBasePrice = 1500;
    public static readonly double PassengerMilestoneBasePrice = 5000;
}