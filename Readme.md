# Lombiq Hosting - Media Theme for Orchard Core

[![Lombiq.Hosting.MediaTheme NuGet](https://img.shields.io/nuget/v/Lombiq.Hosting.MediaTheme?label=Lombiq.Hosting.MediaTheme)](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme/)
[![Lombiq.Hosting.MediaTheme.Bridge NuGet](https://img.shields.io/nuget/v/Lombiq.Hosting.MediaTheme.Bridge?label=Lombiq.Hosting.MediaTheme.Bridge)](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme.Bridge/)
[![Lombiq.Hosting.MediaTheme.Deployer NuGet](https://img.shields.io/nuget/v/Lombiq.Hosting.MediaTheme.Deployer?label=Lombiq.Hosting.MediaTheme.Deployer)](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme.Deployer/)
[![Lombiq.Hosting.MediaTheme.Tests.UI NuGet](https://img.shields.io/nuget/v/Lombiq.Hosting.MediaTheme.Tests.UI?label=Lombiq.Hosting.MediaTheme.Tests.UI)](https://www.nuget.org/packages/Lombiq.Hosting.MediaTheme.Tests.UI/)

## About

The Media Theme feature will allow developers to host their themes in the Orchard Core Media Library, including templates and assets. This is useful in SaaS scenarios where you can't allow everyone to develop full Orchard themes, but still want to make the usual Orchard theming experience available to all users. Media Theme allows full theming of sites running on [DotNest](https://dotnest.com/), the Orchard SaaS, too.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## Documentation

### Getting started

There are two Orchard Core extensions in this project:

- `Lombiq.Hosting.MediaTheme.Bridge`: An Orchard Core module that enables the core logic required for hosting your theme the Media Library.
- `Lombiq.Hosting.MediaTheme`: An Orchard Core theme that bootstraps the media theme logic using the `Lombiq.Hosting.MediaTheme.Bridge` module. With this theme active you can dynamically change the base theme from the Admin UI or using recipes.

To get started, you'll need to add `Lombiq.Hosting.MediaTheme` and `Lombiq.Hosting.MediaTheme.Bridge` NuGet packages to your web project. Set the `Lombiq.Hosting.MediaTheme` as the active site theme from the Admin UI or recipe.

Once the theme is enabled, it'll look for templates and assets in the Media Library. Go to Media Library and create the __MediaTheme_ folder, where you can put shape templates inside the _Templates_ folder (see limitations below), and assets inside the _Assets_ folder.

Note that the `.liquid` extension is not allowed in Media Library by default, which might be an issue with a few of your assets as well. Please update the Media settings as described [here](https://docs.orchardcore.net/en/latest/docs/reference/modules/Media/#configuration).

### Using a base theme

Media Theme supports base themes. First, enable the theme you want to use as a base theme. Then go to the Admin UI → Configuration → Media Theme page and select the one you want to use. You won't be able to update the base theme dynamically if the Media Theme is not active even if you are using the Media Theme logic provided by the `Lombiq.Hosting.MediaTheme.Bridge`.

### Limitations

- Shape templates must be _.liquid_ files. You can't use Razor templates.
- Shape template file names must be valid template, not template file names, i.e., may not contain `.` and `-` characters (but use the `_` and `__` notation instead). Note that these don't just differ in the interchangeable characters but may also differ in the order of sections. E.g. `[Stereotype]_[DisplayType]__[PartType]__[PartName]__[DisplayMode]_Display` is "Widget_Summary__ServicePart__Services__CustomMode_Display" as template name but "Widget-ServicePart-Services-CustomMode.Display.Summary.liquid" as a file name (note e.g. "Summary" being in a different location). See [the docs](https://docs.orchardcore.net/en/latest/docs/reference/modules/Templates/) for details.
- The theme may include static resources in its _wwwroot_ folder as any theme. You can also build those from source files like SCSS files as usual.
- The theme mustn't include any C# code apart from its `Manifest`.
- Folders within the _Views_ folder are not supported. It also means that MVC views can't be overridden or custom shape harvesters can't be used.

### Local development

If you want to build a theme that'll eventually end up in the Media Library, you can develop it as usual but make sure the `Lombiq.Hosting.MediaTheme.Bridge` module is its dependency. You can set a base theme for your theme from the `Manifest` as usual too. Note that you won't be able to change the base theme dynamically from the Admin UI during local development.

You can proceed with developing your theme as you'd typically do: put the templates inside the _Views_ folder and static resources inside the _wwwroot_ folder; however, keep the limitations mentioned above in mind.

If you want to reference assets in your templates, you can use the `/mediatheme/` prefix in URLs, like below:

```liquid
{% assign 32pxIconUrl = "~/mediatheme/images/favicon-32x32.png" | href %}
{% link type:"image/png", rel:"icon", sizes:"32x32", href:32pxIconUrl, append_version:"true" %}

{% assign stylesPath = "~/mediatheme/styles/site.css" | href %}
{% style src:stylesPath %}
```

These use Orchard's resource manager and thus will also include a browser/proxy cache busting `v` parameter that updates when you deploy a new version of your theme. This will ensure that everybody sees the current version of your site's styling. Note that while the Liquid `style` and `script` tags do this by default, for `link` you have to add `append_version` like above too.

Media Theme will translate this special _~/mediatheme_ path to either your local theme asset path or Media Library if the file exists. This way, you don't need to update your asset URLs in your templates one-by-one when deploying them. The `~` notation of virtual paths also comes in handy if you want to work with multiple tenants using URL prefixes locally, i.e. develop multiple Media Themes for multiple sites from the same solution.

If you are developing a theme for your [DotNest](https://dotnest.com) site you can use the [DotNest Core SDK](https://github.com/Lombiq/DotNest-Core-SDK) that has everything prepared for you right away.

### Deployment (import/export)

#### Importing a deployment package created by the Deployer tool

Instead of manually uploading files to the Media Library, [install](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-use) the `Lombiq.Hosting.MediaTheme.Deployer` dotnet tool:

```pwsh
dotnet tool install --global Lombiq.Hosting.MediaTheme.Deployer
```

Then you can use it to build a deployment package of your theme:

```pwsh
media-theme-deploy --path [path of your theme] --base-id [base theme id] --clear [clear media hosting folder] --deployment-path [deployment path]
```

A specific example when run in the folder of your theme project:

```pwsh
media-theme-deploy --path . --base-id TheTheme --clear true --deployment-path .\Deployment
```

- `--base-id` is optional. If not provided, the tool will try to get it from the Manifest file, and if it's not defined there either, no base theme will be used.
- `--deployment-path` is optional. Without it, the package will be exported to your directory root, for example _C:\MediaThemeDeployment_04Aug2022230500.zip_. The parameters also have shorthand versions, `-p`, `-i`, `-c`, `-d`, respectively.

You can then take the resulting ZIP file and import it on your site from the Admin UI → Configuration → Import/Export → Package Import. Everything necessary will be configured by the package. If you don't see this menu item then first enable the "Deployment" feature under Configuration → Features.

#### Remote deployment with the Deployer tool

You can use [Remote Deployment](https://docs.orchardcore.net/en/latest/docs/reference/modules/Deployment.Remote/) to accept packages created with the above-explained Deployer too via the internet, without manually uploading the ZIP file. You can use this to deploy your theme remotely from your local development environment or CI workflow too, for which we provide a ready-to-use [GitHub Actions workflow](https://github.com/features/actions).

Do the following to set up automated GitHub Actions deployments:

1. Create a Remote Client on the Orchard admin UI → Configuration → Import/Export → Remote Clients. Use a suitable name and a strong, unique API key. If you don't see this menu item then first enable the "Remote Deployment" feature under Configuration → Features.
2. Configure the Client API Key as a [repository secret](https://docs.github.com/en/actions/security-guides/encrypted-secrets#creating-encrypted-secrets-for-a-repository). While not strictly necessary, we recommend also storing the Client Name and Remote Deployment URL as secrets too.
3. Add a workflow to the _.github/workflows_ folder of your repository that executes the `deploy-media-theme` reusable workflow with some suitable configuration:

```yaml
name: Deploy Media Theme to DotNest

on:
  push:
    branches:
      - my-dev
    paths:
      - 'src/Themes/My.Theme/**'

jobs:
  deploy-media-theme:
    uses: Lombiq/Hosting-Media-Theme/.github/workflows/deploy-media-theme.yml@dev
    secrets:
      URL: ${{ secrets.MY_THEME_DEPLOYMENT_URL }}
      CLIENT_NAME: ${{ secrets.MY_THEME_DEPLOYMENT_CLIENT_NAME }}
      CLIENT_API_KEY: ${{ secrets.MY_THEME_DEPLOYMENT_CLIENT_API_KEY }}
    with:
      theme-path: "src/Themes/My.Theme"
      # You can leave out base-theme-id to get it from the Manifest, or to not use a base theme at all.
      #base-theme-id: "TheBlogTheme"
```

If you want to use a different CI system or would like to run remote deployment from the command line otherwise, use the `--remote-deployment-url`, `--remote-deployment-client-name`, and `--remote-deployment-client-api-key` parameters. See this PowerShell script for an example:

```pwsh
$switches = @(
    '--path', '.'
    '--remote-deployment-url', 'https://localhost:44335/OrchardCore.Deployment.Remote/ImportRemoteInstance/Import'
    '--remote-deployment-client-name', 'demo'
    '--remote-deployment-client-api-key', 'Password1!'
)

media-theme-deploy @switches
```

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.

### Publishing and using a version of the Deployer during local development

If you want to publish `Lombiq.Hosting.MediaTheme.Deployer` to test it during local development, then do the following:

1. Pack it into a NuGet package by running `dotnet pack` in the project's folder.
2. Uninstall the tool if you have it installed already: `dotnet tool uninstall --global Lombiq.Hosting.MediaTheme.Deployer`.
3. Install the local version (note that without a version specified, it'll be published as v1.0.0): `dotnet tool install --global Lombiq.Hosting.MediaTheme.Deployer --add-source .\nupkg --version 1.0.0`.<!-- #spell-check-ignore-line -->
4. Now `media-theme-deploy` will use the development version.
