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
                ReleaseCatalogService catalogService) =>
            {
                if (!configuredTargets.Contains(target))
                {
                    return Results.BadRequest(new ErrorResponse("Unknown target"));
                }

                var latestRelease = await catalogService.GetLatestAsync();
                if (latestRelease == null)
                {
                    return Results.BadRequest(new ErrorResponse("No releases available"));
                }

                // Verify target has an artifact
                var artifact = latestRelease.Artifacts.FirstOrDefault(a => 
                    a.Target.Equals(target, StringComparison.OrdinalIgnoreCase));
                if (artifact == null)
                {
                    return Results.BadRequest(new ErrorResponse($"No artifact for target {target}"));
                }

                if (
                    !Version.TryParse(currentVersion, out var clientVersion)
                    || !Version.TryParse(latestRelease.Version, out var serverVersion)
                )
                {
                    return Results.BadRequest(new ErrorResponse("Invalid version format"));
                }

                if (clientVersion >= serverVersion)
                {
                    return Results.NoContent();
                }

                // Build response with artifacts from database
                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                var platforms = new Dictionary<string, PlatformInfo>(StringComparer.OrdinalIgnoreCase);

                foreach (var releaseArtifact in latestRelease.Artifacts)
                {
                    var downloadUrl = $"{baseUrl}/download/{releaseArtifact.Target}/{latestRelease.Version}";
                    platforms[releaseArtifact.Target] = new PlatformInfo
                    {
                        Signature = releaseArtifact.Signature,
                        Url = downloadUrl
                    };
                }

                var updateInfo = new UpdateResponse
                {
                    Version = latestRelease.Version,
                    Notes = latestRelease.Notes ?? "Yeni özellikler:\n- Bildirim desteği\n- Ses cihazı seçimi",
                    PubDate = latestRelease.PubDate,
                    Platforms = platforms
                };

                return Results.Ok(updateInfo);
            }
        );

        app.MapGet(
            "/download/{target}/{version}",
            async (string target, string version, ReleaseCatalogService catalogService) =>
            {
                if (!configuredTargets.Contains(target))
                {
                    return Results.NotFound();
                }

                var release = await catalogService.GetByVersionAsync(version);
                if (release == null)
                {
                    return Results.NotFound();
                }

                var artifact = release.Artifacts.FirstOrDefault(a =>
                    a.Target.Equals(target, StringComparison.OrdinalIgnoreCase));
                if (artifact == null)
                {
                    return Results.NotFound();
                }

                return Results.Redirect(artifact.Url);
            }
        );

        return app;
    }
}