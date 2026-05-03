using VersionControlService.Models;
using VersionControlService.Services;

namespace VersionControlService.Endpoints;

public static class VersionControlEndpoints
{
    public static IEndpointRouteBuilder MapVersionControlEndpoints(
        this IEndpointRouteBuilder app,
        ISet<string> configuredTargets)
    {
        app.MapGet(
            "/update/{target}/{currentVersion}",
            async (
                HttpContext httpContext,
                string target,
                string currentVersion,
                IConfiguration configuration,
                ReleaseNotesService releaseNotesService) =>
            {
                if (!configuredTargets.Contains(target))
                {
                    return Results.BadRequest(new { error = "Unknown target" });
                }

                var latestVersion = configuration["LatestVersion"] ?? "1.2.3";

                if (
                    !Version.TryParse(currentVersion, out var clientVersion)
                    || !Version.TryParse(latestVersion, out var serverVersion)
                )
                {
                    return Results.BadRequest(new { error = "Invalid version format" });
                }

                if (clientVersion >= serverVersion)
                {
                    return Results.NoContent();
                }

                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                var platforms = new Dictionary<string, PlatformInfo>(StringComparer.OrdinalIgnoreCase);

                foreach (var configuredTarget in configuredTargets)
                {
                    var signature = configuration[$"Platforms:{configuredTarget}:Signature"] ?? string.Empty;
                    var downloadUrl = $"{baseUrl}/download/{configuredTarget}/{latestVersion}";

                    platforms[configuredTarget] = new PlatformInfo
                    {
                        Signature = signature,
                        Url = downloadUrl
                    };
                }

                var updateInfo = new UpdateResponse
                {
                    Version = latestVersion,
                    Notes = await releaseNotesService.TryGetReleaseNotesAsync(latestVersion)
                        ?? configuration["DefaultReleaseNotes"]
                        ?? "Yeni özellikler:\n- Bildirim desteği\n- Ses cihazı seçimi",
                    PubDate = DateTime.Parse(configuration["ReleaseDate"] ?? "2025-05-01T00:00:00Z"),
                    Platforms = platforms
                };

                return Results.Ok(updateInfo);
            }
        );

        app.MapGet(
            "/download/{target}/{version}",
            (string target, string version, IConfiguration configuration) =>
            {
                if (!configuredTargets.Contains(target))
                {
                    return Results.NotFound();
                }

                var latestVersion = configuration["LatestVersion"] ?? "1.2.3";
                if (!string.Equals(version, latestVersion, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.NotFound();
                }

                var artifactUrl = configuration[$"Platforms:{target}:Url"];
                if (string.IsNullOrWhiteSpace(artifactUrl))
                {
                    return Results.NotFound();
                }

                return Results.Redirect(artifactUrl);
            }
        );

        return app;
    }
}