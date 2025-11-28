using System.Windows;
using SnowfallApp.Models;

namespace SnowfallApp;

public partial class App : Application
{
    public static bool IsShuttingDown { get; private set; }

    public static void ShutdownApplication()
    {
        IsShuttingDown = true;
        Current.Shutdown();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var settings = SettingsStorage.Load();
        settings.PropertyChanged += (_, __) => SettingsStorage.Save(settings);
        var mainWindow = new MainWindow(settings);
        MainWindow = mainWindow;
        mainWindow.Show();

        var controlWindow = new ControlWindow(settings)
        {
            Owner = mainWindow,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = mainWindow.Left + 40,
            Top = mainWindow.Top + 40
        };
        controlWindow.Show();
    }
}
