using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Lombiq.Hosting.MediaTheme.Bridge.Services;
using Lombiq.Hosting.MediaTheme.Bridge.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Themes.Services;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.Hosting.MediaTheme.Bridge.Permissions.MediaThemeDeploymentPermissions;

namespace Lombiq.Hosting.MediaTheme.Bridge.Controllers;

public class AdminController(
    IOrchardServices<AdminController> orchardServices,
    IMediaThemeStateStore mediaThemeStateStore,
    IUpdateModelAccessor updateModelAccessor,
    IMediaThemeManager mediaThemeManager,
    ISiteThemeService siteThemeService,
    IMediaThemeCachingService mediaThemeCachingService,
    INotifier notifier) : Controller
{
    private readonly IAuthorizationService _authorizationService = orchardServices.AuthorizationService.Value;
    private readonly IStringLocalizer<AdminController> T = orchardServices.StringLocalizer.Value;
    private readonly IHtmlLocalizer<AdminController> H = orchardServices.HtmlLocalizer.Value;

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        var baseThemeId = (await mediaThemeStateStore.GetMediaThemeStateAsync())?.BaseThemeId;
        var availableThemes = await mediaThemeManager.GetAvailableBaseThemesAsync();

        return View(await SetFlagIfMediaThemeIsActiveAsync(new MediaThemeSettingsViewModel
        {
            AvailableBaseThemes = availableThemes,
            BaseThemeId = baseThemeId,
        }));
    }

    [HttpPost, ActionName(nameof(Index))]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> IndexPost(MediaThemeSettingsViewModel viewModel)
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        var state = await mediaThemeStateStore.LoadMediaThemeStateAsync();
        if (state.BaseThemeId == viewModel.BaseThemeId) RedirectToAction(nameof(Index));

        var availableThemes = (await mediaThemeManager.GetAvailableBaseThemesAsync()).ToList();
        viewModel.AvailableBaseThemes = availableThemes;
        if (!string.IsNullOrEmpty(viewModel.BaseThemeId) &&
            availableThemes.TrueForAll(theme => theme.Id != viewModel.BaseThemeId))
        {
            updateModelAccessor.ModelUpdater.ModelState.AddModelError(
                nameof(viewModel.BaseThemeId),
                T["The selected theme is not available."]);

            return View(await SetFlagIfMediaThemeIsActiveAsync(viewModel));
        }

        await mediaThemeManager.UpdateBaseThemeAsync(viewModel.BaseThemeId);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteMediaThemeTemplateCache()
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        await mediaThemeCachingService.InvalidateCachedMediaThemeTemplatesAsync();
        await notifier.SuccessAsync(H["Media Theme template cache was invalidated successfully!"]);
        return RedirectToAction(nameof(Index));
    }

    private Task<bool> IsAuthorizedToManageMediaThemeAsync() =>
        _authorizationService.AuthorizeAsync(User, ManageMediaTheme);

    private async Task<MediaThemeSettingsViewModel> SetFlagIfMediaThemeIsActiveAsync(MediaThemeSettingsViewModel viewModel)
    {
        viewModel.IsMediaThemeActive = (await siteThemeService.GetSiteThemeAsync())?.Id == FeatureNames.MediaTheme;
        return viewModel;
    }
}
