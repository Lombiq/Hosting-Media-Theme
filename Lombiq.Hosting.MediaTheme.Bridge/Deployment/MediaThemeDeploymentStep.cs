using OrchardCore.Deployment;

namespace Lombiq.Hosting.MediaTheme.Bridge.Deployment;

public class MediaThemeDeploymentStep : DeploymentStep
{
    public MediaThemeDeploymentStep() => Name = "MediaTheme";

    public bool ClearMediaThemeFolder { get; set; }
}
