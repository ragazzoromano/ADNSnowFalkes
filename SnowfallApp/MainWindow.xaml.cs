using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
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
    private DateTime _lastFrameTime = DateTime.Now;
    private bool _isAnimating;
    private bool _isFullscreen;
    private const double TargetFrameTime = 1.0 / 60.0; // Target 60 FPS

    public event EventHandler? AnimationStateChanged;
    public bool IsAnimating => _isAnimating;
    public bool HasFlakes => _flakes.Count > 0;

    public MainWindow(SnowfallSettings settings)
    {
        InitializeComponent();
        
        // Enable hardware acceleration for better GPU utilization
        // RenderMode.Default allows WPF to use hardware acceleration when available
        RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
        
        _settings = settings;
        _settings.PropertyChanged += (_, __) => RefreshFlakes();
        Loaded += OnLoaded;
        SizeChanged += (_, __) => RefreshFlakes();
        Closing += OnClosing;
    }

    private void NotifyAnimationStateChanged() => AnimationStateChanged?.Invoke(this, EventArgs.Empty);

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RefreshFlakes(initial: true);
        StartAnimation();
    }

    public void StartAnimation()
    {
        if (_isAnimating)
        {
            NotifyAnimationStateChanged();
            return;
        }

        if (_flakes.Count == 0)
        {
            RefreshFlakes(initial: true);
        }

        _lastUpdate = DateTime.Now;
        _lastFrameTime = DateTime.Now;
        CompositionTarget.Rendering += OnRendering;
        _isAnimating = true;
        NotifyAnimationStateChanged();
    }

    public void PauseAnimation()
    {
        if (!_isAnimating)
        {
            NotifyAnimationStateChanged();
            return;
        }

        CompositionTarget.Rendering -= OnRendering;
        _isAnimating = false;
        NotifyAnimationStateChanged();
    }

    public void StopAnimation()
    {
        if (_flakes.Count > 0)
        {
            SnowCanvas.Children.Clear();
            _flakes.Clear();
        }

        PauseAnimation();
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        
        // Frame rate throttling - skip frames if rendering too fast
        var timeSinceLastFrame = (now - _lastFrameTime).TotalSeconds;
        if (timeSinceLastFrame < TargetFrameTime)
        {
            return;
        }
        _lastFrameTime = now;
        
        var deltaSeconds = (now - _lastUpdate).TotalSeconds;
        _lastUpdate = now;

        if (deltaSeconds <= 0)
        {
            return;
        }

        var width = SnowCanvas.ActualWidth <= 0 ? ActualWidth : SnowCanvas.ActualWidth;
        var height = SnowCanvas.ActualHeight <= 0 ? ActualHeight : SnowCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        foreach (var flake in _flakes)
        {
            var drift = Math.Sin(flake.Phase + (flake.Y * 0.015)) * flake.Drift * deltaSeconds;
            flake.X += drift;
            flake.Y += flake.Speed * deltaSeconds;
            flake.Rotation += flake.RotationSpeed * deltaSeconds;

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

        AdjustCount(
            SnowflakeSize.Small,
            _settings.SmallCount,
            _settings.SmallBlurCount,
            _settings.SmallSizeScale,
            _settings.BlurIntensity,
            initial,
            width,
            height);
        AdjustCount(
            SnowflakeSize.Medium,
            _settings.MediumCount,
            _settings.MediumBlurCount,
            _settings.MediumSizeScale,
            _settings.BlurIntensity,
            initial,
            width,
            height);
        AdjustCount(
            SnowflakeSize.Large,
            _settings.LargeCount,
            _settings.LargeBlurCount,
            _settings.LargeSizeScale,
            _settings.BlurIntensity,
            initial,
            width,
            height);

        UpdateSpeeds();
    }

    private void UpdateSpeeds()
    {
        foreach (var flake in _flakes)
        {
            flake.Speed = BaseSpeedFor(flake.SizeCategory, _settings) * flake.SpeedFactor;
        }
    }

    private void AdjustCount(
        SnowflakeSize size,
        int targetCount,
        int targetBlurCount,
        double sizeScale,
        double blurIntensity,
        bool initial,
        double width,
        double height)
    {
        var sizeFlakes = _flakes.Where(f => f.SizeCategory == size).ToList();
        if (sizeFlakes.Count > targetCount)
        {
            var toRemove = sizeFlakes.Skip(targetCount).ToList();
            foreach (var flake in toRemove)
            {
                if (flake.Visual is not null)
                {
                    SnowCanvas.Children.Remove(flake.Visual);
                }

                _flakes.Remove(flake);
            }

            sizeFlakes = _flakes.Where(f => f.SizeCategory == size).ToList();
        }
        else if (sizeFlakes.Count < targetCount)
        {
            var addCount = targetCount - sizeFlakes.Count;
            var blurredCount = sizeFlakes.Count(f => f.IsBokeh);
            for (var i = 0; i < addCount; i++)
            {
                var shouldBlur = blurredCount < targetBlurCount;
                var flake = SnowflakeFactory.CreateSnowflake(
                    size,
                    initial,
                    width,
                    height,
                    BaseSpeedFor(size, _settings),
                    sizeScale,
                    shouldBlur,
                    blurIntensity,
                    _settings.RotationSpeed,
                    _settings.StrokeThickness);

                _flakes.Add(flake);
                sizeFlakes.Add(flake);
                if (flake.IsBokeh)
                {
                    blurredCount++;
                }

                flake.Visual = SnowflakeFactory.CreateVisual(flake, _settings.SnowflakeShape);
                SnowCanvas.Children.Add(flake.Visual);
                UpdateVisual(flake);
            }
        }

        BalanceBlurForSize(sizeFlakes, targetBlurCount, sizeScale, blurIntensity);
    }
    
    private void BalanceBlurForSize(
        List<Snowflake> flakes,
        int targetBlurCount,
        double sizeScale,
        double blurIntensity)
    {
        if (flakes.Count == 0)
        {
            return;
        }

        var blurred = flakes.Where(f => f.IsBokeh).ToList();
        var sharp = flakes.Where(f => !f.IsBokeh).ToList();

        if (blurred.Count > targetBlurCount)
        {
            foreach (var flake in blurred.Skip(targetBlurCount))
            {
                SetBlurState(flake, isBokeh: false, blurIntensity);
            }
        }
        else if (blurred.Count < targetBlurCount)
        {
            foreach (var flake in sharp.Take(targetBlurCount - blurred.Count))
            {
                SetBlurState(flake, isBokeh: true, blurIntensity);
            }
        }

        foreach (var flake in flakes)
        {
            ApplyAppearance(flake, sizeScale, blurIntensity);
        }
    }

    private void SetBlurState(Snowflake flake, bool isBokeh, double blurIntensity)
    {
        flake.IsBokeh = isBokeh;
        if (isBokeh)
        {
            flake.BaseBlurRadius = 2 + (SnowflakeFactory.Random.NextDouble() * 6);
            flake.Opacity = 0.25 + (SnowflakeFactory.Random.NextDouble() * 0.35);
        }
        else
        {
            flake.BaseBlurRadius = SnowflakeFactory.Random.NextDouble() * 0.5;
            flake.Opacity = 0.6 + (SnowflakeFactory.Random.NextDouble() * 0.35);
        }

        flake.BlurRadius = isBokeh ? flake.BaseBlurRadius * blurIntensity : flake.BaseBlurRadius;
    }

    private void ApplyAppearance(Snowflake flake, double sizeScale, double blurIntensity)
    {
        flake.Radius = flake.BaseRadius * sizeScale;
        flake.BlurRadius = flake.IsBokeh ? flake.BaseBlurRadius * blurIntensity : flake.BaseBlurRadius;

        // Update the cached ScaleTransform when radius changes
        var scale = flake.Radius / 10.0;
        flake.ScaleTransform.ScaleX = scale;
        flake.ScaleTransform.ScaleY = scale;

        SnowflakeFactory.ApplyVisualProperties(flake);
        UpdateVisual(flake);
    }

    private static double BaseSpeedFor(SnowflakeSize size, SnowfallSettings settings) => size switch
    {
        SnowflakeSize.Small => 55 * settings.SmallSpeed,
        SnowflakeSize.Medium => 70 * settings.MediumSpeed,
        _ => 85 * settings.LargeSpeed
    };

    private static void UpdateVisual(Snowflake flake)
    {
        if (flake.Visual is null)
        {
            return;
        }

        // Update the existing transform objects instead of creating new ones
        // Order in TransformGroup is: Scale, Rotate, Translate
        flake.RotateTransform.Angle = flake.Rotation;
        flake.TranslateTransform.X = flake.X;
        flake.TranslateTransform.Y = flake.Y;
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

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (!App.IsShuttingDown)
        {
            e.Cancel = true;
        }
    }
}
