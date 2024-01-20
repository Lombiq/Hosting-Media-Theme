using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Security;
using System;
using System.Threading.Tasks;
using static Lombiq.Hosting.MediaTheme.Bridge.Permissions.MediaThemeDeploymentPermissions;
using MediaPermissions = OrchardCore.Media.Permissions;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class ManageMediaThemeFolderAuthorizationHandler(IServiceProvider serviceProvider) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (requirement.Permission.Name != MediaPermissions.ManageMediaFolder.Name) return;

        var path = context.Resource as string;
        if (string.IsNullOrEmpty(path) || !path.StartsWith(Paths.MediaThemeRootFolder, StringComparison.Ordinal))
        {
            return;
        }

        var authorizationService = serviceProvider.GetService<IAuthorizationService>();

        if (!await authorizationService.AuthorizeAsync(context.User, ManageMediaTheme))
        {
            context.Fail();
        }
    }
}
