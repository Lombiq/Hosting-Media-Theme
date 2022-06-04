using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Permissions;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Navigation;

public class MediaThemeDeploymentSettingsAdminMenu : INavigationProvider
{
    private readonly IStringLocalizer T;

    public MediaThemeDeploymentSettingsAdminMenu(IStringLocalizer<MediaThemeDeploymentSettingsAdminMenu> stringLocalizer) => T = stringLocalizer;

    public Task BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase)) return Task.CompletedTask;

        builder.Add(T["Configuration"], configuration => configuration
            .Add(T["Settings"], settings => settings
                .Add(T["Media Theme Deployment"], T["Media Theme Deployment"], mediaThemeDeployment => mediaThemeDeployment
                    .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = EditorGroupIds.MediaThemeDeploymentSettings })
                    .Permission(MediaThemeDeploymentSettingsPermissions.ManageMediaThemeDeploymentSettings)
                    .LocalNav()
                )));

        return Task.CompletedTask;
    }
}
