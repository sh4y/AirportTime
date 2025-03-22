// AchievementConfig.cs - Configuration parameters for easy editing
public static class AchievementConfig
{
    // Flight Type Achievement thresholds - extended progression
    public static readonly int[] FlightTypeThresholds = { 20, 60, 200, 750, 2500, 7500, 15000 };

    // Perfect Landing Achievement thresholds - extended progression
    public static readonly int[] PerfectLandingThresholds = { 10, 35, 100, 300, 800, 2000, 4000 };

    // Runway Expert Achievement thresholds - extended progression
    public static readonly int[] RunwayExpertThresholds = { 15, 45, 100, 250, 500, 1000, 2000 };

    // Passenger Milestone Achievement configuration - increased range
    public static readonly int PassengerMilestoneBaseValue = 2;
    public static readonly int PassengerMilestoneMinExponent = 13;  // 2^7 = 128
    public static readonly int PassengerMilestoneMaxExponent = 24; // 2^24 = 16,777,216

    // Weather Master Achievement thresholds - extended progression
    public static readonly Dictionary<WeatherType, int[]> WeatherMasterThresholds = new Dictionary<WeatherType, int[]>
    {
        { WeatherType.Rainy, new[] { 15, 45, 150, 450, 1000 } },
        { WeatherType.Foggy, new[] { 10, 30, 100, 300, 750 } },
        { WeatherType.Snowy, new[] { 10, 30, 100, 300, 750 } },
        { WeatherType.Stormy, new[] { 8, 25, 75, 200, 500 } }
    };

    // Night Flight Achievement thresholds - extended progression
    public static readonly int[] NightFlightThresholds = { 25, 75, 175, 350, 700, 1500 };

    // Consecutive Flight Achievement thresholds - extended progression
    public static readonly int[] ConsecutiveFlightThresholds = { 10, 30, 60, 120, 250, 500 };

    // Simultaneous Flight Achievement thresholds - extended progression
    public static readonly int[] SimultaneousFlightThresholds = { 5, 8, 12, 18, 25, 35 };

    // Emergency Landing Achievement thresholds - extended progression
    public static readonly int[] EmergencyLandingThresholds = { 5, 20, 50, 120, 250, 500 };

    // Milestone titles for passenger achievements - extended for new exponents
    public static readonly Dictionary<int, string> PassengerMilestoneTitles = new Dictionary<int, string>()
    {
        { 7, "First Steps" },            // 128 passengers
        { 8, "Taking Off" },             // 256 passengers
        { 9, "Regional Hub" },           // 512 passengers
        { 10, "People Mover" },          // 1,024 passengers
        { 11, "Terminal Bustle" },       // 2,048 passengers
        { 12, "Thousand Club" },         // 4,096 passengers
        { 13, "People Planet" },         // 8,192 passengers
        { 14, "Passenger Paradise" },    // 16,384 passengers
        { 15, "Aviation Empire" },       // 32,768 passengers
        { 16, "Skybound Metropolis" },   // 65,536 passengers
        { 17, "Global Gateway" },        // 131,072 passengers
        { 18, "Passenger Kingdom" },     // 262,144 passengers
        { 19, "Interstellar Terminal" }, // 524,288 passengers
        { 20, "Galactic Transport" },    // 1,048,576 passengers
        { 21, "Universal Transit Hub" }, // 2,097,152 passengers
        { 22, "Cosmic Travel Network" }, // 4,194,304 passengers
        { 23, "Dimensional Portal" },    // 8,388,608 passengers
        { 24, "Transcendent Station" }   // 16,777,216 passengers
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