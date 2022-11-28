using Lombiq.Hosting.MediaTheme.Bridge.Models;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

/// <summary>
/// Media Theme template cache store and invalidation functions.
/// </summary>
public interface IMediaThemeCachingService
{
    /// <summary>
    /// Returns <see cref="MediaTemplate"/> if cached, else tries to fetch it from the storage. If not available stores
    /// and returns <see langword="null"/>.
    /// </summary>
    /// <param name="shapeType">The searched shape type.</param>
    Task<MediaTemplate> GetMemoryCachedMediaTemplateAsync(string shapeType);

    /// <summary>
    /// Deletes all Media Theme templates from the cache.
    /// </summary>
    Task InvalidateCachedMediaThemeTemplatesAsync();
}
