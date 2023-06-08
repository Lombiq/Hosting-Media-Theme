using Lombiq.Hosting.MediaTheme.Bridge.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

/// <summary>
/// Service for Media Theme-specific operations.
/// </summary>
public interface IMediaThemeManager
{
    /// <summary>
    /// Sets the given theme as a base theme dynamically.
    /// </summary>
    /// <param name="baseThemeId">ID of the base theme extension.</param>
    Task UpdateBaseThemeAsync(string baseThemeId);

    /// <summary>
    /// Returns the available theme names and IDs to select as a base theme.
    /// </summary>
    /// <returns>List of available base theme names and IDs.</returns>
    Task<IEnumerable<(string Id, string Name)>> GetAvailableBaseThemesAsync();

    /// <summary>
    /// Returns a shape template content placed in the Media Library. Returns null if it doesn't exists.
    /// </summary>
    Task<MediaTemplate> GetMediaTemplateByShapeTypeAsync(string shapeType);
}
