using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;
using System;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeStep : IRecipeStepHandler
{
    private readonly IMediaFileStore _mediaFileStore;
    private readonly IMediaThemeService _mediaThemeService;

    public MediaThemeStep(
        IMediaFileStore mediaFileStore,
        IMediaThemeService mediaThemeService)
    {
        _mediaFileStore = mediaFileStore;
        _mediaThemeService = mediaThemeService;
    }

    public async Task ExecuteAsync(RecipeExecutionContext context)
    {
        if (!string.Equals(context.Name, "mediatheme", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var model = context.Step.ToObject<MediaThemeStepModel>();

        await _mediaThemeService.UpdateBaseThemeAsync(model.BaseThemeId);

        if (model.ClearMediaThemeFolder)
        {
            await _mediaFileStore.TryDeleteDirectoryAsync(_mediaFileStore.Combine(Paths.MediaThemeRootFolder));
        }
    }

    private class MediaThemeStepModel
    {
        public string BaseThemeId { get; set; }
        public bool ClearMediaThemeFolder { get; set; }
    }
}
