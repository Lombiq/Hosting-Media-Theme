using Lombiq.Hosting.MediaTheme.Models;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Services;

/// <summary>
/// Manages shape templates placed in the Media Library.
/// </summary>
public interface IMediaTemplateService
{
    /// <summary>
    /// Returns a shape template content placed in the Media Library. Returns null if it doesn't exists.
    /// </summary>
    Task<MediaTemplate> GetMediaTemplateByShapeTypeAsync(string shapeType);
}
