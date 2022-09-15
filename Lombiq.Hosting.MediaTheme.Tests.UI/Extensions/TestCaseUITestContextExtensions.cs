using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    public static async Task TestMediaThemeTemplateRenderingBehaviorAsync(this UITestContext context)
    {
        await context.SignInDirectlyAsync();
        await context.ExecuteMediaThemeSampleRecipeDirectlyAsync();
        await context.GoToMediaThemeTestContentPageAsync();

        context.Exists(By.XPath("//p[contains(., 'This is an example template hosted in Media Theme.')]"));
    }
}
