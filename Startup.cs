using Lombiq.Hosting.MediaTheme.Navigation;
using Lombiq.Hosting.MediaTheme.Permissions;
using Lombiq.Hosting.MediaTheme.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement;
using OrchardCore.Environment.Extensions;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;

namespace Lombiq.Hosting.MediaTheme;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IPermissionProvider, MediaThemeDeploymentPermissions>();
        services.AddScoped<INavigationProvider, MediaThemeDeploymentSettingsAdminMenu>();
        services.AddSingleton<IMediaThemeStateStore, MediaThemeStateStore>();
        services.Decorate<IExtensionManager, ExtensionManagerDecorator>();
        services.AddScoped<IShapeBindingResolver, MediaTemplatesShapeBindingResolver>();
        services.AddScoped<IMediaTemplateService, MediaTemplateService>();
    }
}
