namespace VolumeChangeBlocker;

public class EventObserver<T> : IObserver<T>
{
    private readonly Action? _onNextAction;

    public EventObserver(Action? onNextAction)
    {
        _onNextAction = onNextAction;
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(T value)
    {
        _onNextAction?.Invoke();
    }
    
}