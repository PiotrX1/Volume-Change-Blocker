using System.ComponentModel;
using System.Windows;
using AudioSwitcher.AudioApi.CoreAudio;
using MessageBox = System.Windows.MessageBox;

namespace VolumeChangeBlocker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private string AppName => "Volume change blocker";
    private readonly CoreAudioDevice _playbackDevice = new CoreAudioController().DefaultPlaybackDevice;
    private readonly NotifyIcon _notifyIcon = new();
    
    public MainWindow()
    {
        InitializeComponent();
        CreateTrayIcon();
        UpdateVolumeSlider();
        PrepareObserver();
        DataContext = new { AppName };
    }

    private void CreateTrayIcon()
    {
        _notifyIcon.Text = AppName;
        _notifyIcon.Visible = true;
        var icon = System.Drawing.Icon.ExtractAssociatedIcon(
            System.Reflection.Assembly.GetEntryAssembly()!.ManifestModule.Name);
        _notifyIcon.Icon = icon;
        _notifyIcon.DoubleClick += (_, _) => Show();
    }

    private void PrepareObserver()
    {
        var observer = new VolumeObserver();
        _playbackDevice.VolumeChanged.Subscribe(observer);
        observer.OnVolumeChanged += () =>
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
        };
    }

    private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ChangeVolume(e.NewValue);
    }

    private void ChangeVolume(double value)
    {
        _playbackDevice.Volume = value;
    }

    private void UpdateVolumeSlider()
    {
        VolumeSlider.Value = _playbackDevice.Volume;
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
}