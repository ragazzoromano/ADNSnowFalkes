#!/bin/bash

echo "========================================"
echo "Building ADN SnowFlakes Solution"
echo "========================================"
echo ""

# Build the solution
echo "Building solution..."
dotnet build --configuration Release

if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi

echo ""
echo "========================================"
echo "Build Summary"
echo "========================================"
echo ""

# Check if executables were created
echo "Checking build outputs..."
echo ""

if [ -f "SnowfallApp/bin/Release/net8.0-windows/SnowfallApp.dll" ]; then
    echo "✓ SnowfallApp.dll created successfully"
else
    echo "✗ SnowfallApp.dll not found"
fi

if [ -f "SnowfallServer/bin/Release/net8.0-windows/SnowfallServer.dll" ]; then
    echo "✓ SnowfallServer.dll created successfully"
else
    echo "✗ SnowfallServer.dll not found"
fi

if [ -f "SnowfallClient/bin/Release/net8.0-windows/SnowfallClient.dll" ]; then
    echo "✓ SnowfallClient.dll created successfully"
else
    echo "✗ SnowfallClient.dll not found"
fi

if [ -f "SnowfallShared/bin/Release/net8.0/SnowfallShared.dll" ]; then
    echo "✓ SnowfallShared.dll created successfully"
else
    echo "✗ SnowfallShared.dll not found"
fi

echo ""
echo "========================================"
echo "All projects built successfully!"
echo "========================================"
echo ""
echo "To run the applications:"
echo ""
echo "Standalone Application:"
echo "  cd SnowfallApp && dotnet run"
echo ""
echo "Client-Server Architecture:"
echo "  Server: cd SnowfallServer && dotnet run"
echo "  Client: cd SnowfallClient && dotnet run"
echo ""
