using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SnowfallServer.Models;
using SnowfallShared.Protocol;
using SnowfallShared.Models;

namespace SnowfallServer;

public partial class ServerWindow : Window
{
    private TcpListener? _listener;
    private CancellationTokenSource? _cancellationTokenSource;
    private MainWindow? _mainWindow;
    private SnowfallSettings _settings;
    private bool _isServerRunning;

    public ServerWindow()
    {
        InitializeComponent();
        _settings = SettingsStorage.Load();
        Closing += (_, __) => Shutdown();
    }

    private void OnShowDisplay(object sender, RoutedEventArgs e)
    {
        if (_mainWindow == null || !_mainWindow.IsVisible)
        {
            _mainWindow = new MainWindow(_settings);
            _mainWindow.Show();
        }
        else
        {
            _mainWindow.Activate();
        }
    }

    private async void OnStartServer(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PortTextBox.Text, out int port) || port < 1 || port > 65535)
        {
            MessageBox.Show("Please enter a valid port number (1-65535).", "Invalid Port", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _cancellationTokenSource = new CancellationTokenSource();
            _isServerRunning = true;

            StartServerButton.IsEnabled = false;
            StopServerButton.IsEnabled = true;
            PortTextBox.IsEnabled = false;
            StatusTextBlock.Text = $"Server listening on port {port}";
            
            AddLog($"Server started on port {port}");

            // Start accepting clients
            _ = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            AddLog($"Error starting server: {ex.Message}");
        }
    }

    private void OnStopServer(object sender, RoutedEventArgs e)
    {
        StopServerInternal();
    }

    private void StopServerInternal()
    {
        if (!_isServerRunning) return;

        _isServerRunning = false;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _listener?.Stop();

        Dispatcher.Invoke(() =>
        {
            StartServerButton.IsEnabled = true;
            StopServerButton.IsEnabled = false;
            PortTextBox.IsEnabled = true;
            StatusTextBlock.Text = "Server stopped";
            AddLog("Server stopped");
        });
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _listener != null)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                AddLog($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var clientEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        AddLog($"Client connected: {clientEndPoint}");

        try
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (!cancellationToken.IsCancellationRequested && client.Connected)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;

                    ProcessCommand(line, clientEndPoint);
                }
            }
        }
        catch (Exception ex)
        {
            AddLog($"Error handling client {clientEndPoint}: {ex.Message}");
        }
        finally
        {
            AddLog($"Client disconnected: {clientEndPoint}");
        }
    }

    private void ProcessCommand(string json, string clientEndPoint)
    {
        try
        {
            var command = Command.FromJson(json);
            if (command == null)
            {
                AddLog($"Invalid command from {clientEndPoint}");
                return;
            }

            AddLog($"Command received from {clientEndPoint}: {command.Type}");

            Dispatcher.Invoke(() =>
            {
                switch (command.Type)
                {
                    case CommandType.StartAnimation:
                        _mainWindow?.StartAnimation();
                        break;

                    case CommandType.PauseAnimation:
                        _mainWindow?.PauseAnimation();
                        break;

                    case CommandType.StopAnimation:
                        _mainWindow?.StopAnimation();
                        break;

                    case CommandType.UpdateSettings:
                        if (command.Data != null)
                        {
                            var settingsDto = SettingsDto.FromJson(command.Data);
                            if (settingsDto != null)
                            {
                                ApplySettings(settingsDto);
                                AddLog($"Settings updated from {clientEndPoint}");
                            }
                        }
                        break;

                    case CommandType.Shutdown:
                        AddLog($"Shutdown requested from {clientEndPoint}");
                        Shutdown();
                        break;
                }
            });
        }
        catch (Exception ex)
        {
            AddLog($"Error processing command: {ex.Message}");
        }
    }

    private void ApplySettings(SettingsDto dto)
    {
        _settings.SmallCount = dto.SmallCount;
        _settings.MediumCount = dto.MediumCount;
        _settings.LargeCount = dto.LargeCount;
        _settings.SmallSpeed = dto.SmallSpeed;
        _settings.MediumSpeed = dto.MediumSpeed;
        _settings.LargeSpeed = dto.LargeSpeed;
        _settings.SmallSizeScale = dto.SmallSizeScale;
        _settings.MediumSizeScale = dto.MediumSizeScale;
        _settings.LargeSizeScale = dto.LargeSizeScale;
        _settings.SmallBlurCount = dto.SmallBlurCount;
        _settings.MediumBlurCount = dto.MediumBlurCount;
        _settings.LargeBlurCount = dto.LargeBlurCount;
        _settings.BlurIntensity = dto.BlurIntensity;
        _settings.SnowflakeShape = dto.SnowflakeShape;
        _settings.RotationSpeed = dto.RotationSpeed;
        _settings.StrokeThickness = dto.StrokeThickness;
    }

    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] {message}\n";
        
        Dispatcher.Invoke(() =>
        {
            LogTextBlock.Text += logEntry;
        });
    }

    private void Shutdown()
    {
        StopServerInternal();
        _mainWindow?.Close();
        Application.Current.Shutdown();
    }
}
