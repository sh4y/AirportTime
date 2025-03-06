public class Weather
{
    public WeatherType CurrentWeather { get; private set; }
    private IRandomGenerator randomGenerator;

    public Weather(IRandomGenerator rng)
    {
        randomGenerator = rng;
        GenerateRandomWeather();
    }

    // Randomly determines weather, can be called per tick or periodically
    public void GenerateRandomWeather()
    {
        int weatherValue = randomGenerator.Next(0, 100);

        if (weatherValue < 50)
            CurrentWeather = WeatherType.Clear;
        else if (weatherValue < 70)
            CurrentWeather = WeatherType.Rainy;
        else if (weatherValue < 85)
            CurrentWeather = WeatherType.Foggy;
        else if (weatherValue < 95)
            CurrentWeather = WeatherType.Snowy;
        else
            CurrentWeather = WeatherType.Stormy;
    }

    // Determines runway wear impact multiplier
    public int GetWeatherImpact()
    {
        switch (CurrentWeather)
        {
            case WeatherType.Clear:
                return 0;    // No additional wear
            case WeatherType.Rainy:
                return 1;    // Slightly increased wear
            case WeatherType.Foggy:
                return 2;    // Moderate wear
            case WeatherType.Snowy:
                return 3;    // High wear
            case WeatherType.Stormy:
                return 5;    // Extreme wear
            default:
                return 0;
        }
    }

    // Optional: Weather's effect on landing success probability
    public double GetLandingSuccessModifier()
    {
        switch (CurrentWeather)
        {
            case WeatherType.Clear:
                return 1.0;
            case WeatherType.Rainy:
                return 0.95;
            case WeatherType.Foggy:
                return 0.9;
            case WeatherType.Snowy:
                return 0.8;
            case WeatherType.Stormy:
                return 0.6;
            default:
                return 1.0;
        }
    }

    public override string ToString()
    {
        return CurrentWeather.ToString();
    }
}
