using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Models;
using Lombiq.Hosting.MediaTheme.Permissions;
using Lombiq.Hosting.MediaTheme.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Drivers;

public class MediaThemeDeploymentSettingsDisplayDriver : SectionDisplayDriver<ISite, MediaThemeDeploymentSettings>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _hca;

    public MediaThemeDeploymentSettingsDisplayDriver(IAuthorizationService authorizationService, IHttpContextAccessor hca)
    {
        _authorizationService = authorizationService;
        _hca = hca;
    }

    public override async Task<IDisplayResult> EditAsync(MediaThemeDeploymentSettings section, BuildEditorContext context)
    {
        if (!await IsAuthorizedToManageDemoSettingsAsync())
        {
            return null;
        }

        return Initialize<MediaThemeDeploymentSettingsViewModel>(
            $"{nameof(MediaThemeDeploymentSettings)}_Edit",
            viewModel => viewModel.ApiKey = section.ApiKey)
        .Location("Content:1")
        .OnGroup(EditorGroupIds.MediaThemeDeploymentSettings);
    }

    public override async Task<IDisplayResult> UpdateAsync(MediaThemeDeploymentSettings section, BuildEditorContext context)
    {
        if (context.GroupId == EditorGroupIds.MediaThemeDeploymentSettings)
        {
            if (!await IsAuthorizedToManageDemoSettingsAsync())
            {
                return null;
            }

            var viewModel = new MediaThemeDeploymentSettingsViewModel();

            await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

            section.ApiKey = viewModel.ApiKey;
        }

        return await EditAsync(section, context);
    }

    private async Task<bool> IsAuthorizedToManageDemoSettingsAsync()
    {
        var user = _hca.HttpContext?.User;

        return user != null
            && await _authorizationService.AuthorizeAsync(user, MediaThemeDeploymentPermissions.ManageMediaThemeDeploymentSettings);
    }
}
