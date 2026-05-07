namespace VersionControlService.Models;

/// <summary>
/// Represents a software release in the database.
/// </summary>
public class ReleaseEntity
{
    /// <summary>
    /// Unique identifier for the release
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Semantic version of the release (e.g., "1.3.0")
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Release notes/changelog
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Publication date of the release
    /// </summary>
    public DateTime PubDate { get; set; }

    /// <summary>
    /// Whether this is the latest release
    /// </summary>
    public bool IsLatest { get; set; }

    /// <summary>
    /// Download artifacts for each target platform
    /// </summary>
    public List<ReleaseArtifactEntity> Artifacts { get; set; } = [];
}
