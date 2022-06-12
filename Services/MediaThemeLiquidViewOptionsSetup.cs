using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCore.Media;

namespace Lombiq.Hosting.MediaTheme.Services;

public class MediaThemeLiquidViewOptionsSetup : IConfigureOptions<LiquidViewOptions>
{
    private readonly IMediaFileStore _mediaFileStore;

    public MediaThemeLiquidViewOptionsSetup(IMediaFileStore mediaFileStore) =>
        _mediaFileStore = mediaFileStore;

    public void Configure(LiquidViewOptions options) =>
        options.FileProviders.Insert(0, new MediaThemeFileProvider(_mediaFileStore));
}
