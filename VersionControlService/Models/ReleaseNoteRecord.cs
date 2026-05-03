namespace VersionControlService.Models;

public sealed record ReleaseNoteRecord
{
    public required string Version { get; init; }

    public required string Notes { get; init; }

    public DateTime PubDate { get; init; }
}