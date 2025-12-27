# ADN SnowFlakes - Client-Server Architecture

This solution now includes three projects:

## Projects

### 1. SnowfallApp (Standalone)
The original standalone application with both the display and control panel in a single application.
- **Main Window**: Displays the snowfall animation
- **Control Window**: Provides settings and controls
- Runs as a single integrated application

### 2. SnowfallServer
The server application that displays the snowfall animation and accepts commands via TCP.
- **Server Window**: Controls for starting/stopping the TCP server and viewing logs
  - Configure port (default: 8888)
  - Start/Stop server
  - View connection logs and received commands
- **Main Window**: Displays the snowfall animation (can be shown/hidden)
- Listens for TCP connections from clients
- Processes commands: Start, Pause, Stop, UpdateSettings, Shutdown

### 3. SnowfallClient
The client application that provides the settings control panel and connects to the server.
- **Connection Settings**: IP address and port configuration
- **Control Panel**: All snowfall settings (density, speed, size, blur, appearance)
- **Animation Controls**: Start, Pause, Stop buttons
- Sends settings updates in real-time to the connected server

### 4. SnowfallShared
Shared library containing:
- Communication protocol (Command, CommandType)
- Data transfer objects (SettingsDto)
- Used by both server and client for TCP communication

## How to Use

### Running the Standalone Application
```bash
cd SnowfallApp
dotnet run
```
The standalone app works exactly as before with both windows integrated.

### Running the Client-Server Architecture

#### Start the Server:
```bash
cd SnowfallServer
dotnet run
```
1. Configure the port (default: 8888)
2. Click "Start Server" to begin listening
3. Click "Show Snowfall Display" to show the animation window
4. Monitor the log for client connections and commands

#### Start the Client:
```bash
cd SnowfallClient
dotnet run
```
1. Enter the server IP address (default: 127.0.0.1 for local)
2. Enter the port (default: 8888)
3. Click "Connect" to establish connection
4. Use the control panel to adjust settings - changes are sent immediately
5. Use Start/Pause/Stop buttons to control the animation

## Communication Protocol

The client and server communicate via TCP using JSON-formatted commands:

### Command Types:
- **StartAnimation**: Starts the snowfall animation
- **PauseAnimation**: Pauses/resumes the animation
- **StopAnimation**: Stops the animation and clears all snowflakes
- **UpdateSettings**: Updates all snowfall settings
- **Shutdown**: Shuts down the server application

### Settings Parameters:
All snowfall parameters are synchronized between client and server:
- Snow density (small, medium, large)
- Fall speed (small, medium, large)
- Flake size (small, medium, large)
- Blur effect counts and intensity
- Snowflake shape
- Rotation speed
- Line width

## Building the Solution

Build all projects:
```bash
dotnet build
```

Build specific project:
```bash
dotnet build SnowfallApp/SnowfallApp.csproj
dotnet build SnowfallServer/SnowfallServer.csproj
dotnet build SnowfallClient/SnowfallClient.csproj
```

## Notes

- The original SnowfallApp standalone application remains unchanged and fully functional
- Server can handle only one client connection at a time
- Settings changes are applied immediately when connected
- Connection status is displayed in the client window
- Server logs all client connections and commands
- The snowfall display window can be toggled to fullscreen by double-clicking
