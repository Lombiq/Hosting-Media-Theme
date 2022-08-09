namespace Lombiq.Hosting.MediaTheme.Deployer.Constants;
public static class PathConstants
{
    public const string MediaThemeRootDirectory = "_MediaTheme";
    public const string MediaThemeDeploymentDirectory = "MediaThemeDeployment_";
    public const string MediaThemeWwwRootDirectory = "wwwroot";
    public const string MediaThemeViewsDirectory = "Views";
    public const string MediaThemeTemplatesFolder = "Templates";
    public const string MediaThemeAssetsDriectory = "Assets";

    public const string MediaThemeAssetsWebPath = MediaThemeRootDirectory + "/" + MediaThemeAssetsDriectory;
    public const string MediaThemeAssetsCopyDirectoryPath = "\\" + MediaThemeRootDirectory + "\\" + MediaThemeAssetsDriectory;

    public const string MediaThemeTemplatesWebPath = MediaThemeRootDirectory + "/" + MediaThemeTemplatesFolder;
    public const string MediaThemeTemplatesCopyDirectoryPath = "\\" + MediaThemeRootDirectory + "\\" + MediaThemeTemplatesFolder;

    public const string RecipeFile = "\\Recipe.json";
    public const string LiquidFileExtension = ".liquid";
}
