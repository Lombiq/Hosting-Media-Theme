using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.DisplayManagement.Extensions;
using System.Collections.Generic;

namespace Lombiq.Hosting.MediaTheme.ViewModels;

public class MediaThemeSettingsViewModel
{
    [BindNever]
    public IEnumerable<(string Id, string Name)> AvailableBaseThemes { get; set; }

    public string BaseThemeId { get; set; }
}
