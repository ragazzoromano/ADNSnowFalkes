using System.Windows;
using SnowfallApp.Models;

namespace SnowfallApp;

public partial class ControlWindow : Window
{
    public ControlWindow(SnowfallSettings settings)
    {
        InitializeComponent();
        DataContext = settings;
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.StartAnimation();
        }
    }

    private void OnPauseClick(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.PauseAnimation();
        }
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.StopAnimation();
        }
    }
}
