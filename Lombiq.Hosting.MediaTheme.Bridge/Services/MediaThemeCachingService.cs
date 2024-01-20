using Lombiq.Hosting.MediaTheme.Bridge.Models;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeCachingService(
    IMemoryCache memoryCache,
    IMediaThemeManager mediaThemeManager,
    ISignal signal) : IMediaThemeCachingService
{
    private const string MediaThemeMemoryCacheKeyPrefix = "Lombiq.Hosting.MediaTheme.Bridge";

    public async Task<MediaTemplate> GetMemoryCachedMediaTemplateAsync(string shapeType)
    {
        var cacheKey = $"{MediaThemeMemoryCacheKeyPrefix}:{shapeType}";
        if (memoryCache.TryGetValue(cacheKey, out MediaTemplate cachedMediaTemplate))
        {
            return cachedMediaTemplate;
        }

        cachedMediaTemplate = await mediaThemeManager.GetMediaTemplateByShapeTypeAsync(shapeType);
        memoryCache.Set(cacheKey, cachedMediaTemplate, signal.GetToken(MediaThemeMemoryCacheKeyPrefix));

        return cachedMediaTemplate;
    }

    public Task InvalidateCachedMediaThemeTemplatesAsync() => signal.SignalTokenAsync(MediaThemeMemoryCacheKeyPrefix);
}
