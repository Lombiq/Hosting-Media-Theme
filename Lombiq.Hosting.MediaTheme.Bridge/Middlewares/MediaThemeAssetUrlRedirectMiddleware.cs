using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Http;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Themes.Services;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Middlewares;

public class MediaThemeAssetUrlRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public MediaThemeAssetUrlRedirectMiddleware(RequestDelegate next) =>
        _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IMediaFileStore mediaFileStore,
        ISiteThemeService siteThemeService)
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
        var mediaPath = mediaFileStore.Combine(Paths.MediaThemeRootFolder, Paths.MediaThemeAssetsFolder, assetRelativePath);
        string assetUrl;
        if (!context.IsDevelopment() || await mediaFileStore.FileExistsAsync(mediaPath))
        {
            assetUrl = mediaFileStore.MapPathToPublicUrl(mediaPath);
        }
        else
        {
            var activeTheme = await siteThemeService.GetSiteThemeAsync();

            assetUrl = "/" + activeTheme.Id + assetRelativePath;
        }

        // URL starts with "/mediatheme" which is checked above.
#pragma warning disable SCS0027 // Potential Open Redirect vulnerability.
        context.Response.Redirect(assetUrl);
#pragma warning restore SCS0027 // Potential Open Redirect vulnerability.
    }
}
