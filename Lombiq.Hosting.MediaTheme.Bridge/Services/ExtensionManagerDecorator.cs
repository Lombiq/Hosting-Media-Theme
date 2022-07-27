using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class ExtensionManagerDecorator : IExtensionManager
{
    private readonly IExtensionManager _decorated;
    private readonly IMediaThemeStateStore _mediaThemeStateStore;

    public ExtensionManagerDecorator(
        IExtensionManager decorated,
        IMediaThemeStateStore mediaThemeStateStore)
    {
        _decorated = decorated;
        _mediaThemeStateStore = mediaThemeStateStore;
    }

    public IExtensionInfo GetExtension(string extensionId) =>
        _decorated.GetExtension(extensionId);

    public IEnumerable<IExtensionInfo> GetExtensions() =>
        _decorated.GetExtensions();

    public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo) =>
        _decorated.LoadExtensionAsync(extensionInfo);

    public IEnumerable<IFeatureInfo> GetFeatures()
    {
        var features = _decorated.GetFeatures().ToList();

        var mediaTheme = features.FirstOrDefault(feature => feature.Id == FeatureNames.MediaTheme);
        if (mediaTheme == null) return features;

        // Put the Media Theme to the end so it'll be the highest priority during shape harvesting.
        features.Remove(mediaTheme);
        features.Add(mediaTheme);

        return features;
    }

    public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad) =>
        _decorated.GetFeatures(featureIdsToLoad);

    public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId)
    {
        var dependencies = _decorated.GetFeatureDependencies(featureId).ToList();

        if (featureId != FeatureNames.MediaTheme) return dependencies;

        // It'll be retrieved from cache so it's not an issue.
        var baseThemeId = _mediaThemeStateStore.GetMediaThemeStateAsync().GetAwaiter().GetResult()?.BaseThemeId;
        if (string.IsNullOrEmpty(baseThemeId)) return dependencies;

        var baseTheme = GetFeatures().FirstOrDefault(feature => feature.Id == baseThemeId);
        var mediaTheme = dependencies.First(theme => theme.Id == FeatureNames.MediaTheme);
        dependencies.Remove(mediaTheme);
        dependencies.Add(baseTheme);
        dependencies.Add(mediaTheme);
        return dependencies;
    }

    public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId) =>
        _decorated.GetDependentFeatures(featureId);

    public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync() =>
        _decorated.LoadFeaturesAsync();

    public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync(string[] featureIdsToLoad) =>
        _decorated.LoadFeaturesAsync(featureIdsToLoad);
}
