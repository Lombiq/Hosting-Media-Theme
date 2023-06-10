# Lombiq Hosting - Media Theme MSBuild Targets

[![Lombiq.Hosting.MediaTheme.Targets NuGet](https://img.shields.io/nuget/v/Lombiq.Hosting.MediaTheme.Targets?label=Lombiq.Hosting.MediaTheme.Targets)](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme.Targets/)

## About

Provides automatic Media Theme package creation in the project. This way you don't have to manage changes in your Media Theme recipe, or folder structure. The targets reuses the `media-theme-deployer` dotnet tool, what will be automatically installed locally to the project, then it will create the deployment zip file. Please read the [Lombiq.Hosting.MediaTheme Readme](https://github.com/Lombiq/Hosting-Media-Theme/blob/dev/Readme.md) for more information about this deployer tool.

When the zip is ready it is unzipped and its files are moved to the project relative _Recipe/\_MediaTheme/_ path. The Recipe is moved directly to the project relative _Recipe_ folder.

Note, that these operations are optimized by running them only if the corresponding files have been changed.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## How to use

Install the [NuGet package](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme.Targets/) or if you use the project from a submodule, add the following lines to the csproj file. Make sure that the paths are pointing to the _Lombiq.Hosting.MediaTheme.Targets.props_ and _Lombiq.Hosting.MediaTheme.Targets.targets_ files of this project.

```xml
    <Import Project="path/to/Lombiq.Hosting.MediaTheme.Targets/Lombiq.Hosting.MediaTheme.Targets.props" />
    <Import Project="path/to/Lombiq.Hosting.MediaTheme.Targets/Lombiq.Hosting.MediaTheme.Targets.targets" />
```

Override the `MediaThemeRecipeFileName` property to modify the Recipe file name and the name property in the recipe.

```xml
<PropertyGroup>
    <MediaThemeRecipeFileName>Sample.MediaTheme</MediaThemeRecipeFileName>
</PropertyGroup>
```

Add a dotnet-tools.json file into the .config folder in your module:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "lombiq.hosting.mediatheme.deployer": {
      "version": "3.0.1",
      "commands": [
        "media-theme-deploy"
      ]
    }
  }
}
```

To have a successful run you must follow the required shape templates name convention as it is also required for the [Media Theme Deployer](https://github.com/Lombiq/Hosting-Media-Theme/blob/dev/Readme.md#limitations).

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
