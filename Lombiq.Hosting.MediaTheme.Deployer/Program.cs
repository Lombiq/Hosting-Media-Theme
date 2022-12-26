using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO.Compression;
using static Lombiq.Hosting.MediaTheme.Deployer.Constants.PathConstants;
using static System.Console;

namespace Lombiq.Hosting.MediaTheme.Deployer;

public class CommandLineOptions
{
    [Option('p', "path", Required = true, HelpText = "Path of your theme.")]
    public string? PathOfTheTheme { get; set; }

    [Option('i', "base-id", Required = true, HelpText = "Default theme ID.")]
    public string? BaseThemeId { get; set; }

    [Option('c', "clear", Required = true, HelpText = "Whether or not to clear media hosting folder.")]
    public bool ClearMediaHostingFolder { get; set; }

    [Option('d', "deployment-path", Required = false, HelpText = "The path where you want the deployment package copied.")]
    public string? DeploymentPackagePath { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Globalization",
    "CA1303:Do not pass literals as localized parameters",
    Justification = "It's a console application, it doesn't need localization.")]
internal static class Program
{
    public static void Main(string[] args) =>
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(options => RunOptions(options))
            .WithNotParsed(HandleParseError);

    private static void HandleParseError(IEnumerable<Error> errors)
    {
        if (errors.Any())
        {
            foreach (var error in errors)
            {
                WriteLine(error);
            }
        }
        else
        {
            WriteLine("Unknown error.");
        }
    }

    private static void RunOptions(CommandLineOptions values)
    {
        // Creating directory for the deployment.
        var newDirectoryPath = CreateNewDirectoryPath(values);

        try
        {
            // Determine whether the directory exists.
            if (Directory.Exists(newDirectoryPath))
            {
                WriteLine("That directory already exists.");
                return;
            }

            // Try to create the directory.
            Directory.CreateDirectory(newDirectoryPath);

            WriteLine("The directory was created successfully. {0}", newDirectoryPath);
        }
        catch (Exception exception)
        {
            WriteLine("The directory creation failed: {0}", exception.ToString());
            return;
        }

        var pathToTheme = values.PathOfTheTheme;

        // Creating media theme step.
        dynamic mediaThemeStep = new JObject();
        mediaThemeStep.name = "mediatheme";
        mediaThemeStep.BaseThemeId = values.BaseThemeId;
        mediaThemeStep.ClearMediaThemeFolder = values.ClearMediaHostingFolder;

        // Creating media step.
        var files = new JArray();

        // Getting assets.
        var pathToAssets = Path.Join(pathToTheme, LocalThemeWwwRootDirectory);

        var allAssetsPaths = Directory.EnumerateFiles(pathToAssets, "*", SearchOption.AllDirectories);

        foreach (var assetPath in allAssetsPaths)
        {
            dynamic assetJObject = new JObject();
            assetJObject.SourcePath = Path.Join(
                MediaThemeAssetsWebPath,
                assetPath[pathToAssets.Length..].Replace("\\", "/"));
            assetJObject.TargetPath = assetJObject.SourcePath;

            files.Add(assetJObject);
        }

        // Copying assets to deployment directory.
        CopyDirectory(
            pathToAssets,
            Path.Join(newDirectoryPath, MediaThemeAssetsCopyDirectoryPath),
            areLiquidFiles: false);

        // Getting templates.
        var pathToTemplates = Path.Join(pathToTheme, LocalThemeViewsDirectory);

        var allTemplatesPaths = Directory
            .EnumerateFiles(pathToTemplates, "*" + LiquidFileExtension, SearchOption.TopDirectoryOnly);

        foreach (var templatePath in allTemplatesPaths)
        {
            dynamic templateJObject = new JObject();
            templateJObject.SourcePath = Path.Join(
                MediaThemeTemplatesWebPath,
                templatePath[pathToTemplates.Length..].Replace("\\", "/"));
            templateJObject.TargetPath = templateJObject.SourcePath;

            files.Add(templateJObject);
        }

        // Copying templates to deployment directory.
        CopyDirectory(
            pathToTemplates,
            Path.Join(newDirectoryPath, MediaThemeTemplatesCopyDirectoryPath),
            areLiquidFiles: true,
            recursive: false);

        dynamic mediaStep = new JObject();
        mediaStep.name = "media";
        mediaStep.Files = files;

        CreateRecipeAndWriteIt(mediaThemeStep, mediaStep, newDirectoryPath);

        // Zipping the directory.
        var zippedDirectoryPath = newDirectoryPath + ".zip";
        ZipFile.CreateFromDirectory(newDirectoryPath, zippedDirectoryPath);

        // Getting rid of the original directory.
        Directory.Delete(newDirectoryPath, recursive: true);

        WriteLine("{0} was created successfully. ", zippedDirectoryPath);
    }

    private static void CopyDirectory(
        string sourceDir,
        string destinationDirectory,
        bool areLiquidFiles,
        bool recursive = true)
    {
        // Get information about the source directory.
        var directory = new DirectoryInfo(sourceDir);

        // Check if the source directory exists.
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {directory.FullName}");

        // Cache directories before we start copying.
        var directories = directory.GetDirectories();

        // Create the destination directory.
        Directory.CreateDirectory(destinationDirectory);

        // Get the files in the source directory and copy to the destination directory.
        foreach (var file in directory.GetFiles())
        {
            var fileName = file.Name;

            if (areLiquidFiles)
            {
                var fileNameWithoutExtension = fileName[..fileName.LastIndexOf(
                    LiquidFileExtension,
                    StringComparison.InvariantCulture)];

                fileName = fileNameWithoutExtension.Replace('.', '_').Replace("_", "__") + LiquidFileExtension;
            }

            string targetFilePath = Path.Combine(destinationDirectory, fileName);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method.
        if (recursive)
        {
            foreach (var subDirectory in directories)
            {
                string newDestinationDir = Path.Combine(destinationDirectory, subDirectory.Name);
                CopyDirectory(subDirectory.FullName, newDestinationDir, areLiquidFiles);
            }
        }
    }

    private static string CreateNewDirectoryPath(CommandLineOptions values)
    {
        var deploymentPathCommandLineValue = values.DeploymentPackagePath;
        var deploymentPath = !string.IsNullOrEmpty(deploymentPathCommandLineValue)
            ? deploymentPathCommandLineValue
            : Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());

        return Path.Join(deploymentPath, MediaThemeDeploymentDirectory)
            + DateTime.Now.ToString("ddMMMyyyyHHmmss", CultureInfo.CurrentCulture); // #spell-check-ignore-line
    }

    private static void CreateRecipeAndWriteIt(JObject mediaThemeStep, JObject mediaStep, string newDirectoryPath)
    {
        // Creating the recipe itself.
        dynamic recipe = new JObject();
        recipe.name = "MediaTheme";
        recipe.displayName = "Media Theme";
        recipe.description = "A recipe created with the media-theme-deployment tool.";
        recipe.author = string.Empty;
        recipe.website = string.Empty;
        recipe.version = string.Empty;
        recipe.issetuprecipe = false;
        recipe.categories = new JArray();
        recipe.tags = new JArray();
        recipe.steps = new JArray(mediaThemeStep, mediaStep);

        // Creating JSON file.
        using var file = File.CreateText(Path.Join(newDirectoryPath + RecipeFile));
        using var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };
        recipe.WriteTo(writer);

        file.Close();
    }
}
