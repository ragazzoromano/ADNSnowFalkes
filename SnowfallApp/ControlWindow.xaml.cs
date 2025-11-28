using System;
using System.ComponentModel;
using System.Windows;
using SnowfallApp.Models;

namespace SnowfallApp;

public partial class ControlWindow : Window
{
    public ControlWindow(SnowfallSettings settings)
    {
        InitializeComponent();
        DataContext = settings;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.AnimationStateChanged += OnAnimationStateChanged;
            UpdateButtons(mainWindow);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.AnimationStateChanged -= OnAnimationStateChanged;
        }
    }

    private void OnAnimationStateChanged(object? sender, EventArgs e)
    {
        if (sender is MainWindow mainWindow)
        {
            Dispatcher.Invoke(() => UpdateButtons(mainWindow));
        }
    }

    private void UpdateButtons(MainWindow mainWindow)
    {
        var hasFlakes = mainWindow.HasFlakes;
        var isAnimating = mainWindow.IsAnimating;

        StartButton.IsEnabled = !hasFlakes;
        PauseResumeButton.Content = isAnimating ? "Pause" : "Resume";
        PauseResumeButton.IsEnabled = hasFlakes;
        StopButton.IsEnabled = hasFlakes;
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.StartAnimation();
        }
    }

    private void OnPauseResumeClick(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            if (mainWindow.IsAnimating)
            {
                mainWindow.PauseAnimation();
            }
            else
            {
                mainWindow.StartAnimation();
            }
        }
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.StopAnimation();
        }
    }

    private void OnQuitClick(object sender, RoutedEventArgs e)
    {
        App.ShutdownApplication();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (!App.IsShuttingDown)
        {
            e.Cancel = true;
        }
    }
}
