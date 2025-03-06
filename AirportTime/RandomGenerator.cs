public class RandomGenerator : IRandomGenerator
{
    private Random random;

    public RandomGenerator(int seed = -1)
    {
        random = (seed >= 0) ? new Random(seed) : new Random();
    }

    public int Next(int minValue, int maxValue)
    {
        return random.Next(minValue, maxValue);
    }

    public double NextDouble()
    {
        return random.NextDouble();
    }

    public void SetSeed(int seed)
    {
        random = new Random(seed);
    }
}
