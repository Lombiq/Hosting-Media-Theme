using Lombiq.Hosting.MediaTheme.Middlewares;
using Lombiq.Hosting.MediaTheme.Navigation;
using Lombiq.Hosting.MediaTheme.Permissions;
using Lombiq.Hosting.MediaTheme.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement;
using OrchardCore.Environment.Extensions;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Recipes;
using OrchardCore.Recipes.Services;
using OrchardCore.Security.Permissions;
using System;

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
        services.AddScoped<IMediaThemeService, MediaThemeService>();
        services.AddRecipeExecutionStep<MediaThemeStep>();
    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        app.UseMiddleware<MediaThemeAssetUrlRedirectMiddleware>();
}
