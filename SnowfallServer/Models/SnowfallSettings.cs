using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnowfallServer.Models;

public class SnowfallSettings : INotifyPropertyChanged
{
    private int _smallCount = 180;
    private int _mediumCount = 120;
    private int _largeCount = 60;

    private double _smallSpeed = 0.9;
    private double _mediumSpeed = 1.2;
    private double _largeSpeed = 1.6;

    private double _smallSizeScale = 1.0;
    private double _mediumSizeScale = 1.0;
    private double _largeSizeScale = 1.0;

    private int _smallBlurCount = 60;
    private int _mediumBlurCount = 40;
    private int _largeBlurCount = 30;

    private double _blurIntensity = 1.0;

    private int _snowflakeShape = 0; // 0 = Random, 1-5 = specific shapes
    private double _rotationSpeed = 1.0; // Multiplier for rotation speed
    private double _strokeThickness = 1.0; // Line thickness for snowflakes

    public int SmallCount
    {
        get => _smallCount;
        set
        {
            if (SetField(ref _smallCount, value))
            {
                SmallBlurCount = ClampBlur(SmallBlurCount, _smallCount);
            }
        }
    }

    public int MediumCount
    {
        get => _mediumCount;
        set
        {
            if (SetField(ref _mediumCount, value))
            {
                MediumBlurCount = ClampBlur(MediumBlurCount, _mediumCount);
            }
        }
    }

    public int LargeCount
    {
        get => _largeCount;
        set
        {
            if (SetField(ref _largeCount, value))
            {
                LargeBlurCount = ClampBlur(LargeBlurCount, _largeCount);
            }
        }
    }

    public double SmallSpeed
    {
        get => _smallSpeed;
        set => SetField(ref _smallSpeed, value);
    }

    public double MediumSpeed
    {
        get => _mediumSpeed;
        set => SetField(ref _mediumSpeed, value);
    }

    public double LargeSpeed
    {
        get => _largeSpeed;
        set => SetField(ref _largeSpeed, value);
    }

    public double SmallSizeScale
    {
        get => _smallSizeScale;
        set => SetField(ref _smallSizeScale, value);
    }

    public double MediumSizeScale
    {
        get => _mediumSizeScale;
        set => SetField(ref _mediumSizeScale, value);
    }

    public double LargeSizeScale
    {
        get => _largeSizeScale;
        set => SetField(ref _largeSizeScale, value);
    }

    public int SmallBlurCount
    {
        get => _smallBlurCount;
        set => SetField(ref _smallBlurCount, ClampBlur(value, _smallCount));
    }

    public int MediumBlurCount
    {
        get => _mediumBlurCount;
        set => SetField(ref _mediumBlurCount, ClampBlur(value, _mediumCount));
    }

    public int LargeBlurCount
    {
        get => _largeBlurCount;
        set => SetField(ref _largeBlurCount, ClampBlur(value, _largeCount));
    }

    public double BlurIntensity
    {
        get => _blurIntensity;
        set => SetField(ref _blurIntensity, value);
    }

    public int SnowflakeShape
    {
        get => _snowflakeShape;
        set => SetField(ref _snowflakeShape, value);
    }

    public double RotationSpeed
    {
        get => _rotationSpeed;
        set => SetField(ref _rotationSpeed, value);
    }

    public double StrokeThickness
    {
        get => _strokeThickness;
        set => SetField(ref _strokeThickness, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private static int ClampBlur(int value, int maxCount) => Math.Max(0, Math.Min(value, maxCount));
}
