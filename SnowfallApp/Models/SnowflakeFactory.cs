using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace SnowfallApp.Models;

public static class SnowflakeFactory
{
    public static readonly Random Random = new();
    private static readonly SolidColorBrush SnowBrush = CreateSnowBrush();

    private static SolidColorBrush CreateSnowBrush()
    {
        var brush = new SolidColorBrush(Color.FromRgb(240, 244, 255));
        brush.Freeze();
        return brush;
    }

    public static Snowflake CreateSnowflake(
        SnowflakeSize size,
        bool initial,
        double width,
        double height,
        double baseSpeed)
    {
        var (radiusMin, radiusMax) = size switch
        {
            SnowflakeSize.Small => (1.5, 2.6),
            SnowflakeSize.Medium => (2.7, 4.2),
            _ => (4.3, 6.2)
        };

        var speedFactor = 0.8 + (Random.NextDouble() * 0.4);
        var blurChance = size == SnowflakeSize.Large ? 0.5 : 0.35;
        var isBokeh = Random.NextDouble() < blurChance;
        var blurRadius = isBokeh ? 2 + (Random.NextDouble() * 6) : Random.NextDouble() * 0.5;
        var opacity = isBokeh ? 0.25 + (Random.NextDouble() * 0.35) : 0.6 + (Random.NextDouble() * 0.35);

        return new Snowflake
        {
            SizeCategory = size,
            Radius = radiusMin + (Random.NextDouble() * (radiusMax - radiusMin)),
            Drift = 15 + (Random.NextDouble() * 28),
            Phase = Random.NextDouble() * Math.PI * 2,
            SpeedFactor = speedFactor,
            Speed = baseSpeed * speedFactor,
            BlurRadius = blurRadius,
            Opacity = opacity,
            IsBokeh = isBokeh,
            X = Random.NextDouble() * Math.Max(1, width),
            Y = initial ? Random.NextDouble() * Math.Max(1, height) : -radiusMax,
            Brush = SnowBrush
        };
    }

    public static Ellipse CreateVisual(Snowflake flake)
    {
        var ellipse = new Ellipse
        {
            Width = flake.Radius * 2,
            Height = flake.Radius * 2,
            Fill = flake.Brush,
            Opacity = flake.Opacity,
            SnapsToDevicePixels = true,
            RenderTransform = flake.Transform,
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

        if (flake.BlurRadius > 0.2)
        {
            var blur = new BlurEffect { Radius = flake.BlurRadius, RenderingBias = RenderingBias.Performance };
            blur.Freeze();
            ellipse.CacheMode = new BitmapCache();
            ellipse.Effect = blur;
        }
        else
        {
            ellipse.CacheMode = new BitmapCache();
        }

        return ellipse;
    }
}
