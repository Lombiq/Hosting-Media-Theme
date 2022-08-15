namespace Lombiq.Hosting.MediaTheme.Deployer.Constants;
public static class PathConstants
{
    public const string MediaThemeRootDirectory = "_MediaTheme";
    public const string MediaThemeDeploymentDirectory = "MediaThemeDeployment_";
    public const string LocalThemeWwwRootDirectory = "wwwroot";
    public const string LocalThemeViewsDirectory = "Views";
    public const string MediaThemeTemplatesDirectory = "Templates";
    public const string MediaThemeAssetsDriectory = "Assets";

    public const string MediaThemeAssetsWebPath = MediaThemeRootDirectory + "/" + MediaThemeAssetsDriectory;
    public const string MediaThemeAssetsCopyDirectoryPath = "\\" + MediaThemeRootDirectory + "\\" + MediaThemeAssetsDriectory;

    public const string MediaThemeTemplatesWebPath = MediaThemeRootDirectory + "/" + MediaThemeTemplatesDirectory;
    public const string MediaThemeTemplatesCopyDirectoryPath = "\\" + MediaThemeRootDirectory + "\\" + MediaThemeTemplatesDirectory;

    public const string RecipeFile = "\\Recipe.json";
    public const string LiquidFileExtension = ".liquid";
}
