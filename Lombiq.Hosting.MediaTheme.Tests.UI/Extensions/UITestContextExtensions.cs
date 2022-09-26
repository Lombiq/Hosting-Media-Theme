using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Tests.UI.Extensions;

public static class UITestContextExtensions
{
    public static Task ExecuteMediaThemeSampleRecipeDirectlyAsync(this UITestContext context) =>
        context.ExecuteRecipeDirectlyAsync("Lombiq.Hosting.MediaTheme.Samples");

    public static Task GoToMediaThemeTestContentPageAsync(this UITestContext context) =>
        context.GoToRelativeUrlAsync("/Contents/ContentItems/mediathemetest000000000000");
}
