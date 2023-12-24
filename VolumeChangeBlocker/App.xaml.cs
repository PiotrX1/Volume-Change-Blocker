using System.Windows;
using Application = System.Windows.Application;

namespace VolumeChangeBlocker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    void AppStartup(object sender, StartupEventArgs e)
    {
        var startMinimized = false;

        foreach (var arg in e.Args)
        {
            if (arg == "/minimized")
            {
                startMinimized = true;
            }
        }

        var mainWindow = new MainWindow();
        if (!startMinimized)
            mainWindow.Show();
       
    }
}