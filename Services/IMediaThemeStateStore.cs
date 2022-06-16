using Lombiq.Hosting.MediaTheme.Models;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Services;

/// <summary>
/// Manages the stored media theme state (e.g., base theme).
/// </summary>
public interface IMediaThemeStateStore
{
    /// <summary>
    /// Loads the media theme store for editing.
    /// </summary>
    Task<MediaThemeStateDocument> LoadMediaThemeStateAsync();

    /// <summary>
    /// Fetches the media theme store from cache. Not suitable for updating.
    /// </summary>
    Task<MediaThemeStateDocument> GetMediaThemeStateAsync();

    /// <summary>
    /// Saves the updated state document.
    /// </summary>
    Task SaveMediaThemeStateAsync(MediaThemeStateDocument state);
}
