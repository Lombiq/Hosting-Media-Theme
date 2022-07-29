using Lombiq.Hosting.MediaTheme.Bridge.Services;
using Lombiq.Hosting.MediaTheme.Bridge.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.ModelBinding;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.Hosting.MediaTheme.Bridge.Permissions.MediaThemeDeploymentPermissions;

namespace Lombiq.Hosting.MediaTheme.Bridge.Controllers;

public class AdminController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IMediaThemeStateStore _mediaThemeStateStore;
    private readonly IUpdateModelAccessor _updateModelAccessor;
    private readonly IStringLocalizer<AdminController> T;
    private readonly IMediaThemeManager _mediaThemeManager;

    public AdminController(
        IAuthorizationService authorizationService,
        IMediaThemeStateStore mediaThemeStateStore,
        IUpdateModelAccessor updateModelAccessor,
        IStringLocalizer<AdminController> stringLocalizer,
        IMediaThemeManager mediaThemeManager)
    {
        _authorizationService = authorizationService;
        _mediaThemeStateStore = mediaThemeStateStore;
        _updateModelAccessor = updateModelAccessor;
        T = stringLocalizer;
        _mediaThemeManager = mediaThemeManager;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        if (!await IsAuthorizedToManageMediaThemeAsync()) return NotFound();

        var baseThemeId = (await _mediaThemeStateStore.GetMediaThemeStateAsync())?.BaseThemeId;
        var availableThemes = await _mediaThemeManager.GetAvailableBaseThemesAsync();

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

        var availableThemes = (await _mediaThemeManager.GetAvailableBaseThemesAsync()).ToList();
        viewModel.AvailableBaseThemes = availableThemes;
        if (!string.IsNullOrEmpty(viewModel.BaseThemeId) &&
            availableThemes.All(theme => theme.Id != viewModel.BaseThemeId))
        {
            _updateModelAccessor.ModelUpdater.ModelState.AddModelError(
                nameof(viewModel.BaseThemeId),
                T["The selected theme is not available."]);

            return View(viewModel);
        }

        await _mediaThemeManager.UpdateBaseThemeAsync(viewModel.BaseThemeId);

        return RedirectToAction(nameof(Index));
    }

    private Task<bool> IsAuthorizedToManageMediaThemeAsync() =>
        _authorizationService.AuthorizeAsync(User, ManageMediaTheme);
}
