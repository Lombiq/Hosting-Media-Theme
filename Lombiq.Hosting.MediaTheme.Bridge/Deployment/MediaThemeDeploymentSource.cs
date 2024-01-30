using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Lombiq.Hosting.MediaTheme.Bridge.Models;
using Lombiq.Hosting.MediaTheme.Bridge.Services;
using Newtonsoft.Json.Linq;
using OrchardCore.Deployment;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Deployment;

public class MediaThemeDeploymentSource : IDeploymentSource
{
    private readonly IMediaThemeStateStore _mediaThemeStateStore;

    public MediaThemeDeploymentSource(IMediaThemeStateStore mediaThemeStateStore) =>
        _mediaThemeStateStore = mediaThemeStateStore;

    public async Task ProcessDeploymentStepAsync(DeploymentStep step, DeploymentPlanResult result)
    {
        if (step is not MediaThemeDeploymentStep mediaThemeStep)
        {
            return;
        }

        var mediaThemeState = await _mediaThemeStateStore.GetMediaThemeStateAsync();

        result.Steps.Add(new JObject(
            new JProperty("name", RecipeStepIds.MediaTheme),
            new JProperty(nameof(MediaThemeDeploymentStep.ClearMediaThemeFolder), mediaThemeStep.ClearMediaThemeFolder),
            new JProperty(nameof(MediaThemeStateDocument.BaseThemeId), mediaThemeState.BaseThemeId)
        ));
    }
}
