using VersionControlService.Models;
using VersionControlService.Repositories;

namespace VersionControlService.Services;

/// <summary>
/// Business logic service for release catalog management.
/// Handles seeding from configuration and providing release data.
/// </summary>
public class ReleaseCatalogService
{
    private readonly IReleaseRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReleaseCatalogService> _logger;

    public ReleaseCatalogService(
        IReleaseRepository repository,
        IConfiguration configuration,
        ILogger<ReleaseCatalogService> logger)
    {
        _repository = repository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Ensures the database has releases. If empty, does nothing (manual seed via SQL).
    /// </summary>
    public async Task EnsureSeedAsync(CancellationToken cancellationToken = default)
    {
        var hasReleases = await _repository.AnyAsync(cancellationToken);
        if (hasReleases)
        {
            _logger.LogInformation("Database already contains releases");
            return;
        }

        _logger.LogInformation("Database is empty. Manual seed required: insert releases via SQL into VersionControlService/Database");
    }

    /// <summary>
    /// Gets the latest available release.
    /// </summary>
    public async Task<ReleaseEntity?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetLatestAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a specific release by version.
    /// </summary>
    public async Task<ReleaseEntity?> GetByVersionAsync(string version, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByVersionAsync(version, cancellationToken);
    }
}
