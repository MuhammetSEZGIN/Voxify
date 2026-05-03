using System.Text.Json.Serialization;

namespace VersionControlService.Models;

public record UpdateResponse
{
    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("notes")]
    public required string Notes { get; init; }

    [JsonPropertyName("pub_date")]
    public DateTime PubDate { get; init; }

    [JsonPropertyName("platforms")]
    public required Dictionary<string, PlatformInfo> Platforms { get; init; }
}