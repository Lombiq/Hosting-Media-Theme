using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeStep(
    IMediaFileStore mediaFileStore,
    IMediaThemeManager mediaThemeManager) : IRecipeStepHandler
{
    public async Task ExecuteAsync(RecipeExecutionContext context)
    {
        if (!string.Equals(context.Name, RecipeStepIds.MediaTheme, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var model = context.Step.ToObject<MediaThemeStepModel>();

        await mediaThemeManager.UpdateBaseThemeAsync(model.BaseThemeId);

        if (model.ClearMediaThemeFolder)
        {
            await mediaFileStore.TryDeleteDirectoryAsync(mediaFileStore.Combine(Paths.MediaThemeRootFolder));
        }
    }

    private sealed class MediaThemeStepModel
    {
        public string BaseThemeId { get; set; } = string.Empty;
        public bool ClearMediaThemeFolder { get; set; } = true;
    }
}
