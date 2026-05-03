using System.Text.Json.Serialization;
using VersionControlService.Endpoints;
using VersionControlService.Serialization;
using VersionControlService.Services;

var builder = WebApplication.CreateSlimBuilder(args);
var configuredTargets = builder.Configuration
    .GetSection("Platforms")
    .GetChildren()
    .Select(section => section.Key)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);

if (configuredTargets.Count == 0)
{
    configuredTargets = new[] { "windows-x86_64" }.ToHashSet(StringComparer.OrdinalIgnoreCase);
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<VersionControlService.Storage.ReleaseNotesStore>();
builder.Services.AddSingleton<ReleaseNotesService>();

var app = builder.Build();

await app.Services.GetRequiredService<ReleaseNotesService>().EnsureSeedAsync();

app.MapVersionControlEndpoints(configuredTargets);

app.Run();
