using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Services;
using Lombiq.Hosting.MediaTheme.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules.Manifest;
using OrchardCore.Themes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.Hosting.MediaTheme.Permissions.MediaThemeDeploymentPermissions;

namespace Lombiq.Hosting.MediaTheme.Controllers;

public class AdminController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly INotifier _notifier;
    private readonly IHtmlLocalizer<AdminController> H;
    private readonly ISiteThemeService _siteThemeService;
    private readonly IShellFeaturesManager _shellFeaturesManager;
    private readonly IMediaThemeStateStore _mediaThemeStateStore;
    private readonly IUpdateModelAccessor _updateModelAccessor;
    private readonly IStringLocalizer<AdminController> T;
    private readonly IMemoryCache _memoryCache;

#pragma warning disable S107
    public AdminController(
        IAuthorizationService authorizationService,
        INotifier notifier,
        IHtmlLocalizer<AdminController> htmlLocalizer,
        ISiteThemeService siteThemeService,
        IShellFeaturesManager shellFeaturesManager,
        IMediaThemeStateStore mediaThemeStateStore,
        IUpdateModelAccessor updateModelAccessor,
        IStringLocalizer<AdminController> stringLocalizer,
        IMemoryCache memoryCache)
#pragma warning restore S107
    {
        _authorizationService = authorizationService;
        _notifier = notifier;
        H = htmlLocalizer;
        _siteThemeService = siteThemeService;
        _shellFeaturesManager = shellFeaturesManager;
        _mediaThemeStateStore = mediaThemeStateStore;
        _updateModelAccessor = updateModelAccessor;
        T = stringLocalizer;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        var activeTheme = await _siteThemeService.GetSiteThemeAsync();
        var baseThemeId = (await _mediaThemeStateStore.GetMediaThemeStateAsync())?.BaseThemeId;
        var availableThemes = await GetAvailableThemesAsync();

        if (activeTheme.Id != FeatureNames.MediaTheme)
        {
            await _notifier.WarningAsync(H["The Media Theme feature is not active."]);
        }

        return View(new MediaThemeSettingsViewModel
        {
            AvailableBaseThemes = availableThemes,
            BaseThemeId = baseThemeId,
        });
    }

    [HttpPost, ActionName(nameof(Index))]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> IndexPost(MediaThemeSettingsViewModel viewModel)
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        var state = await _mediaThemeStateStore.LoadMediaThemeStateAsync();
        if (state.BaseThemeId == viewModel.BaseThemeId) RedirectToAction(nameof(Index));

        var availableThemes = (await GetAvailableThemesAsync()).ToList();
        viewModel.AvailableBaseThemes = availableThemes;
        if (!string.IsNullOrEmpty(viewModel.BaseThemeId) &&
            availableThemes.All(theme => theme.Id != viewModel.BaseThemeId))
        {
            _updateModelAccessor.ModelUpdater.ModelState.AddModelError(
                nameof(viewModel.BaseThemeId),
                T["The selected theme is not available."]);

            return View(viewModel);
        }

        state.BaseThemeId = viewModel.BaseThemeId;
        await _mediaThemeStateStore.SaveMediaThemeStateAsync(state);

        await InvalidateCacheAndNotifyAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> InvalidateCache()
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        await InvalidateCacheAndNotifyAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task InvalidateCacheAndNotifyAsync()
    {
        _memoryCache.Remove($"ShapeTable:{FeatureNames.MediaTheme}");

        await _notifier.InformationAsync(
            H["The media theme shape table cache has been invalidated."]);
    }

    private Task<bool> IsAuthorizedToManageMediaThemeAsync() =>
        _authorizationService.AuthorizeAsync(User, ManageMediaTheme);

    private async Task<IEnumerable<(string Id, string Name)>> GetAvailableThemesAsync()
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
