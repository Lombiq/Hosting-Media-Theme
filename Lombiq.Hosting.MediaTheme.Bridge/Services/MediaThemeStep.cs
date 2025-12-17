using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Media;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public sealed class MediaThemeStep : NamedRecipeStepHandler
{
    private readonly IMediaFileStore _mediaFileStore;
    private readonly IMediaThemeManager _mediaThemeManager;
    private readonly IServiceProvider _serviceProvider;

    public MediaThemeStep(
        IMediaFileStore mediaFileStore,
        IMediaThemeManager mediaThemeManager,
        IServiceProvider serviceProvider)
        : base(RecipeStepIds.MediaTheme)
    {
        _mediaFileStore = mediaFileStore;
        _mediaThemeManager = mediaThemeManager;
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleAsync(RecipeExecutionContext context)
    {
        var model = context.Step.Deserialize<MediaThemeStepModel>();

        await _mediaThemeManager.UpdateBaseThemeAsync(model.BaseThemeId);

        if (model.ClearMediaThemeFolder)
        {
            await _mediaFileStore.TryDeleteDirectoryAsync(Paths.MediaThemeRootFolder);
        }

        // If a remote storage implementation is used, its cache needs to be purged too to make sure all files are
        // fresh. This is easiest to do by removing the whole folder even if ClearMediaThemeFolder wasn't set.
        if (_serviceProvider.GetService<IMediaFileStoreCache>() is { } mediaFileStoreCache)
        {
            await mediaFileStoreCache.TryDeleteDirectoryAsync(Paths.MediaThemeRootFolder);
        }
    }

    private sealed class MediaThemeStepModel
    {
        public string BaseThemeId { get; set; } = string.Empty;
        public bool ClearMediaThemeFolder { get; set; } = true;
    }
}
