using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public interface IMediaThemeService
{
    Task UpdateBaseThemeAsync(string baseTheme);
    Task<IEnumerable<(string Id, string Name)>> GetAvailableThemesAsync();
}
