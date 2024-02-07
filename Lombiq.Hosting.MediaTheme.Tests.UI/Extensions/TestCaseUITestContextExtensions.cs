using Atata;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    public static async Task TestMediaThemeDeployedBehaviorAsync(this UITestContext context)
    {
        await context.ExecuteMediaThemeSampleRecipeDirectlyAsync();
        await context.GoToMediaThemeTestContentPageAsync();
        AssertElements(context, "v");
    }

    public static async Task TestMediaThemeLocalBehaviorAsync(this UITestContext context)
    {
        await context.SetThemeDirectlyAsync("Lombiq.Hosting.MediaTheme.Tests.Theme");
        await context.GoToHomePageAsync(onlyIfNotAlreadyThere: false);
        AssertElements(context, "mediatheme");
    }

    private static void AssertElements(UITestContext context, string cacheBustingParameterName)
    {
        context.Exists(By.XPath($"//head//link[contains(@href, '/mediatheme/example.css?{cacheBustingParameterName}=')]").Hidden());
        context.Exists(By.XPath("//p[contains(., 'This is an example template hosted in Media Theme.')]"));
        context.Exists(By.XPath("//img[contains(@src, '/mediatheme/example.png')]"));
        context.Exists(By.XPath($"//img[contains(@src, '/mediatheme/example2.png?{cacheBustingParameterName}=')]"));
    }
}
