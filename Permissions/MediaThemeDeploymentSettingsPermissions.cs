using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Permissions;

public class MediaThemeDeploymentSettingsPermissions : IPermissionProvider
{
    public static readonly Permission ManageMediaThemeDeploymentSettings = new(
        nameof(ManageMediaThemeDeploymentSettings),
        "Manage media theme deployment settings.");

    public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
        Task.FromResult(new[]
        {
            ManageMediaThemeDeploymentSettings,
        }
        .AsEnumerable());

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
        new[]
        {
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[] { ManageMediaThemeDeploymentSettings },
            },
        };
}
