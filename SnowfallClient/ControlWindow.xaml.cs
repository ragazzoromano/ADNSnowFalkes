using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SnowfallClient.Models;
using SnowfallShared.Protocol;
using SnowfallShared.Models;

namespace SnowfallClient;

public partial class ControlWindow : Window
{
    private TcpClient? _client;
    private StreamWriter? _writer;
    private SnowfallSettings _settings;
    private bool _isConnected;

    public ControlWindow()
    {
        InitializeComponent();
        _settings = new SnowfallSettings();
        DataContext = _settings;
        _settings.PropertyChanged += (_, __) => SendSettingsUpdate();
        Closing += (_, __) => Disconnect();
    }

    private async void OnConnect(object sender, RoutedEventArgs e)
    {
        if (_isConnected) return;

        var serverIp = ServerIpTextBox.Text;
        if (!int.TryParse(PortTextBox.Text, out int port) || port < 1 || port > 65535)
        {
            MessageBox.Show("Please enter a valid port number (1-65535).", "Invalid Port", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(serverIp, port);
            _writer = new StreamWriter(_client.GetStream(), Encoding.UTF8) { AutoFlush = true };
            
            _isConnected = true;
            ConnectButton.IsEnabled = false;
            DisconnectButton.IsEnabled = true;
            ServerIpTextBox.IsEnabled = false;
            PortTextBox.IsEnabled = false;
            ConnectionStatus.Text = "Connected";
            ConnectionStatus.Foreground = System.Windows.Media.Brushes.Green;

            // Send initial settings
            SendSettingsUpdate();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to connect: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Disconnect();
        }
    }

    private void OnDisconnect(object sender, RoutedEventArgs e)
    {
        Disconnect();
    }

    private void Disconnect()
    {
        if (!_isConnected) return;

        _isConnected = false;
        _writer?.Dispose();
        _client?.Dispose();
        _writer = null;
        _client = null;

        Dispatcher.Invoke(() =>
        {
            ConnectButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;
            ServerIpTextBox.IsEnabled = true;
            PortTextBox.IsEnabled = true;
            ConnectionStatus.Text = "Disconnected";
            ConnectionStatus.Foreground = System.Windows.Media.Brushes.Red;
        });
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        SendCommand(CommandType.StartAnimation);
    }

    private void OnPauseResumeClick(object sender, RoutedEventArgs e)
    {
        SendCommand(CommandType.PauseAnimation);
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        SendCommand(CommandType.StopAnimation);
    }

    private void SendCommand(CommandType commandType, string? data = null)
    {
        if (!_isConnected || _writer == null)
        {
            MessageBox.Show("Not connected to server.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var command = new Command
            {
                Type = commandType,
                Data = data
            };

            _writer.WriteLine(command.ToJson());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to send command: {ex.Message}", "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Disconnect();
        }
    }

    private void SendSettingsUpdate()
    {
        if (!_isConnected) return;

        var settingsDto = new SettingsDto
        {
            SmallCount = _settings.SmallCount,
            MediumCount = _settings.MediumCount,
            LargeCount = _settings.LargeCount,
            SmallSpeed = _settings.SmallSpeed,
            MediumSpeed = _settings.MediumSpeed,
            LargeSpeed = _settings.LargeSpeed,
            SmallSizeScale = _settings.SmallSizeScale,
            MediumSizeScale = _settings.MediumSizeScale,
            LargeSizeScale = _settings.LargeSizeScale,
            SmallBlurCount = _settings.SmallBlurCount,
            MediumBlurCount = _settings.MediumBlurCount,
            LargeBlurCount = _settings.LargeBlurCount,
            BlurIntensity = _settings.BlurIntensity,
            SnowflakeShape = _settings.SnowflakeShape,
            RotationSpeed = _settings.RotationSpeed,
            StrokeThickness = _settings.StrokeThickness
        };

        SendCommand(CommandType.UpdateSettings, settingsDto.ToJson());
    }
}
