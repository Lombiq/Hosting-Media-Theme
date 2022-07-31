using OrchardCore.Data.Documents;

namespace Lombiq.Hosting.MediaTheme.Bridge.Models;

public class MediaThemeStateDocument : Document
{
    public string BaseThemeId { get; set; }
}
