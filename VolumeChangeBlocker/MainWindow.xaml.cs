using System.ComponentModel;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using MessageBox = System.Windows.MessageBox;

namespace VolumeChangeBlocker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static string AppName => "Volume change blocker";
    private readonly CoreAudioController _coreAudioController = new ();
    private CoreAudioDevice? _playbackDevice;
    private readonly NotifyIcon _notifyIcon = new();
    
    public MainWindow()
    {
        InitializeComponent();
        CreateTrayIcon();
        SetPlaybackDevice();
        UpdateVolumeSlider();
        SubscribeOnDeviceChange();
        DataContext = new { AppName };
    }

    private void CreateTrayIcon()
    {
        _notifyIcon.Text = AppName;
        _notifyIcon.Visible = true;
        var icon = System.Drawing.Icon.ExtractAssociatedIcon(
            System.Reflection.Assembly.GetEntryAssembly()!.ManifestModule.Name);
        _notifyIcon.Icon = icon;
        _notifyIcon.DoubleClick += (_, _) =>
        {
            Show();
            WindowState = WindowState.Normal;
        };
    }

    private void SubscribeOnDeviceChange()
    {
        var defaultObserver = new EventObserver<DeviceChangedArgs>(() =>
        {
            Dispatcher.Invoke(SetPlaybackDevice);
        });
        _coreAudioController.AudioDeviceChanged.Subscribe(defaultObserver);
    }

    private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ChangeVolume(e.NewValue);
    }

    private void ChangeVolume(double value)
    {
        if(_playbackDevice != null)
            _playbackDevice.Volume = value;
    }

    private void UpdateVolumeSlider()
    {
        VolumeSlider.Value = _playbackDevice?.Volume ?? 0;
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized) Hide();
        base.OnStateChanged(e);
    }
    
    protected override void OnClosing(CancelEventArgs e)
    {
        var result = MessageBox.Show(this, "Do you want to close app?", "Confirmation", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes) return;
        e.Cancel = true;
        base.OnClosing(e);
    }

    private void SetPlaybackDevice()
    {
        _playbackDevice = _coreAudioController.DefaultPlaybackDevice;
        var volumeObserver = new EventObserver<DeviceVolumeChangedArgs>(() =>
        {
            Dispatcher.Invoke(() =>
            {
                if (CheckBoxBlockVolume.IsChecked == true)
                {
                    if (Math.Abs(VolumeSlider.Value - _playbackDevice.Volume) > 0)
                        ChangeVolume(VolumeSlider.Value);
                }
                else
                {
                    UpdateVolumeSlider();
                }
            });
        });
        _playbackDevice.VolumeChanged.Subscribe(volumeObserver);
        
        UpdateVolumeSlider();

    }

    ~MainWindow()
    {
        _playbackDevice?.Dispose();
        _coreAudioController.Dispose();
    }
}