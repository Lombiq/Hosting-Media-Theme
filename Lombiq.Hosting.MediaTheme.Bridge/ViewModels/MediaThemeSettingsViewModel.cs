using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Lombiq.Hosting.MediaTheme.Bridge.ViewModels;

public class MediaThemeSettingsViewModel
{
    [BindNever]
    public IEnumerable<(string Id, string Name)> AvailableBaseThemes { get; set; }

    [BindNever]
    public bool IsMediaThemeActive { get; set; }

    public string BaseThemeId { get; set; }
}
