using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Models;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using System.IO;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Services;

public class MediaTemplateService : IMediaTemplateService
{
    private readonly IMediaFileStore _mediaFileStore;

    public MediaTemplateService(IMediaFileStore mediaFileStore) => _mediaFileStore = mediaFileStore;

    public async Task<MediaTemplate> GetMediaTemplateByShapeTypeAsync(string shapeType)
    {
        var templatePath = _mediaFileStore.Combine(
            Paths.MediaThemeRootPath,
            Paths.MediaThemeTemplatesFolder,
            shapeType + ".liquid");
        if (!await _mediaFileStore.FileExistsAsync(templatePath)) return null;

        await using var templateFileStream = await _mediaFileStore.GetFileStreamAsync(templatePath);
        using var reader = new StreamReader(templateFileStream);
        var content = await reader.ReadToEndAsync();

        return new MediaTemplate
        {
            TemplatePath = templatePath,
            Content = content,
        };
    }
}
