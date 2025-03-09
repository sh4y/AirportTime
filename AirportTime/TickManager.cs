using System.Timers;

public class TickManager
{
    public int CurrentTick { get; private set; }
    private System.Timers.Timer timer;
    private int tickInterval; // milliseconds per tick
    private readonly int defaultInterval = 800; // 40 ticks per second
    private bool isRunning;
    private bool isPaused;

    public event Action<int> OnTick;

    public TickManager()
    {
        tickInterval = defaultInterval;
        CurrentTick = 0;
        timer = new System.Timers.Timer(tickInterval);
        timer.Elapsed += HandleTick;
    }

    public void Start()
    {
        if (!isRunning)
        {
            isRunning = true;
            timer.Interval = tickInterval;
            timer.Start();
            isPaused = false;
        }
    }

    public void Pause()
    {
        if (isRunning)
        {
            timer.Stop();
            isRunning = false;
            isPaused = true;
            
        }
    }

    public void SetSpeedMultiplier(double multiplier)
    {
        if (multiplier <= 0) throw new ArgumentException("Multiplier must be positive");
        tickInterval = (int)(defaultInterval / multiplier);
        timer.Interval = tickInterval;
    }

    private void HandleTick(object sender, ElapsedEventArgs e)
    {
        CurrentTick++;
        OnTick?.Invoke(CurrentTick);
        
    }
    
    public bool IsRunning() => isRunning;
    
    public bool IsPaused() => isPaused;
}
