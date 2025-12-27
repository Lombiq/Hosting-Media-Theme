#nullable enable

using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.DisplayManagement.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeResourceFilterThemeResolver : IResourceFilterThemeResolver
{
    private readonly IMediaThemeStateStore _store;

    public MediaThemeResourceFilterThemeResolver(IMediaThemeStateStore store) => _store = store;

    public Task UpdateThemeNamesAsync(
        IList<string> themeNames,
        IDictionary<string, IThemeExtensionInfo> themes,
        IList<ResourceFilterMiddleware.ProviderInfo> providers) =>
        themeNames.Contains(FeatureNames.MediaTheme, StringComparer.OrdinalIgnoreCase)
            ? UpdateThemeNamesInnerAsync(themeNames)
            : Task.CompletedTask;

    private async Task UpdateThemeNamesInnerAsync(IList<string> themeNames)
    {
        if (await _store.GetMediaThemeStateAsync() is { BaseThemeId: { Length: > 0 } baseThemeId })
        {
            themeNames.Add(baseThemeId);
        }
    }
}
