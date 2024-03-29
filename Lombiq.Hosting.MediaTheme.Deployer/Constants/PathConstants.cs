namespace Lombiq.Hosting.MediaTheme.Deployer.Constants;

public static class PathConstants
{
    public const string MediaThemeRootDirectory = "_MediaTheme";
    public const string MediaThemeDeploymentDirectory = "MediaThemeDeployment_";
    public const string LocalThemeWwwRootDirectory = "wwwroot";
    public const string LocalThemeViewsDirectory = "Views";
    public const string MediaThemeTemplatesDirectory = "Templates";
    public const string MediaThemeAssetsDirectory = "Assets";

    public const string RecipeFile = "Recipe.json";
    public const string LiquidFileExtension = ".liquid";

    public static readonly string MediaThemeAssetsCopyDirectoryPath =
        Path.Combine(MediaThemeRootDirectory, MediaThemeAssetsDirectory);

    public static readonly string MediaThemeTemplatesCopyDirectoryPath =
        Path.Combine(MediaThemeRootDirectory, MediaThemeTemplatesDirectory);
}
