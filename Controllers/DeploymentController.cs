using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Controllers;

[Route(Routes.Deployment)]
[ApiController]
[Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
public class DeploymentController : Controller
{
    private readonly IMediaFileStore _mediaFileStore;
    private readonly MediaThemeDeploymentSettings _settings;

    public DeploymentController(
        IOptionsSnapshot<MediaThemeDeploymentSettings> mediaThemeDeploymentOptions,
        IMediaFileStore mediaFileStore)
    {
        _mediaFileStore = mediaFileStore;
        _settings = mediaThemeDeploymentOptions.Value;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] MediaThemeDeploymentPackageRequest request)
    {
        if (string.IsNullOrEmpty(_settings.ApiKey) || _settings.ApiKey != request.ApiKey)
        {
            return BadRequest("The API key is incorrect.");
        }

        var tempArchiveName = Path.GetRandomFileName() + ".zip";
        var tempArchiveFolder = PathExtensions.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            await using (var tempCreateFileStream = System.IO.File.Create(tempArchiveName))
            {
                await request.Content.CopyToAsync(tempCreateFileStream, HttpContext.RequestAborted);
            }

            ZipFile.ExtractToDirectory(tempArchiveName, tempArchiveFolder);

            await _mediaFileStore.TryDeleteDirectoryAsync(Paths.MediaThemeRootPath);
            await _mediaFileStore.TryCreateDirectoryAsync(Paths.MediaThemeRootPath);

            var tempTemplateFolder = Path.Combine(tempArchiveFolder, "Templates");
            if (Directory.Exists(tempTemplateFolder))
            {
                var mediaTemplateFolder = _mediaFileStore.Combine(Paths.MediaThemeRootPath, Paths.MediaThemeTemplatesFolder);
                await _mediaFileStore.TryCreateDirectoryAsync(mediaTemplateFolder);
                foreach (var liquidFile in Directory.GetFiles(tempTemplateFolder, "*.liquid"))
                {
                    var mediaTemplateFile = _mediaFileStore.Combine(mediaTemplateFolder, Path.GetFileName(liquidFile));
                    await using var fileStream = System.IO.File.OpenRead(liquidFile);
                    await _mediaFileStore.CreateFileFromStreamAsync(mediaTemplateFile, fileStream);
                }
            }
        }
        finally
        {
            if (System.IO.File.Exists(tempArchiveName))
            {
                System.IO.File.Delete(tempArchiveName);
            }

            if (Directory.Exists(tempArchiveFolder))
            {
                Directory.Delete(tempArchiveFolder, recursive: true);
            }
        }

        return Ok();
    }
}
