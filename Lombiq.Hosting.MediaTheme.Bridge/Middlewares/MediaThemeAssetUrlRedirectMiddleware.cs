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

        var assetPath = context.Request.Path.Value?.Replace(
            Routes.MediaThemeAssets,
            string.Empty);
        string assetUrl;
        if (!context.IsDevelopment() || await mediaFileStore.FileExistsAsync(assetPath))
        {
            assetUrl = mediaFileStore.MapPathToPublicUrl(Paths.MediaThemeAssetsWebPath + assetPath);
        }
        else
        {
            var activeTheme = await siteThemeService.GetSiteThemeAsync();

            assetUrl = "/" + activeTheme.Id + assetPath;
        }

#pragma warning disable SCS0027 // URL starts with "/mediatheme" which is checked above.
        context.Response.Redirect(assetUrl);
#pragma warning restore SCS0027
    }
}
