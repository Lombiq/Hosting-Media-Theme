using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Lombiq.Hosting.MediaTheme.Bridge.Models;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.Environment.Shell;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Modules.Manifest;
using OrchardCore.Themes.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeManager(
    IMediaThemeStateStore mediaThemeStateStore,
    IShellFeaturesManager shellFeaturesManager,
    IMemoryCache memoryCache,
    IMediaFileStore mediaFileStore,
    ISiteThemeService siteThemeService) : IMediaThemeManager
{
    public async Task UpdateBaseThemeAsync(string baseThemeId)
    {
        ThrowIfBaseThemeIdIsInvalid(baseThemeId);

        if (!string.IsNullOrEmpty(baseThemeId))
        {
            var baseThemeFeature = (await shellFeaturesManager.GetAvailableFeaturesAsync())
                .FirstOrDefault(feature => feature.IsTheme() && feature.Id == baseThemeId)
                ?? throw new ArgumentException($"Theme with the given ID ({baseThemeId}) doesn't exist.", nameof(baseThemeId));
            await shellFeaturesManager.EnableFeaturesAsync(new[] { baseThemeFeature }, force: true);
        }

        var state = await mediaThemeStateStore.LoadMediaThemeStateAsync();
        state.BaseThemeId = baseThemeId;
        await mediaThemeStateStore.SaveMediaThemeStateAsync(state);

        // Invalidate the cache to have the harvesters include the shapes from the base theme.
        var currentTheme = await siteThemeService.GetSiteThemeAsync();
        if (currentTheme.Id == FeatureNames.MediaTheme)
        {
            memoryCache.Remove($"ShapeTable:{currentTheme.Id}");
        }
    }

    public async Task<IEnumerable<(string Id, string Name)>> GetAvailableBaseThemesAsync()
    {
        var enabledFeatures = await shellFeaturesManager.GetEnabledFeaturesAsync();
        return (await shellFeaturesManager.GetAvailableFeaturesAsync())
            .Where(feature =>
                feature.IsTheme() &&
                feature.Id != FeatureNames.MediaTheme &&
                !feature.Extension.Manifest.Tags.Any(tag =>
                    tag.Equals("hidden", StringComparison.OrdinalIgnoreCase) ||
                    tag.Equals(ManifestConstants.AdminTag, StringComparison.OrdinalIgnoreCase)) &&
                enabledFeatures.Any(enabledFeature => enabledFeature.Id == feature.Id))
            .Select(feature => (feature.Id, feature.Name));
    }

    public async Task<MediaTemplate> GetMediaTemplateByShapeTypeAsync(string shapeType)
    {
        var templatePath = mediaFileStore.Combine(
            Paths.MediaThemeRootFolder,
            Paths.MediaThemeTemplatesFolder,
            shapeType + ".liquid");
        if (!await mediaFileStore.FileExistsAsync(templatePath)) return null;

        await using var templateFileStream = await mediaFileStore.GetFileStreamAsync(templatePath);
        using var reader = new StreamReader(templateFileStream);
        var content = await reader.ReadToEndAsync();

        return new MediaTemplate
        {
            TemplatePath = templatePath,
            ShapeType = shapeType,
            Content = content,
        };
    }

    private static void ThrowIfBaseThemeIdIsInvalid(string baseThemeId)
    {
        if (baseThemeId == FeatureNames.MediaTheme)
        {
            throw new ArgumentException("The Media Theme can't be its own base theme.", nameof(baseThemeId));
        }
    }
}
