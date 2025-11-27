using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnowfallApp.Models;

public class SnowfallSettings : INotifyPropertyChanged
{
    private int _smallCount = 180;
    private int _mediumCount = 120;
    private int _largeCount = 60;

    private double _smallSpeed = 0.9;
    private double _mediumSpeed = 1.2;
    private double _largeSpeed = 1.6;

    public int SmallCount
    {
        get => _smallCount;
        set => SetField(ref _smallCount, value);
    }

    public int MediumCount
    {
        get => _mediumCount;
        set => SetField(ref _mediumCount, value);
    }

    public int LargeCount
    {
        get => _largeCount;
        set => SetField(ref _largeCount, value);
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
}
