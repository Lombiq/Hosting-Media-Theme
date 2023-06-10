using Lombiq.Hosting.MediaTheme.Bridge.Models;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

/// <summary>
/// Manages the stored Media Theme state (e.g., base theme).
/// </summary>
public interface IMediaThemeStateStore
{
    /// <summary>
    /// Loads the Media Theme store for editing.
    /// </summary>
    Task<MediaThemeStateDocument> LoadMediaThemeStateAsync();

    /// <summary>
    /// Fetches the Media Theme store from cache. Not suitable for updating.
    /// </summary>
    Task<MediaThemeStateDocument> GetMediaThemeStateAsync();

    /// <summary>
    /// Saves the updated state document.
    /// </summary>
    Task SaveMediaThemeStateAsync(MediaThemeStateDocument state);
}
