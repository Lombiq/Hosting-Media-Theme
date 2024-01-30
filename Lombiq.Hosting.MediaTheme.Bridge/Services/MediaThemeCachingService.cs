using Lombiq.Hosting.MediaTheme.Bridge.Models;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeCachingService : IMediaThemeCachingService
{
    private const string MediaThemeMemoryCacheKeyPrefix = "Lombiq.Hosting.MediaTheme.Bridge";

    private readonly IMemoryCache _memoryCache;
    private readonly IMediaThemeManager _mediaThemeManager;
    private readonly ISignal _signal;

    public MediaThemeCachingService(
        IMemoryCache memoryCache,
        IMediaThemeManager mediaThemeManager,
        ISignal signal)
    {
        _memoryCache = memoryCache;
        _mediaThemeManager = mediaThemeManager;
        _signal = signal;
    }

    public async Task<MediaTemplate> GetMemoryCachedMediaTemplateAsync(string shapeType)
    {
        var cacheKey = $"{MediaThemeMemoryCacheKeyPrefix}:{shapeType}";
        if (_memoryCache.TryGetValue(cacheKey, out MediaTemplate cachedMediaTemplate))
        {
            return cachedMediaTemplate;
        }

        cachedMediaTemplate = await _mediaThemeManager.GetMediaTemplateByShapeTypeAsync(shapeType);
        _memoryCache.Set(cacheKey, cachedMediaTemplate, _signal.GetToken(MediaThemeMemoryCacheKeyPrefix));

        return cachedMediaTemplate;
    }

    public Task InvalidateCachedMediaThemeTemplatesAsync() => _signal.SignalTokenAsync(MediaThemeMemoryCacheKeyPrefix);
}
