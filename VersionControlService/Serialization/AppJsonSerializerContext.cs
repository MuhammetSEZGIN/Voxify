using System.Text.Json.Serialization;
using VersionControlService.Models;

namespace VersionControlService.Serialization;

[JsonSerializable(typeof(UpdateResponse))]
[JsonSerializable(typeof(PlatformInfo))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(ReleaseEntity))]
[JsonSerializable(typeof(ReleaseArtifactEntity))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}