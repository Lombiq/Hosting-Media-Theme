using Lombiq.Hosting.MediaTheme.Bridge.ViewModels;
using OrchardCore.Deployment;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Deployment;

public class MediaThemeDeploymentStepDriver : DisplayDriver<DeploymentStep, MediaThemeDeploymentStep>
{
    public override IDisplayResult Display(MediaThemeDeploymentStep model) =>
        Combine(
            View($"{nameof(MediaThemeDeploymentStep)}_Fields_Summary", model).Location("Summary", "Content"),
            View($"{nameof(MediaThemeDeploymentStep)}_Fields_Thumbnail", model).Location("Thumbnail", "Content")
        );

    public override IDisplayResult Edit(MediaThemeDeploymentStep model) =>
        Initialize<MediaThemeDeploymentStepViewModel>($"{nameof(MediaThemeDeploymentStep)}_Fields_Edit", viewModel =>
        {
            viewModel.ClearMediaThemeFolder = model.ClearMediaThemeFolder;
        }).Location("Content");

    public override async Task<IDisplayResult> UpdateAsync(MediaThemeDeploymentStep model, IUpdateModel updater)
    {
        await updater.TryUpdateModelAsync(model, Prefix, x => x.ClearMediaThemeFolder);

        return Edit(model);
    }
}
