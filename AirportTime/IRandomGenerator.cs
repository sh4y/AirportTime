namespace AirportTime;

public interface IRandomGenerator
{
    int Next(int minValue, int maxValue);
    double NextDouble();
    void SetSeed(int seed);
}