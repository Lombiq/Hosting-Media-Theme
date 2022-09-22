# Lombiq Hosting - Media Theme for Orchard Core

[![Lombiq.Hosting.MediaTheme NuGet](https://img.shields.io/nuget/v/Lombiq.Hosting.MediaTheme?label=Lombiq.Hosting.MediaTheme)](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme/)

## About

The Media Theme feature will allow developers to host their themes in the Orchard Core Media Library, including templates and assets.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## Documentation

### Getting started

There are two Orchard Core extensions in this project:

- `Lombiq.Hosting.MediaTheme.Bridge`: An Orchard Core module that enables the core logic required for hosting your theme the Media Library.
- `Lombiq.Hosting.MediaTheme`: An Orchard Core theme that bootstraps the media theme logic using the `Lombiq.Hosting.MediaTheme.Bridge` module. With this theme active you can dynamically change the base theme from the Admin UI or using recipes.

To get started, you'll need to add `Lombiq.Hosting.MediaTheme` and `Lombiq.Hosting.MediaTheme.Bridge` NuGet packages to your web project. Set the `Lombiq.Hosting.MediaTheme` as the active site theme from the Admin UI or recipe.

Once the theme is enabled, it'll look for templates and assets in the Media Library. Go to Media Library and create the `_MediaTheme` folder, where you can put shape templates inside the _Templates_ folder (see limitations below), and assets inside the _Assets_ folder.

Note that the `.liquid` extension is not allowed in Media Library by default, which might be an issue with a few of your assets as well. Please update the Media settings as described [here](https://docs.orchardcore.net/en/dev/docs/reference/modules/Media/#configuration).

### Using a base theme

Media Theme supports base themes. First, enable the theme you want to use as a base theme. Then go to the _Admin UI > Configuration > Media Theme_ page and select the one you want to use. You won't be able to update the base theme dynamically if the Media Theme is not active even if you are using the Media Theme logic provided by the `Lombiq.Hosting.MediaTheme.Bridge`.

### Limitations

- Shape templates must be _.liquid_ files.
- Shape template file names must be valid shape types, i.e., instead of `.` and `-`, use `_` and `__` characters.
- Folders within the shape templates are not supported. It also means that MVC views can't be overridden or custom shape harvesters can't be used.

### Local development

If you want to build a theme that'll eventually end up in the Media Library, you can develop your theme as usual but make sure the `Lombiq.Hosting.MediaTheme.Bridge` module is enabled. You can set a base theme for your theme from `Manifest.cs`. Note that you won't be able to change the base theme dynamically from the Admin UI during local development.

You can proceed with developing your base theme as you'd typically do: put the templates inside the _Views_ folder and assets inside the _wwwroot_ folder; however, keep the limitations mentioned above in mind.

If you want to reference assets in your templates, you can use the `/mediatheme/` prefix. The Media Theme will translate this path to either your local theme asset path or Media Library if the file exists. This way, you don't need to update your asset URLs in your templates one-by-one when moving them to Media Library.

If you are developing a theme for your [DotNest](https://dotnest.com) site you can use the [DotNest Core SDK](https://github.com/Lombiq/DotNest-Core-SDK) that has everything prepared for you right away.

### Import/Export

If you want to export your Media Theme, go to the `_Admin UI > Configuration > Import/Export > Deployment Plans` page and create a Deployment Plan with the following steps:

- Add _Media Theme_ step. Here you can tick the _Clear Media Theme folder_ checkbox; if ticked, it will delete all the files in the `_MediaTheme` folder in the Media Library during import. It can be helpful if you have a _Media_ step along with this step bringing in all the Media Theme files, but be conscious of the order within the recipe; put the _Media Theme_ step first. Leave it disabled if you only want to control the base theme.
- Optionally, add a _Media_ step where you select the whole `_MediaTheme` folder.

Alternatively you can [install](https://docs.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use) the `Lombiq.Hosting.MediaTheme.Deployer` dotnet tool in your root project. Then you can use the tool:

```xml
dotnet tool run media-theme-deploy -p [path of your theme] -i [base theme id] -c [clear media hosting folder] -d [deployment path]
```

A specific example:

```xml
dotnet tool run media-theme-deploy -p .\src\Themes\MyTheme -i TheTheme -c true -d C:\MyFolder
```

Option `-d` is not required. Without it, the package will be exported to your directory root. For example `C:\MediaThemeDeployment_04Aug2022230500.zip`.

You can use Remote Deployment to accept such exported packages to deploy your theme remotely from your local development environment or CI.

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
