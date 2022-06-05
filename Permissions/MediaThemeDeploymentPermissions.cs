using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Permissions;

public class MediaThemeDeploymentPermissions : IPermissionProvider
{
    public static readonly Permission ManageMediaThemeDeploymentSettings = new(
        nameof(ManageMediaThemeDeploymentSettings),
        "Manage media theme deployment settings.");

    public static readonly Permission DeployMediaTheme = new(
        nameof(DeployMediaTheme),
        "Deploy media theme.");

    public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
        Task.FromResult(new[]
        {
            ManageMediaThemeDeploymentSettings,
            DeployMediaTheme,
        }
        .AsEnumerable());

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
        new[]
        {
            new PermissionStereotype
            {
                Name = "Anonymous",
                Permissions = new[] { DeployMediaTheme },
            },
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[] { ManageMediaThemeDeploymentSettings },
            },
        };
}
