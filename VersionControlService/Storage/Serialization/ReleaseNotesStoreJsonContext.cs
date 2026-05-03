using System.Text.Json.Serialization;
using VersionControlService.Models;

namespace VersionControlService.Storage.Serialization;

[JsonSerializable(typeof(Dictionary<string, ReleaseNoteRecord>))]
internal partial class ReleaseNotesStoreJsonContext : JsonSerializerContext
{
}