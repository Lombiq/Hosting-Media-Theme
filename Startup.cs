using Lombiq.Hosting.MediaTheme.Drivers;
using Lombiq.Hosting.MediaTheme.Models;
using Lombiq.Hosting.MediaTheme.Navigation;
using Lombiq.Hosting.MediaTheme.Permissions;
using Lombiq.Hosting.MediaTheme.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace Lombiq.Hosting.MediaTheme;

public class Startup : StartupBase
{
    private readonly IShellConfiguration _shellConfiguration;

    public Startup(IShellConfiguration shellConfiguration) => _shellConfiguration = shellConfiguration;

    public override void ConfigureServices(IServiceCollection services)
    {
        services.Configure<MediaThemeDeploymentSettings>(
            _shellConfiguration.GetSection("Lombiq_Hosting_MediaTheme_Deployment"));
        services.AddTransient<IConfigureOptions<MediaThemeDeploymentSettings>, MediaThemeDeploymentSettingsConfiguration>();
        services.AddScoped<IDisplayDriver<ISite>, MediaThemeDeploymentSettingsDisplayDriver>();
        services.AddScoped<IPermissionProvider, MediaThemeDeploymentPermissions>();
        services.AddScoped<INavigationProvider, MediaThemeDeploymentSettingsAdminMenu>();
        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IConfigureOptions<LiquidViewOptions>, MediaThemeLiquidViewOptionsSetup>());
        services.AddSingleton<IMediaThemeStateStore, MediaThemeStateStore>();
        services.Decorate<IExtensionManager, ExtensionManagerDecorator>();
        services.AddScoped<IShapeBindingResolver, MediaTemplatesShapeBindingResolver>();
        services.AddScoped<IMediaTemplateService, MediaTemplateService>();
    }
}
