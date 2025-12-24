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
    private static readonly List<PathGeometry> SnowflakeGeometries = CreateSnowflakeGeometries();

    private static SolidColorBrush CreateSnowBrush()
    {
        var brush = new SolidColorBrush(Color.FromRgb(240, 244, 255));
        brush.Freeze();
        return brush;
    }

    private static List<PathGeometry> CreateSnowflakeGeometries()
    {
        var geometries = new List<PathGeometry>
        {
            CreateSnowflakeGeometry1(),
            CreateSnowflakeGeometry2(),
            CreateSnowflakeGeometry3(),
            CreateSnowflakeGeometry4(),
            CreateSnowflakeGeometry5(),
            CreateSnowflakeGeometry6()
        };

        foreach (var geometry in geometries)
        {
            geometry.Freeze();
        }

        return geometries;
    }

    // Simple snowflake with 6 main branches
    private static PathGeometry CreateSnowflakeGeometry1()
    {
        var geometry = new PathGeometry();
        var radius = 10.0;

        for (int i = 0; i < 6; i++)
        {
            var angle = i * Math.PI / 3.0;
            var figure = new PathFigure
            {
                StartPoint = new Point(0, 0),
                IsClosed = false
            };

            var endX = Math.Cos(angle) * radius;
            var endY = Math.Sin(angle) * radius;
            figure.Segments.Add(new LineSegment(new Point(endX, endY), true));

            geometry.Figures.Add(figure);
        }

        return geometry;
    }

    // Snowflake with main branches and small side branches
    private static PathGeometry CreateSnowflakeGeometry2()
    {
        var geometry = new PathGeometry();
        var radius = 10.0;

        for (int i = 0; i < 6; i++)
        {
            var angle = i * Math.PI / 3.0;
            var figure = new PathFigure
            {
                StartPoint = new Point(0, 0),
                IsClosed = false
            };

            var endX = Math.Cos(angle) * radius;
            var endY = Math.Sin(angle) * radius;
            figure.Segments.Add(new LineSegment(new Point(endX, endY), true));

            geometry.Figures.Add(figure);

            // Add small side branches
            var midX = endX * 0.6;
            var midY = endY * 0.6;
            var perpAngle1 = angle + Math.PI / 6;
            var perpAngle2 = angle - Math.PI / 6;

            var branch1 = new PathFigure
            {
                StartPoint = new Point(midX, midY),
                IsClosed = false
            };
            branch1.Segments.Add(new LineSegment(
                new Point(midX + Math.Cos(perpAngle1) * 3, midY + Math.Sin(perpAngle1) * 3), true));
            geometry.Figures.Add(branch1);

            var branch2 = new PathFigure
            {
                StartPoint = new Point(midX, midY),
                IsClosed = false
            };
            branch2.Segments.Add(new LineSegment(
                new Point(midX + Math.Cos(perpAngle2) * 3, midY + Math.Sin(perpAngle2) * 3), true));
            geometry.Figures.Add(branch2);
        }

        return geometry;
    }

    // Complex snowflake with multiple side branches
    private static PathGeometry CreateSnowflakeGeometry3()
    {
        var geometry = new PathGeometry();
        var radius = 10.0;

        for (int i = 0; i < 6; i++)
        {
            var angle = i * Math.PI / 3.0;
            var figure = new PathFigure
            {
                StartPoint = new Point(0, 0),
                IsClosed = false
            };

            var endX = Math.Cos(angle) * radius;
            var endY = Math.Sin(angle) * radius;
            figure.Segments.Add(new LineSegment(new Point(endX, endY), true));

            geometry.Figures.Add(figure);

            // Add multiple side branches at different positions
            for (double t = 0.4; t <= 0.8; t += 0.2)
            {
                var branchX = endX * t;
                var branchY = endY * t;
                var branchLen = 2.5;

                var perpAngle1 = angle + Math.PI / 4;
                var perpAngle2 = angle - Math.PI / 4;

                var branch1 = new PathFigure
                {
                    StartPoint = new Point(branchX, branchY),
                    IsClosed = false
                };
                branch1.Segments.Add(new LineSegment(
                    new Point(branchX + Math.Cos(perpAngle1) * branchLen, 
                              branchY + Math.Sin(perpAngle1) * branchLen), true));
                geometry.Figures.Add(branch1);

                var branch2 = new PathFigure
                {
                    StartPoint = new Point(branchX, branchY),
                    IsClosed = false
                };
                branch2.Segments.Add(new LineSegment(
                    new Point(branchX + Math.Cos(perpAngle2) * branchLen, 
                              branchY + Math.Sin(perpAngle2) * branchLen), true));
                geometry.Figures.Add(branch2);
            }
        }

        return geometry;
    }

    // Delicate snowflake with fine branches
    private static PathGeometry CreateSnowflakeGeometry4()
    {
        var geometry = new PathGeometry();
        var radius = 10.0;

        for (int i = 0; i < 6; i++)
        {
            var angle = i * Math.PI / 3.0;
            var figure = new PathFigure
            {
                StartPoint = new Point(0, 0),
                IsClosed = false
            };

            var endX = Math.Cos(angle) * radius;
            var endY = Math.Sin(angle) * radius;
            figure.Segments.Add(new LineSegment(new Point(endX, endY), true));

            geometry.Figures.Add(figure);

            // Add fine branches near the tip
            var tipX = endX * 0.75;
            var tipY = endY * 0.75;

            for (int j = -1; j <= 1; j += 2)
            {
                var branchAngle = angle + (j * Math.PI / 8);
                var branch = new PathFigure
                {
                    StartPoint = new Point(tipX, tipY),
                    IsClosed = false
                };
                branch.Segments.Add(new LineSegment(
                    new Point(tipX + Math.Cos(branchAngle) * 2.5, 
                              tipY + Math.Sin(branchAngle) * 2.5), true));
                geometry.Figures.Add(branch);
            }

            // Add smaller branches in the middle
            var midX = endX * 0.5;
            var midY = endY * 0.5;

            for (int j = -1; j <= 1; j += 2)
            {
                var branchAngle = angle + (j * Math.PI / 6);
                var branch = new PathFigure
                {
                    StartPoint = new Point(midX, midY),
                    IsClosed = false
                };
                branch.Segments.Add(new LineSegment(
                    new Point(midX + Math.Cos(branchAngle) * 2, 
                              midY + Math.Sin(branchAngle) * 2), true));
                geometry.Figures.Add(branch);
            }
        }

        return geometry;
    }

    // Star-like snowflake with angular branches
    private static PathGeometry CreateSnowflakeGeometry5()
    {
        var geometry = new PathGeometry();
        var radius = 10.0;

        for (int i = 0; i < 6; i++)
        {
            var angle = i * Math.PI / 3.0;
            var figure = new PathFigure
            {
                StartPoint = new Point(0, 0),
                IsClosed = false
            };

            var endX = Math.Cos(angle) * radius;
            var endY = Math.Sin(angle) * radius;
            figure.Segments.Add(new LineSegment(new Point(endX, endY), true));

            geometry.Figures.Add(figure);

            // Add V-shaped branches near the tip
            var tipX = endX * 0.8;
            var tipY = endY * 0.8;

            var leftAngle = angle - Math.PI / 5;
            var rightAngle = angle + Math.PI / 5;

            var leftBranch = new PathFigure
            {
                StartPoint = new Point(tipX, tipY),
                IsClosed = false
            };
            leftBranch.Segments.Add(new LineSegment(
                new Point(tipX + Math.Cos(leftAngle) * 3, 
                          tipY + Math.Sin(leftAngle) * 3), true));
            geometry.Figures.Add(leftBranch);

            var rightBranch = new PathFigure
            {
                StartPoint = new Point(tipX, tipY),
                IsClosed = false
            };
            rightBranch.Segments.Add(new LineSegment(
                new Point(tipX + Math.Cos(rightAngle) * 3, 
                          tipY + Math.Sin(rightAngle) * 3), true));
            geometry.Figures.Add(rightBranch);
        }

        return geometry;
    }

    // Circle snowflake (simple circle shape)
    private static PathGeometry CreateSnowflakeGeometry6()
    {
        var geometry = new PathGeometry();
        var radius = 10.0;

        var figure = new PathFigure
        {
            StartPoint = new Point(radius, 0),
            IsClosed = true
        };

        // Create a circle using two arc segments
        figure.Segments.Add(new ArcSegment(
            new Point(-radius, 0), 
            new Size(radius, radius), 
            0, 
            false, 
            SweepDirection.Clockwise, 
            true));
        
        figure.Segments.Add(new ArcSegment(
            new Point(radius, 0), 
            new Size(radius, radius), 
            0, 
            false, 
            SweepDirection.Clockwise, 
            true));

        geometry.Figures.Add(figure);

        return geometry;
    }

    public static Snowflake CreateSnowflake(
        SnowflakeSize size,
        bool initial,
        double width,
        double height,
        double baseSpeed,
        double sizeScale,
        bool isBokeh,
        double blurIntensity,
        double rotationSpeedMultiplier,
        double strokeThickness)
    {
        var (radiusMin, radiusMax) = size switch
        {
            SnowflakeSize.Small => (1.5, 2.6),
            SnowflakeSize.Medium => (2.7, 4.2),
            _ => (4.3, 6.2)
        };

        var speedFactor = 0.8 + (Random.NextDouble() * 0.4);
        var baseRadius = radiusMin + (Random.NextDouble() * (radiusMax - radiusMin));
        var baseBlurRadius = isBokeh ? 2 + (Random.NextDouble() * 6) : Random.NextDouble() * 0.5;
        var opacity = isBokeh ? 0.25 + (Random.NextDouble() * 0.35) : 0.6 + (Random.NextDouble() * 0.35);
        
        const double MaxRotationSpeedRange = 60.0;
        const double RotationSpeedOffset = 30.0;
        var rotationSpeed = ((Random.NextDouble() * MaxRotationSpeedRange) - RotationSpeedOffset) * rotationSpeedMultiplier;

        var radius = baseRadius * sizeScale;
        var scale = radius / 10.0; // Base geometry is designed with radius 10
        
        var scaleTransform = new ScaleTransform(scale, scale);
        var rotateTransform = new RotateTransform(Random.NextDouble() * 360);
        var translateTransform = new TranslateTransform();
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(scaleTransform);
        transformGroup.Children.Add(rotateTransform);
        transformGroup.Children.Add(translateTransform);

        return new Snowflake
        {
            SizeCategory = size,
            BaseRadius = baseRadius,
            Radius = radius,
            Drift = 15 + (Random.NextDouble() * 28),
            Phase = Random.NextDouble() * Math.PI * 2,
            SpeedFactor = speedFactor,
            Speed = baseSpeed * speedFactor,
            BaseBlurRadius = baseBlurRadius,
            BlurRadius = isBokeh ? baseBlurRadius * blurIntensity : baseBlurRadius,
            Opacity = opacity,
            IsBokeh = isBokeh,
            X = Random.NextDouble() * Math.Max(1, width),
            Y = initial ? Random.NextDouble() * Math.Max(1, height) : -radiusMax,
            Brush = SnowBrush,
            RotationSpeed = rotationSpeed,
            Rotation = Random.NextDouble() * 360, // Random initial rotation
            StrokeThickness = strokeThickness,
            Transform = transformGroup,
            RotateTransform = rotateTransform,
            TranslateTransform = translateTransform,
            ScaleTransform = scaleTransform
        };
    }

    public static Path CreateVisual(Snowflake flake, int shapeIndex)
    {
        // Select snowflake geometry based on shape index
        // 0 = Random, 1-6 = specific shapes
        int geometryIndex;
        if (shapeIndex == 0)
        {
            geometryIndex = Random.Next(SnowflakeGeometries.Count);
        }
        else
        {
            geometryIndex = Math.Max(0, Math.Min(shapeIndex - 1, SnowflakeGeometries.Count - 1));
        }

        var selectedGeometry = SnowflakeGeometries[geometryIndex];

        var path = new Path
        {
            Data = selectedGeometry,
            SnapsToDevicePixels = true,
            RenderTransform = flake.Transform,
            Stroke = flake.Brush,
            StrokeThickness = flake.StrokeThickness
        };

        ApplyVisualProperties(flake, path);

        return path;
    }

    public static void ApplyVisualProperties(Snowflake flake)
    {
        if (flake.Visual is not { } path)
        {
            return;
        }

        ApplyVisualProperties(flake, path);
    }

    private static void ApplyVisualProperties(Snowflake flake, Path path)
    {
        // All transforms are handled through RenderTransform (TransformGroup)
        path.Stroke = flake.Brush;
        path.StrokeThickness = flake.StrokeThickness;
        path.Opacity = flake.Opacity;

        if (flake.IsBokeh && flake.BlurRadius > 0.2)
        {
            var blur = new BlurEffect { Radius = flake.BlurRadius, RenderingBias = RenderingBias.Performance };
            blur.Freeze();
            path.CacheMode = new BitmapCache();
            path.Effect = blur;
        }
        else
        {
            path.CacheMode = new BitmapCache();
            path.Effect = null;
        }
    }
}
