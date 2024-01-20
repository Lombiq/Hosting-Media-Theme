using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class ExtensionManagerDecorator(
    IExtensionManager decorated,
    IMediaThemeStateStore mediaThemeStateStore) : IExtensionManager
{
    public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId)
    {
        var dependencies = decorated.GetFeatureDependencies(featureId).ToList();

        if (featureId != FeatureNames.MediaTheme) return dependencies;

        var baseThemeId = GetBaseThemeId();
        if (string.IsNullOrEmpty(baseThemeId)) return dependencies;

        var allFeatures = GetFeatures().ToArray();
        var baseTheme = allFeatures.Find(feature => feature.Id == baseThemeId);
        dependencies.Add(baseTheme);

        // The base theme has to be the last dependency, see ThemeFeatureBuilderEvents in Orchard's source.
        var mediaTheme = dependencies.First(theme => theme.Id == FeatureNames.MediaTheme);
        dependencies.Remove(mediaTheme);
        dependencies.Add(mediaTheme);

        return dependencies;
    }

    public IExtensionInfo GetExtension(string extensionId) =>
        decorated.GetExtension(extensionId);

    public IEnumerable<IExtensionInfo> GetExtensions() =>
        decorated.GetExtensions();

    public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo) =>
        decorated.LoadExtensionAsync(extensionInfo);

    public IEnumerable<IFeatureInfo> GetFeatures() =>
        decorated.GetFeatures();

    public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad)
    {
        if (featureIdsToLoad.Contains(FeatureNames.MediaTheme))
        {
            var baseThemeId = GetBaseThemeId();
            if (!string.IsNullOrEmpty(baseThemeId)) featureIdsToLoad = [.. featureIdsToLoad, baseThemeId];
        }

        return decorated.GetFeatures(featureIdsToLoad);
    }

    public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId) =>
        decorated.GetDependentFeatures(featureId);

    public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync() =>
        decorated.LoadFeaturesAsync();

    public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync(string[] featureIdsToLoad) =>
        decorated.LoadFeaturesAsync(featureIdsToLoad);

    private string GetBaseThemeId() =>
        // It'll be retrieved from cache so it's not an issue.
        mediaThemeStateStore.GetMediaThemeStateAsync().GetAwaiter().GetResult()?.BaseThemeId;
}
