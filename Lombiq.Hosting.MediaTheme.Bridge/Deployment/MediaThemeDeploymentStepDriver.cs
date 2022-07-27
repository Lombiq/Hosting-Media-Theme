using Lombiq.Hosting.MediaTheme.Bridge.ViewModels;
using OrchardCore.Deployment;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Deployment;

public class MediaThemeDeploymentStepDriver : DisplayDriver<DeploymentStep, MediaThemeDeploymentStep>
{
    public override IDisplayResult Display(MediaThemeDeploymentStep step) =>
        Combine(
            View($"{nameof(MediaThemeDeploymentStep)}_Fields_Summary", step).Location("Summary", "Content"),
            View($"{nameof(MediaThemeDeploymentStep)}_Fields_Thumbnail", step).Location("Thumbnail", "Content")
        );

    public override IDisplayResult Edit(MediaThemeDeploymentStep step) =>
        Initialize<MediaThemeDeploymentStepViewModel>($"{nameof(MediaThemeDeploymentStep)}_Fields_Edit", model =>
        {
            model.ClearMediaThemeFolder = step.ClearMediaThemeFolder;
        }).Location("Content");

    public override async Task<IDisplayResult> UpdateAsync(MediaThemeDeploymentStep step, IUpdateModel updater)
    {
        await updater.TryUpdateModelAsync(step, Prefix, x => x.ClearMediaThemeFolder);

        return Edit(step);
    }
}
