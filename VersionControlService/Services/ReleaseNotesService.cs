using VersionControlService.Models;
using VersionControlService.Storage;

namespace VersionControlService.Services;

public sealed class ReleaseNotesService(
    IConfiguration configuration,
    ReleaseNotesStore releaseNotesStore
)
{
    private const string DefaultReleaseNotes = "Yeni özellikler";

    public async Task EnsureSeedAsync()
    {
        var latestVersion = configuration["LatestVersion"] ?? "1.2.3";
        var releaseNotes = configuration["DefaultReleaseNotes"] ?? DefaultReleaseNotes;
        var releaseDate = DateTime.Parse(configuration["ReleaseDate"] ?? "2025-05-01T00:00:00Z");

        await releaseNotesStore.SeedAsync(
            new ReleaseNoteRecord
            {
                Version = latestVersion,
                Notes = releaseNotes,
                PubDate = releaseDate
            }
        );
    }

    public async Task<string?> TryGetReleaseNotesAsync(string version)
    {
        var releaseNote = await releaseNotesStore.TryGetAsync(version);
        return releaseNote?.Notes;
    }
}