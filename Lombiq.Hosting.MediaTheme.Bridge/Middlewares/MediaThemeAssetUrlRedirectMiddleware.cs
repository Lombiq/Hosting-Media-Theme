using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Themes.Services;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Middlewares;

public class MediaThemeAssetUrlRedirectMiddleware
{
    private readonly RequestDelegate _next;

    private readonly PathString _assetsRequestPath;

    public MediaThemeAssetUrlRedirectMiddleware(
        RequestDelegate next,
        IOptions<MediaOptions> mediaOptions)
    {
        _next = next;
        _assetsRequestPath = mediaOptions.Value.AssetsRequestPath;
    }

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
            Paths.MediaThemeAssetsWebPath);
        string assetUrl;
        if (!context.IsDevelopment() || await mediaFileStore.FileExistsAsync(assetPath))
        {
            assetUrl = mediaFileStore.MapPathToPublicUrl(assetPath);
        }
        else
        {
            var activeTheme = await siteThemeService.GetSiteThemeAsync();

            assetUrl = "/" + activeTheme.Id + assetPath.Replace(Paths.MediaThemeAssetsWebPath, string.Empty);
        }

        context.Response.Redirect(assetUrl);
    }
}
