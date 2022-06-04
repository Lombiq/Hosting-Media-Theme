using Lombiq.Hosting.MediaTheme.Models;
using Microsoft.Extensions.Options;
using OrchardCore.Entities;
using OrchardCore.Settings;

namespace Lombiq.Hosting.MediaTheme.Services;

public class MediaThemeDeploymentSettingsConfiguration : IConfigureOptions<MediaThemeDeploymentSettings>
{
    private readonly ISiteService _siteService;

    public MediaThemeDeploymentSettingsConfiguration(ISiteService siteService) => _siteService = siteService;

    public void Configure(MediaThemeDeploymentSettings options)
    {
        if (!string.IsNullOrEmpty(options.ApiKey)) return;

        var settings = _siteService.GetSiteSettingsAsync()
            .GetAwaiter().GetResult()
            .As<MediaThemeDeploymentSettings>();

        options.ApiKey = settings.ApiKey;
    }
}
