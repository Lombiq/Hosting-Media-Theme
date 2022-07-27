# Lombiq Hosting - Media Theme for Orchard Core



## About

The Media Theme feature will allow developers to host their themes in the Orchard Core Media Library, including templates and assets.


## Documentation

### Getting started

There are two Orchard Core extensions in this project:
- `Lombiq.Hosting.MediaTheme.Bridge`: An Orchard Core module that enables the core logic required for media theme. It requires `Lombiq.Hosting.MediaTheme` theme to be enabled.
- `Lombiq.Hosting.MediaTheme`: An Orchard Core theme that bootstraps the media theme logic using the `Lombiq.Hosting.MediaTheme.Bridge` module.

To get started, you'll need to add `Lombiq.Hosting.MediaTheme` as a dependency to your web project and set it as the active site theme.

Once the theme is enabled, it'll look for templates and assets in the Media Library. Go to Media Library and create the `_MediaTheme` folder, where you can put
shape templates inside the `Views` folder (see limitations below),
assets inside the `wwwroot` folder.

Note that the `.liquid` extension is not allowed in Media Library by default, which might include a few of your asset extensions. Please update the Media settings as described [here](https://docs.orchardcore.net/en/dev/docs/reference/modules/Media/#configuration).

### Using a base theme

Media Theme supports base themes. First, enable the theme you want to use as a base theme. Then go to the _Admin UI > Configuration > Media Theme_ page and select the one you want to use.

### Limitations

- Shape templates must be _.liquid_ files.
- Shape template file names must be valid shape types, i.e., instead of `.` and `-`, use `_` and `__` characters.
- Folders within the shape templates are not supported. It also means that MVC views can't be overridden or custom shape harvesters can't be used.

### Local development

If you want to build a theme that'll eventually end up in the Media Library, you can set the `Lombiq.Hosting.MediaTheme` as the base theme for your theme. You can select another base theme as mentioned above.

You can proceed with developing your base theme as you'd typically do: put the templates inside the _Views_ folder and assets inside the _wwwroot_ folder; however, keep the limitations mentioned above in mind.

If you want to reference assets in your templates, you can use the `/mediatheme/` prefix. The Media Theme will translate this path to either your local theme asset path or Media Library if the file exists. This way, you don't need to update your asset URLs in your templates one-by-one when moving them to Media Library.

### Import/Export

If you want to export your Media Theme, you need to go to the `_Admin UI > Configuration > Import/Export > Deployment Plans` page and create a Deployment Plan with the following steps:
- Add _Media Theme_ step. Here you can tick the  _Clear Media Theme folder_ checkbox; if ticked, it will delete all the files in the `_MediaTheme` folder in the Media Library during import. It can be helpful if you have a _Media_ step along with this step bringing all the Media Theme files, but be conscious of the order within the recipe; put the _Media Theme_ step first. Leave it disabled if you only want to control the base theme.
- Optionally, add a _Media_ step where you select the whole `_MediaTheme` folder.

You can use Remote Deployment to accept such exported packages to deploy your theme remotely from your local development environment or CI.


## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.