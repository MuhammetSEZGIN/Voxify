using System.Text.Json.Serialization;
using VersionControlService.Models;

namespace VersionControlService.Serialization;

[JsonSerializable(typeof(UpdateResponse))]
[JsonSerializable(typeof(PlatformInfo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}