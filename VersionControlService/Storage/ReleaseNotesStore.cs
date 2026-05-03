using System.Text.Json;
using VersionControlService.Models;
using VersionControlService.Storage.Serialization;

namespace VersionControlService.Storage;

public sealed class ReleaseNotesStore
{
    private readonly string filePath;

    public ReleaseNotesStore(IHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDirectory);
        filePath = Path.Combine(dataDirectory, "release-notes.json");
    }

    public async Task SeedAsync(ReleaseNoteRecord releaseNote)
    {
        var currentNotes = await ReadAllAsync();
        currentNotes[releaseNote.Version] = releaseNote;
        await WriteAllAsync(currentNotes);
    }

    public async Task<ReleaseNoteRecord?> TryGetAsync(string version)
    {
        var currentNotes = await ReadAllAsync();
        currentNotes.TryGetValue(version, out var releaseNote);
        return releaseNote;
    }

    private async Task<Dictionary<string, ReleaseNoteRecord>> ReadAllAsync()
    {
        if (!File.Exists(filePath))
        {
            return new Dictionary<string, ReleaseNoteRecord>(StringComparer.OrdinalIgnoreCase);
        }

        await using var stream = File.OpenRead(filePath);
        var notes = await JsonSerializer.DeserializeAsync(
            stream,
            ReleaseNotesStoreJsonContext.Default.DictionaryStringReleaseNoteRecord
        );
        return notes ?? new Dictionary<string, ReleaseNoteRecord>(StringComparer.OrdinalIgnoreCase);
    }

    private async Task WriteAllAsync(Dictionary<string, ReleaseNoteRecord> notes)
    {
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(
            stream,
            notes,
            ReleaseNotesStoreJsonContext.Default.DictionaryStringReleaseNoteRecord
        );
    }
}