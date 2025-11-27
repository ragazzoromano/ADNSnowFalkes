using System.Windows.Media;
using System.Windows.Shapes;

namespace SnowfallApp.Models;

public enum SnowflakeSize
{
    Small,
    Medium,
    Large
}

public class Snowflake
{
    public SnowflakeSize SizeCategory { get; init; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; init; }
    public double SpeedFactor { get; init; }
    public double Speed { get; set; }
    public double Drift { get; init; }
    public double Phase { get; init; }
    public double BlurRadius { get; init; }
    public double Opacity { get; init; }
    public bool IsBokeh { get; init; }
    public Brush Brush { get; init; } = Brushes.White;
    public Ellipse? Visual { get; set; }
}
