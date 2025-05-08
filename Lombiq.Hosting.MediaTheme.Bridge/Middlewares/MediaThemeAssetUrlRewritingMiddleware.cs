using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Lombiq.Hosting.MediaTheme.Bridge.Helpers;
using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Themes.Services;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Middlewares;

/// <summary>
/// Middleware to rewrite requests coming to ~/mediatheme to the real (Media or theme) paths the files are properly
/// loaded behind the scenes.
/// </summary>
internal sealed class MediaThemeAssetUrlRewritingMiddleware
{
    private readonly RequestDelegate _next;

    public MediaThemeAssetUrlRewritingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IMediaFileStore mediaFileStore,
        ISiteThemeService siteThemeService,
        ShellSettings shellSettings)
    {
        var isMediaThemeRequest = context.Request.Path
            .StartsWithSegments(new PathString(Routes.MediaThemeAssets), StringComparison.OrdinalIgnoreCase, out _);

        if (!isMediaThemeRequest)
        {
            await _next(context);
            return;
        }

        var assetRelativePath = context.Request.Path.Value?.Replace(
            Routes.MediaThemeAssets,
            string.Empty);

        await GenerateMediaThemeAssetUrlAsync(context, mediaFileStore, siteThemeService, shellSettings, assetRelativePath);

        await _next(context);
    }

    private static async Task GenerateMediaThemeAssetUrlAsync(
        HttpContext context,
        IMediaFileStore mediaFileStore,
        ISiteThemeService siteThemeService,
        ShellSettings shellSettings,
        string assetRelativePath)
    {
        var mediaPath = mediaFileStore.Combine(Paths.MediaThemeRootFolder, Paths.MediaThemeAssetsFolder, assetRelativePath);
        string assetUrl;
        var siteThemeId = (await siteThemeService.GetSiteThemeAsync()).Id;
        if (siteThemeId == FeatureNames.MediaTheme && await mediaFileStore.FileExistsAsync(mediaPath))
        {
            assetUrl = RequestUrlPrefixRemover.RemoveIfHasPrefix(mediaFileStore.MapPathToPublicUrl(mediaPath), shellSettings);

            if (!string.IsNullOrEmpty(shellSettings.RequestUrlPrefix) &&
                assetUrl.StartsWith("/" + shellSettings.RequestUrlPrefix, StringComparison.OrdinalIgnoreCase))
            {
                assetUrl = assetUrl[(shellSettings.RequestUrlPrefix.Length + 1)..];
            }
        }
        else
        {
            assetUrl = $"/{siteThemeId}{assetRelativePath}";
        }

        context.Request.Path = assetUrl;
    }
}
