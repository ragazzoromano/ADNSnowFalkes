using System.Text.Json;

namespace SnowfallShared.Protocol;

public class Command
{
    public CommandType Type { get; set; }
    public string? Data { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Command? FromJson(string json)
    {
        return JsonSerializer.Deserialize<Command>(json);
    }
}
