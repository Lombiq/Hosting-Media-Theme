using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Extensions.Manifests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public sealed class ExtensionManagerDecorator : IExtensionManager
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

    public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId)
    {
        static void FindBaseThemes(List<IFeatureInfo> dependencies, IFeatureInfo[] allFeatures, string baseThemeId)
        {
            if (string.IsNullOrWhiteSpace(baseThemeId) ||
                allFeatures.Find(feature => baseThemeId.EqualsOrdinalIgnoreCase(feature.Id)) is not
                { Extension.Manifest: { } manifest } baseTheme)
            {
                return;
            }

            dependencies.Add(baseTheme);

            if (manifest is ManifestInfo { ModuleInfo: ThemeAttribute { BaseTheme: { } ancestorThemeId } })
            {
                FindBaseThemes(dependencies, allFeatures, ancestorThemeId);
            }
        }

        var dependencies = _decorated.GetFeatureDependencies(featureId).ToList();

        if (featureId != FeatureNames.MediaTheme) return dependencies;

        var baseThemeId = GetBaseThemeId();
        if (string.IsNullOrEmpty(baseThemeId)) return dependencies;

        FindBaseThemes(dependencies, GetFeatures().ToArray(), baseThemeId);

        // The base theme has to be the last dependency, see ThemeFeatureBuilderEvents in Orchard's source.
        var mediaTheme = dependencies.First(theme => theme.Id == FeatureNames.MediaTheme);
        dependencies.Remove(mediaTheme);
        dependencies.Add(mediaTheme);

        return dependencies;
    }

    public IExtensionInfo GetExtension(string extensionId) =>
        _decorated.GetExtension(extensionId);

    public IEnumerable<IExtensionInfo> GetExtensions() =>
        _decorated.GetExtensions();

    public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo) =>
        _decorated.LoadExtensionAsync(extensionInfo);

    public IEnumerable<IFeatureInfo> GetFeatures() =>
        _decorated.GetFeatures();

    public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad)
    {
        if (featureIdsToLoad.Contains(FeatureNames.MediaTheme))
        {
            var baseThemeId = GetBaseThemeId();
            if (!string.IsNullOrEmpty(baseThemeId)) featureIdsToLoad = [.. featureIdsToLoad, baseThemeId];
        }

        return _decorated.GetFeatures(featureIdsToLoad.AsEnumerable());
    }

    public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId) =>
        _decorated.GetDependentFeatures(featureId);

    Task<IEnumerable<IFeatureInfo>> IExtensionManager.LoadFeaturesAsync() =>
        _decorated.LoadFeaturesAsync();

    Task<IEnumerable<IFeatureInfo>> IExtensionManager.LoadFeaturesAsync(string[] featureIdsToLoad) =>
        _decorated.LoadFeaturesAsync(featureIdsToLoad.AsEnumerable());

    private string GetBaseThemeId() =>
        // It'll be retrieved from cache, so it's not an issue.
        _mediaThemeStateStore.GetMediaThemeStateAsync().GetAwaiter().GetResult()?.BaseThemeId;
}
