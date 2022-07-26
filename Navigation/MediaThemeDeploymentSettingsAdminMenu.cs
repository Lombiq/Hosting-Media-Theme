using Lombiq.Hosting.MediaTheme.Constants;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;
using static Lombiq.Hosting.MediaTheme.Permissions.MediaThemeDeploymentPermissions;

namespace Lombiq.Hosting.MediaTheme.Navigation;

public class MediaThemeDeploymentSettingsAdminMenu : INavigationProvider
{
    private readonly IStringLocalizer T;

    public MediaThemeDeploymentSettingsAdminMenu(IStringLocalizer<MediaThemeDeploymentSettingsAdminMenu> stringLocalizer) => T = stringLocalizer;

    public Task BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase)) return Task.CompletedTask;

        builder
            .Add(T["Configuration"], configuration => configuration
                .Add(T["Media Theme"], T["Media Theme"].PrefixPosition(), entry => entry
                    .AddClass("mediatheme").Id("mediatheme")
                    .Action("Index", "Admin", new { area = FeatureNames.MediaTheme })
                    .Permission(ManageMediaTheme)
                    .LocalNav()));

        return Task.CompletedTask;
    }
}
