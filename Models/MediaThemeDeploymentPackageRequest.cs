using Microsoft.AspNetCore.Http;

namespace Lombiq.Hosting.MediaTheme.Models;

public class MediaThemeDeploymentPackageRequest
{
    public string ApiKey { get; set; }
    public IFormFile Content { get; set; }
}
