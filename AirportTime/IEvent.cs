public interface IEvent
{
    string EventName { get; }
    void Trigger();
}
