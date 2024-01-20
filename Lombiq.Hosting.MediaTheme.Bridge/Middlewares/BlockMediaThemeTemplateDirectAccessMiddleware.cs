using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OrchardCore.Media;
using OrchardCore.Routing;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Middlewares;

public class BlockMediaThemeTemplateDirectAccessMiddleware(
    RequestDelegate next,
    IOptions<MediaOptions> mediaOptions)
{
    private readonly PathString _assetsRequestPath = mediaOptions.Value.AssetsRequestPath;

    public async Task InvokeAsync(HttpContext context)
    {
        var isMediaThemeTemplateRequest = context.Request.Path.StartsWithNormalizedSegments(
            _assetsRequestPath + "/" + Paths.MediaThemeTemplatesWebPath,
            StringComparison.OrdinalIgnoreCase,
            out _);

        // Since this middleware needs to run early (see comment in Startup), the user's authentication state won't yet
        // be available. So, we can't let people with the ManageMediaTheme permission still see the templates.
        if (!isMediaThemeTemplateRequest)
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = 404;
        context.Response.Headers.Append("Content-Length", "0");
        await context.Response.Body.FlushAsync(context.RequestAborted);
        context.Abort();
    }
}
