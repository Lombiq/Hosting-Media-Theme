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
    [Option('p', "path", Required = true, HelpText = "Path of your theme project.")]
    public string? PathOfTheTheme { get; set; }

    [Option('i', "base-id", Required = false, HelpText = "ID of the base theme, if any.")]
    public string? BaseThemeId { get; set; }

    [Option('c', "clear", Required = false, HelpText = "Whether or not to clear the Media Theme media folder of all files before deployment.")]
    public bool ClearMediaHostingFolder { get; set; } = true;

    [Option('d', "deployment-path", Required = false, HelpText = "The path where you want the deployment package to be written to.")]
    public string? DeploymentPackagePath { get; set; }

    [Option(
        'u',
        "remote-deployment-url",
        Required = false,
        HelpText = "The URL to use for Remote Deployment, as indicated on the Orchard Core admin.")]
    public string? RemoteDeploymentUrl { get; set; }

    [Option(
        'n',
        "remote-deployment-client-name",
        Required = false,
        HelpText = "The \"Client Name\" part of the Remote Deployment client's credentials.")]
    public string? RemoteDeploymentClientName { get; set; }

    [Option(
        'k',
        "remote-deployment-client-api-key",
        Required = false,
        HelpText = "The \"Client API Key\" part of the Remote Deployment client's credentials.")]
    public string? RemoteDeploymentClientApiKey { get; set; }
}

internal static class Program
{
    public static Task Main(string[] args) =>
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithNotParsed(HandleParseError)
            .WithParsedAsync(options => RunOptionsAsync(options));

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

    private static async Task RunOptionsAsync(CommandLineOptions options)
    {
        try
        {
            await RunOptionsInnerAsync(options);
        }
        catch (Exception ex)
        {
            WriteLine("Deployment failed with the following exception: {0}", ex.ToString());
            Environment.ExitCode = 1;
        }
    }

    private static async Task RunOptionsInnerAsync(CommandLineOptions options)
    {
        // Creating directory for the deployment.
        var newDirectoryPath = CreateNewDirectoryPath(options);

        try
        {
            if (Directory.Exists(newDirectoryPath)) Directory.Delete(newDirectoryPath, recursive: true);

            Directory.CreateDirectory(newDirectoryPath);

            WriteLine("The \"{0}\" directory was created successfully.", newDirectoryPath);
        }
        catch (Exception)
        {
            WriteLine("Creating the directory {0} failed.", newDirectoryPath);
            throw;
        }

        var pathToTheme = options.PathOfTheTheme;

        // Creating media theme step.
        dynamic mediaThemeStep = new JObject();
        mediaThemeStep.name = "mediatheme";
        mediaThemeStep.BaseThemeId = string.IsNullOrEmpty(options.BaseThemeId) ? null : options.BaseThemeId;
        mediaThemeStep.ClearMediaThemeFolder = options.ClearMediaHostingFolder;

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
        var zipFilePath = newDirectoryPath + ".zip";
        ZipFile.CreateFromDirectory(newDirectoryPath, zipFilePath);

        // Getting rid of the original directory.
        Directory.Delete(newDirectoryPath, recursive: true);

        WriteLine("{0} was created successfully. ", zipFilePath);

        if (string.IsNullOrEmpty(options.RemoteDeploymentUrl))
        {
            return;
        }

        // This is a remote deployment.
        await RemoteDeploymentHelper.DeployAsync(options, zipFilePath);
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

            // Files should follow the template name convention (with underscores only), not the template file name
            // convention (with dots and hyphens). Note that these don't just differ in the interchangeable characters
            // but may also differ in the order of sections. E.g.
            // [Stereotype]_[DisplayType]__[PartType]__[PartName]__[DisplayMode]_Display is
            // "Widget_Summary__ServicePart__Services__CustomMode_Display" as template name but
            // "Widget-ServicePart-Services-CustomMode.Display.Summary.liquid" as a file name (note e.g. "Summary" being
            // in a different location).
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (areLiquidFiles && (fileNameWithoutExtension.Contains('.') || fileNameWithoutExtension.Contains('-')))
            {
                throw new InvalidOperationException(
                    "Liquid templates must follow the template name, not template *file* name conventions, i.e. they " +
                    " should only contain underscores, not dots and hyphens. See the docs for details: " +
                    "https://docs.orchardcore.net/en/latest/docs/reference/modules/Templates/. Offending file: " +
                    fileName);
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
        using var file = File.CreateText(Path.Join(newDirectoryPath, RecipeFile));
        using var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };
        recipe.WriteTo(writer);

        file.Close();
    }
}
