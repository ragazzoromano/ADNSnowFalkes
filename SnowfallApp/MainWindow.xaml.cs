using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SnowfallApp.Models;

namespace SnowfallApp;

public partial class MainWindow : Window
{
    private readonly SnowfallSettings _settings;
    private readonly List<Snowflake> _flakes = new();
    private DateTime _lastUpdate = DateTime.Now;
    private bool _isFullscreen;

    public MainWindow(SnowfallSettings settings)
    {
        InitializeComponent();
        _settings = settings;
        _settings.PropertyChanged += (_, __) => RefreshFlakes();
        Loaded += OnLoaded;
        SizeChanged += (_, __) => RefreshFlakes();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RefreshFlakes(initial: true);
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        var deltaSeconds = (now - _lastUpdate).TotalSeconds;
        _lastUpdate = now;

        var width = SnowCanvas.ActualWidth <= 0 ? ActualWidth : SnowCanvas.ActualWidth;
        var height = SnowCanvas.ActualHeight <= 0 ? ActualHeight : SnowCanvas.ActualHeight;

        foreach (var flake in _flakes)
        {
            var drift = Math.Sin(flake.Phase + (flake.Y * 0.015)) * flake.Drift * deltaSeconds;
            flake.X += drift;
            flake.Y += flake.Speed * deltaSeconds;

            WrapFlake(flake, width, height);
            UpdateVisual(flake);
        }
    }

    private void WrapFlake(Snowflake flake, double width, double height)
    {
        if (flake.Y - flake.Radius > height)
        {
            flake.Y = -flake.Radius - SnowflakeFactory.Random.NextDouble() * 10;
            flake.X = SnowflakeFactory.Random.NextDouble() * width;
        }

        if (flake.X < -flake.Radius)
        {
            flake.X = width + flake.Radius;
        }
        else if (flake.X - flake.Radius > width)
        {
            flake.X = -flake.Radius;
        }
    }

    private void RefreshFlakes(bool initial = false)
    {
        var width = SnowCanvas.ActualWidth <= 0 ? ActualWidth : SnowCanvas.ActualWidth;
        var height = SnowCanvas.ActualHeight <= 0 ? ActualHeight : SnowCanvas.ActualHeight;

        AdjustCount(SnowflakeSize.Small, _settings.SmallCount, initial, width, height);
        AdjustCount(SnowflakeSize.Medium, _settings.MediumCount, initial, width, height);
        AdjustCount(SnowflakeSize.Large, _settings.LargeCount, initial, width, height);

        UpdateSpeeds();
    }

    private void UpdateSpeeds()
    {
        foreach (var flake in _flakes)
        {
            flake.Speed = BaseSpeedFor(flake.SizeCategory, _settings) * flake.SpeedFactor;
        }
    }

    private void AdjustCount(SnowflakeSize size, int target, bool initial, double width, double height)
    {
        var existing = _flakes.Where(f => f.SizeCategory == size).ToList();
        if (existing.Count > target)
        {
            var toRemove = existing.Skip(target).ToList();
            foreach (var flake in toRemove)
            {
                if (flake.Visual is not null)
                {
                    SnowCanvas.Children.Remove(flake.Visual);
                }

                _flakes.Remove(flake);
            }
        }
        else if (existing.Count < target)
        {
            var addCount = target - existing.Count;
            for (var i = 0; i < addCount; i++)
            {
                var flake = SnowflakeFactory.CreateSnowflake(size, initial, width, height, BaseSpeedFor(size, _settings));
                _flakes.Add(flake);
                flake.Visual = SnowflakeFactory.CreateVisual(flake);
                SnowCanvas.Children.Add(flake.Visual);
            }
        }
    }
    
    private static double BaseSpeedFor(SnowflakeSize size, SnowfallSettings settings) => size switch
    {
        SnowflakeSize.Small => 55 * settings.SmallSpeed,
        SnowflakeSize.Medium => 70 * settings.MediumSpeed,
        _ => 85 * settings.LargeSpeed
    };

    private static void UpdateVisual(Snowflake flake)
    {
        if (flake.Visual is not { } ellipse)
        {
            return;
        }

        Canvas.SetLeft(ellipse, flake.X - flake.Radius);
        Canvas.SetTop(ellipse, flake.Y - flake.Radius);
    }

    private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _isFullscreen = !_isFullscreen;
        if (_isFullscreen)
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            Topmost = true;
        }
        else
        {
            Topmost = false;
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
        }
    }
}
