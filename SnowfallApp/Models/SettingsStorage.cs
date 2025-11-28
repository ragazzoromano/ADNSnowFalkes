using System;
using System.IO;
using System.Text.Json;

namespace SnowfallApp.Models;

public static class SettingsStorage
{
    private const string FileName = "snowfall-settings.json";

    private static string ConfigDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnowfallApp");

    private static string ConfigPath => Path.Combine(ConfigDirectory, FileName);

    public static SnowfallSettings Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var loaded = JsonSerializer.Deserialize<SnowfallSettings>(json);
                if (loaded is not null)
                {
                    return loaded;
                }
            }
        }
        catch
        {
            // If loading fails, fall back to defaults
        }

        return new SnowfallSettings();
    }

    public static void Save(SnowfallSettings settings)
    {
        Directory.CreateDirectory(ConfigDirectory);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(ConfigPath, json);
    }
}
