using System.Text.Json.Serialization;

namespace VersionControlService.Models;

public record PlatformInfo
{
    [JsonPropertyName("signature")]
    public required string Signature { get; init; }

    [JsonPropertyName("url")]
    public required string Url { get; init; }
}