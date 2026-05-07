using System.Text.Json.Serialization;

namespace VersionControlService.Models;

public sealed record ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }

    [JsonPropertyName("error")]
    public string Error { get; init; }
}