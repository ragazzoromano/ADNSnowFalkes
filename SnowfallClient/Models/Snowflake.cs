using System.Windows.Media;
using System.Windows.Shapes;

namespace SnowfallClient.Models;

public enum SnowflakeSize
{
    Small,
    Medium,
    Large
}

public class Snowflake
{
    public SnowflakeSize SizeCategory { get; init; }
    public double BaseRadius { get; init; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; set; }
    public double SpeedFactor { get; init; }
    public double Speed { get; set; }
    public double Drift { get; init; }
    public double Phase { get; init; }
    public double BaseBlurRadius { get; set; }
    public double BlurRadius { get; set; }
    public double Opacity { get; set; }
    public bool IsBokeh { get; set; }
    public Brush Brush { get; init; } = Brushes.White;
    public double RotationSpeed { get; init; }
    public double Rotation { get; set; }
    public double StrokeThickness { get; init; } = 1.0;
    public TransformGroup Transform { get; init; } = new();
    public RotateTransform RotateTransform { get; init; } = new();
    public TranslateTransform TranslateTransform { get; init; } = new();
    public ScaleTransform ScaleTransform { get; init; } = new();
    public Path? Visual { get; set; }
}
