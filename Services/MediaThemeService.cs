using Lombiq.Hosting.MediaTheme.Constants;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules.Manifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Services;

public class MediaThemeService : IMediaThemeService
{
    private readonly IMediaThemeStateStore _mediaThemeStateStore;
    private readonly IShellFeaturesManager _shellFeaturesManager;
    private readonly IMemoryCache _memoryCache;

    public MediaThemeService(
        IMediaThemeStateStore mediaThemeStateStore,
        IShellFeaturesManager shellFeaturesManager,
        IMemoryCache memoryCache)
    {
        _mediaThemeStateStore = mediaThemeStateStore;
        _shellFeaturesManager = shellFeaturesManager;
        _memoryCache = memoryCache;
    }

    public async Task UpdateBaseThemeAsync(string baseThemeId)
    {
        if (!string.IsNullOrEmpty(baseThemeId))
        {
            if (baseThemeId == FeatureNames.MediaTheme)
            {
                throw new ArgumentException("The Media Theme can't be its own base theme.", nameof(baseThemeId));
            }

            var baseThemeFeature = (await _shellFeaturesManager.GetAvailableFeaturesAsync())
                .FirstOrDefault(feature => ExtensionInfoExtensions.IsTheme((IFeatureInfo) feature) && feature.Id == baseThemeId);
            if (baseThemeFeature == null)
            {
                throw new ArgumentException($"Theme with the given ID ({baseThemeId}) doesn't exist.", nameof(baseThemeId));
            }

            await _shellFeaturesManager.EnableFeaturesAsync(new[] { baseThemeFeature }, force: true);
        }

        var state = await _mediaThemeStateStore.LoadMediaThemeStateAsync();
        state.BaseThemeId = baseThemeId;
        await _mediaThemeStateStore.SaveMediaThemeStateAsync(state);

        _memoryCache.Remove($"ShapeTable:{FeatureNames.MediaTheme}");
    }

    public async Task<IEnumerable<(string Id, string Name)>> GetAvailableThemesAsync()
    {
        var enabledFeatures = await _shellFeaturesManager.GetEnabledFeaturesAsync();
        return (await _shellFeaturesManager.GetAvailableFeaturesAsync())
            .Where(feature =>
                feature.IsTheme() &&
                feature.Id != FeatureNames.MediaTheme &&
                !feature.Extension.Manifest.Tags.Any(tag =>
                    tag.Equals("hidden", StringComparison.OrdinalIgnoreCase) ||
                    tag.Equals(ManifestConstants.AdminTag, StringComparison.OrdinalIgnoreCase)) &&
                enabledFeatures.Any(enabledFeature => enabledFeature.Id == feature.Id))
            .Select(feature => (feature.Id, feature.Name));
    }
}
