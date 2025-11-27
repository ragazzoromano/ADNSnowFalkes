using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using SnowfallApp.Models;

namespace SnowfallApp;

public partial class MainWindow : Window
{
    private static readonly SolidColorBrush SnowBrush = CreateSnowBrush();

    private readonly SnowfallSettings _settings;
    private readonly List<Snowflake> _flakes = new();
    private readonly Dictionary<Snowflake, Ellipse> _visualLookup = new();
    private readonly Random _random = new();
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

    private static SolidColorBrush CreateSnowBrush()
    {
        var brush = new SolidColorBrush(Color.FromRgb(240, 244, 255));
        brush.Freeze();
        return brush;
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

        foreach (var flake in _flakes)
        {
            var drift = Math.Sin(flake.Phase + (flake.Y * 0.015)) * flake.Drift * deltaSeconds;
            flake.X += drift;
            flake.Y += flake.Speed * deltaSeconds;

            WrapFlake(flake);
            UpdateVisual(flake);
        }
    }

    private void WrapFlake(Snowflake flake)
    {
        var width = SnowCanvas.ActualWidth <= 0 ? ActualWidth : SnowCanvas.ActualWidth;
        var height = SnowCanvas.ActualHeight <= 0 ? ActualHeight : SnowCanvas.ActualHeight;

        if (flake.Y - flake.Radius > height)
        {
            flake.Y = -flake.Radius - _random.NextDouble() * 10;
            flake.X = _random.NextDouble() * width;
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
        AdjustCount(SnowflakeSize.Small, _settings.SmallCount, initial);
        AdjustCount(SnowflakeSize.Medium, _settings.MediumCount, initial);
        AdjustCount(SnowflakeSize.Large, _settings.LargeCount, initial);

        UpdateSpeeds();
    }

    private void UpdateSpeeds()
    {
        foreach (var flake in _flakes)
        {
            flake.Speed = BaseSpeedFor(flake.SizeCategory) * flake.SpeedFactor;
        }
    }

    private void AdjustCount(SnowflakeSize size, int target, bool initial)
    {
        var existing = _flakes.Where(f => f.SizeCategory == size).ToList();
        if (existing.Count > target)
        {
            var toRemove = existing.Skip(target).ToList();
            foreach (var flake in toRemove)
            {
                if (_visualLookup.TryGetValue(flake, out var ellipse))
                {
                    SnowCanvas.Children.Remove(ellipse);
                    _visualLookup.Remove(flake);
                }

                _flakes.Remove(flake);
            }
        }
        else if (existing.Count < target)
        {
            var addCount = target - existing.Count;
            for (var i = 0; i < addCount; i++)
            {
                var flake = CreateSnowflake(size, initial);
                _flakes.Add(flake);
                var ellipse = CreateVisual(flake);
                _visualLookup[flake] = ellipse;
                SnowCanvas.Children.Add(ellipse);
            }
        }
    }

    private Snowflake CreateSnowflake(SnowflakeSize size, bool initial)
    {
        var baseSpeed = BaseSpeedFor(size);
        var (radiusMin, radiusMax) = size switch
        {
            SnowflakeSize.Small => (1.5, 2.6),
            SnowflakeSize.Medium => (2.7, 4.2),
            _ => (4.3, 6.2)
        };

        var speedFactor = 0.8 + (_random.NextDouble() * 0.4);
        var blurChance = size == SnowflakeSize.Large ? 0.5 : 0.35;
        var isBokeh = _random.NextDouble() < blurChance;
        var blurRadius = isBokeh ? 2 + (_random.NextDouble() * 6) : _random.NextDouble() * 0.5;
        var opacity = isBokeh ? 0.25 + (_random.NextDouble() * 0.35) : 0.6 + (_random.NextDouble() * 0.35);

        var width = SnowCanvas.ActualWidth <= 0 ? ActualWidth : SnowCanvas.ActualWidth;
        var height = SnowCanvas.ActualHeight <= 0 ? ActualHeight : SnowCanvas.ActualHeight;

        return new Snowflake
        {
            SizeCategory = size,
            Radius = radiusMin + (_random.NextDouble() * (radiusMax - radiusMin)),
            Drift = 15 + (_random.NextDouble() * 28),
            Phase = _random.NextDouble() * Math.PI * 2,
            SpeedFactor = speedFactor,
            Speed = baseSpeed * speedFactor,
            BlurRadius = blurRadius,
            Opacity = opacity,
            IsBokeh = isBokeh,
            X = _random.NextDouble() * Math.Max(1, width),
            Y = initial ? _random.NextDouble() * Math.Max(1, height) : -radiusMax,
            Brush = SnowBrush
        };
    }

    private double BaseSpeedFor(SnowflakeSize size) => size switch
    {
        SnowflakeSize.Small => 55 * _settings.SmallSpeed,
        SnowflakeSize.Medium => 70 * _settings.MediumSpeed,
        _ => 85 * _settings.LargeSpeed
    };

    private Ellipse CreateVisual(Snowflake flake)
    {
        var ellipse = new Ellipse
        {
            Width = flake.Radius * 2,
            Height = flake.Radius * 2,
            Fill = flake.Brush,
            Opacity = flake.Opacity,
            SnapsToDevicePixels = true
        };

        if (flake.BlurRadius > 0.2)
        {
            var blur = new BlurEffect { Radius = flake.BlurRadius, RenderingBias = RenderingBias.Quality };
            blur.Freeze();
            ellipse.CacheMode = new BitmapCache();
            ellipse.Effect = blur;
        }

        UpdateVisual(flake, ellipse);
        return ellipse;
    }

    private void UpdateVisual(Snowflake flake, Ellipse? ellipse = null)
    {
        ellipse ??= _visualLookup[flake];
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
