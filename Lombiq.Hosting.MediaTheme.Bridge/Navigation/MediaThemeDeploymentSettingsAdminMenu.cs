using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using static Lombiq.Hosting.MediaTheme.Bridge.Permissions.MediaThemeDeploymentPermissions;

namespace Lombiq.Hosting.MediaTheme.Bridge.Navigation;

public sealed class MediaThemeDeploymentSettingsAdminMenu : AdminMenuNavigationProviderBase
{
    public MediaThemeDeploymentSettingsAdminMenu(
        IHttpContextAccessor hca,
        IStringLocalizer<MediaThemeDeploymentSettingsAdminMenu> stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }

    protected override void Build(NavigationBuilder builder) =>
    builder.Add(T["Configuration"], config => config
        .Add(T["Media Theme"], T["Media Theme"].PrefixPosition(), entry => entry
            .AddClass("mediatheme").Id("mediatheme")
            .Action("Index", "Admin", new { area = "Lombiq.Hosting.MediaTheme.Bridge" })
            .Permission(ManageMediaTheme)
            .LocalNav()));
}
