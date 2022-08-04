using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;

namespace Lombiq.Hosting.MediaTheme.Deployer;

internal class Program
{
    private readonly IStringLocalizer T;

    public Program(IStringLocalizer stringLocalizer) =>
        T = stringLocalizer;

    public static void Main(string[] args)
    {
        if (args.Length == 3 && bool.TryParse(args[2], out bool clearMediaFolder))
        {
            var pathToTheme = args[0];

            // Creating media theme step.
            dynamic mediaThemeStep = new JObject();
            mediaThemeStep.name = "mediatheme";
            mediaThemeStep.ClearMediaThemeFolder = clearMediaFolder;
            mediaThemeStep.BaseThemeId = args[1];

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

            // Getting templates.
            var pathToTemplates = Path.Join(pathToTheme, "Views");

            var allTemplatesPaths = Directory
                .EnumerateFiles(pathToTemplates, "*.liquid", SearchOption.TopDirectoryOnly);

            foreach (var templatesPath in allTemplatesPaths)
            {
                dynamic templateJObject = new JObject();
                templateJObject.SourcePath = Path.Join(
                    "_MediaTheme/Templates",
                    templatesPath[pathToTemplates.Length..].Replace("\\", "/"));
                templateJObject.TargetPath = templateJObject.SourcePath;

                files.Add(templateJObject);
            }

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

            var currentRootDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            var newDirectoryPath = Path.Join(currentRootDirectory, "MediaThemeDeployment_")
                + DateTime.Now.ToString("ddMMMyyyyHHmmss", CultureInfo.CurrentCulture);

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(newDirectoryPath))
                {
                    Console.WriteLine("That file exists already.");
                    return;
                }

                // Try to create the directory.
                Directory.CreateDirectory(newDirectoryPath);
                Console.WriteLine("The .zip was created successfully. {0}", newDirectoryPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine("The process failed: {0}", exception.ToString());
                return;
            }

            using var file = File.CreateText(Path.Join(newDirectoryPath + "\\Recipe.json"));
            using var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };
            recipe.WriteTo(writer);

            file.Close();

            ZipFile.CreateFromDirectory(newDirectoryPath, newDirectoryPath + ".zip");

        }
        else
        {
            var versionString = Assembly
                .GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            Console.WriteLine($"media-theme-deploy v{versionString}");
            Console.WriteLine("-------------");
            Console.WriteLine("\nUsages:");
            Console.WriteLine("  media-theme-deploy <path to the theme> <base theme id> <clear media theme folder>");
            return;
        }
    }
}
