using VersionControlService.Models;

namespace VersionControlService.Repositories;

/// <summary>
/// Repository interface for release data access.
/// </summary>
public interface IReleaseRepository
{
    /// <summary>
    /// Gets the latest available release.
    /// </summary>
    Task<ReleaseEntity?> GetLatestAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific release by version.
    /// </summary>
    Task<ReleaseEntity?> GetByVersionAsync(string version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a release and marks it as latest.
    /// </summary>
    Task UpsertLatestAsync(ReleaseEntity release, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any releases exist in the database.
    /// </summary>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all releases.
    /// </summary>
    Task<List<ReleaseEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
