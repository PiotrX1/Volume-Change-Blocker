using AudioSwitcher.AudioApi;

namespace VolumeChangeBlocker;

public class VolumeObserver : IObserver<DeviceVolumeChangedArgs>
{
    public delegate void OnVolumeChangedDelegate();

    public event OnVolumeChangedDelegate? OnVolumeChanged;
    
    
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DeviceVolumeChangedArgs value)
    {
        OnVolumeChanged?.Invoke();
    }
}