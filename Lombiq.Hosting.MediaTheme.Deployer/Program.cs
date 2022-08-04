using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using static System.Console;

namespace Lombiq.Hosting.MediaTheme.Deployer;

internal static class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Globalization",
        "CA1303:Do not pass literals as localized parameters",
        Justification = "It's a console application it doesn't need localization.")]
    public static void Main(string[] args)
    {
        if (args.Length == 3 && bool.TryParse(args[2], out bool clearMediaFolder))
        {
            // Creating directory for the deployment.
            var currentRootDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            var newDirectoryPath = Path.Join(currentRootDirectory, "MediaThemeDeployment_")
                + DateTime.Now.ToString("ddMMMyyyyHHmmss", CultureInfo.CurrentCulture);

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(newDirectoryPath))
                {
                    WriteLine("That file already exists.");
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

            var pathToTheme = args[0];

            // Creating media theme step.
            dynamic mediaThemeStep = new JObject();
            mediaThemeStep.name = "mediatheme";
            mediaThemeStep.BaseThemeId = args[1];
            mediaThemeStep.ClearMediaThemeFolder = clearMediaFolder;

            // Creating media step.
            var files = new JArray();

            // Getting assets.
            var pathToAssets = Path.Join(pathToTheme, "wwwroot");

            var allAssetsPaths = Directory.EnumerateFiles(pathToAssets, "*", SearchOption.AllDirectories);

            foreach (var assetPath in allAssetsPaths)
            {
                dynamic assetJObject = new JObject();
                assetJObject.SourcePath = Path.Join(
                    "_MediaTheme/Assets",
                    assetPath[pathToAssets.Length..].Replace("\\", "/"));
                assetJObject.TargetPath = assetJObject.SourcePath;

                files.Add(assetJObject);
            }

            // Copying assets to deployment directory.
            CopyDirectory(pathToAssets, Path.Join(newDirectoryPath, "\\_MediaTheme\\Assets"));

            // Getting templates.
            var pathToTemplates = Path.Join(pathToTheme, "Views");

            var allTemplatesPaths = Directory
                .EnumerateFiles(pathToTemplates, "*.liquid", SearchOption.TopDirectoryOnly);

            foreach (var templatePath in allTemplatesPaths)
            {
                dynamic templateJObject = new JObject();
                templateJObject.SourcePath = Path.Join(
                    "_MediaTheme/Templates",
                    templatePath[pathToTemplates.Length..].Replace("\\", "/"));
                templateJObject.TargetPath = templateJObject.SourcePath;

                files.Add(templateJObject);
            }

            // Copying templates to deployment directory.
            CopyDirectory(pathToTemplates, Path.Join(newDirectoryPath, "\\_MediaTheme\\Templates"), recursive: false);

            dynamic mediaStep = new JObject();
            mediaStep.name = "media";
            mediaStep.Files = files;

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
            using var file = File.CreateText(Path.Join(newDirectoryPath + "\\Recipe.json"));
            using var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };
            recipe.WriteTo(writer);

            file.Close();

            // Zipping the directory.
            var zippedDirectoryPath = newDirectoryPath + ".zip";
            ZipFile.CreateFromDirectory(newDirectoryPath, zippedDirectoryPath);

            // Getting rid of the original directory.
            Directory.Delete(newDirectoryPath, recursive: true);

            WriteLine("{0} was created successfully. ", zippedDirectoryPath);
        }
        else
        {
            var versionString = Assembly
                .GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            WriteLine($"media-theme-deploy v{versionString}");
            WriteLine("-------------");
            WriteLine("\nUsages:");
            WriteLine("  media-theme-deploy <path to the theme> <base theme id> <clear media theme folder>");
        }
    }

    private static void CopyDirectory(string sourceDir, string destinationDirectory, bool recursive = true)
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
            string targetFilePath = Path.Combine(destinationDirectory, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method.
        if (recursive)
        {
            foreach (var subDirectory in directories)
            {
                string newDestinationDir = Path.Combine(destinationDirectory, subDirectory.Name);
                CopyDirectory(subDirectory.FullName, newDestinationDir);
            }
        }
    }
}
