using System.Windows;
using SnowfallApp.Models;

namespace SnowfallApp;

public partial class ControlWindow : Window
{
    public ControlWindow(SnowfallSettings settings)
    {
        InitializeComponent();
        DataContext = settings;
    }
}
