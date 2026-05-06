namespace VersionControlService.Models;

/// <summary>
/// Represents a download artifact for a specific platform in a release.
/// </summary>
public class ReleaseArtifactEntity
{
    /// <summary>
    /// Platform target identifier (e.g., "windows-x86_64", "windows-x86")
    /// </summary>
    public required string Target { get; set; }

    /// <summary>
    /// Base64-encoded signature for download verification
    /// </summary>
    public required string Signature { get; set; }

    /// <summary>
    /// Download URL for the artifact
    /// </summary>
    public required string Url { get; set; }
}
