using Microsoft.AspNetCore.Html;

namespace Lombiq.Hosting.MediaTheme.Models;

public class MediaTemplate
{
    public string TemplatePath { get; set; }
    public string ShapeType { get; set; }
    public string Content { get; set; }
}
