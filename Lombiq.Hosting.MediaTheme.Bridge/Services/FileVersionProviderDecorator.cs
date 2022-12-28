using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using OrchardCore.Mvc;
using System;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

/// <summary>
/// Service to add a cache busting version key to the URL of static resources references from Media Theme, like it
/// happens for standard resources.
/// </summary>
/// <remarks>
/// <para>
/// The default <see cref="IFileVersionProvider"/>, <see cref="ShellFileVersionProvider"/>, caches the version keys
/// until tenant restart (and in a second level, until app restart). While that's fine for local files that are part of
/// the app's source code, like CSS files in themes, it won't work for Media Theme files that can be updated any time
/// without app restart.
/// </para>
/// </remarks>
internal class FileVersionProviderDecorator : IFileVersionProvider
{
    private readonly IFileVersionProvider _decorated;
    private readonly IMediaFileStore _mediaFileStore;
    private readonly IOptions<MediaOptions> _mediaOption;

    public FileVersionProviderDecorator(
        IFileVersionProvider decorated,
        IMediaFileStore mediaFileStore,
        IOptions<MediaOptions> mediaOptions)
    {
        _decorated = decorated;
        _mediaFileStore = mediaFileStore;
        _mediaOption = mediaOptions;
    }

    public string AddFileVersionToPath(PathString requestPathBase, string path)
    {
        var isMediaThemePath = path.StartsWithOrdinalIgnoreCase(Routes.MediaThemeAssets) ||
            path.ContainsOrdinalIgnoreCase(Routes.MediaThemeAssets + "/");

        if (isMediaThemePath)
        {
            var assetsSubPath = _mediaFileStore.Combine(
                _mediaOption.Value.AssetsRequestPath, Paths.MediaThemeRootFolder, Paths.MediaThemeAssetsFolder);
            path = path.Replace(Routes.MediaThemeAssets, assetsSubPath);
        }

        // Note that if this will work all the time for local files. When a remote storage implementation is used to
        // store Media files though (like Azure Blob Storage) then Media Cache will mirror the files locally. Since this
        // only happens on the first request to the file, until then in the HTML output you'll see a URL without the
        // cache busting parameter.
        // This isn't an issue for real-life scenarios, just be mindful during development.
        return _decorated.AddFileVersionToPath(requestPathBase, path);
    }
}
