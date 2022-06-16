using Lombiq.Hosting.MediaTheme.Constants;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Services;

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

    public IEnumerable<IFeatureInfo> GetFeatures() =>
        _decorated.GetFeatures();

    public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad) =>
        _decorated.GetFeatures(featureIdsToLoad);

    public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId)
    {
        var dependencies = _decorated.GetFeatureDependencies(featureId);

        if (featureId != FeatureNames.MediaTheme) return dependencies;

        // It'll be retrieved from cache so it's not an issue.
        var baseThemeId = _mediaThemeStateStore.GetMediaThemeStateAsync().GetAwaiter().GetResult()?.BaseTheme;
        var baseTheme = GetFeatures().FirstOrDefault(feature => feature.Id == baseThemeId);
        return baseTheme == null ? dependencies : dependencies.Union(new[] { baseTheme });
    }

    public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId) =>
        _decorated.GetDependentFeatures(featureId);

    public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync() =>
        _decorated.LoadFeaturesAsync();

    public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync(string[] featureIdsToLoad) =>
        _decorated.LoadFeaturesAsync(featureIdsToLoad);
}
