using System.Text.Json;

namespace SnowfallShared.Models;

public class SettingsDto
{
    public int SmallCount { get; set; }
    public int MediumCount { get; set; }
    public int LargeCount { get; set; }
    public double SmallSpeed { get; set; }
    public double MediumSpeed { get; set; }
    public double LargeSpeed { get; set; }
    public double SmallSizeScale { get; set; }
    public double MediumSizeScale { get; set; }
    public double LargeSizeScale { get; set; }
    public int SmallBlurCount { get; set; }
    public int MediumBlurCount { get; set; }
    public int LargeBlurCount { get; set; }
    public double BlurIntensity { get; set; }
    public int SnowflakeShape { get; set; }
    public double RotationSpeed { get; set; }
    public double StrokeThickness { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static SettingsDto? FromJson(string json)
    {
        return JsonSerializer.Deserialize<SettingsDto>(json);
    }
}
