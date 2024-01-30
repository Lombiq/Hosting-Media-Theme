using Lombiq.HelpfulLibraries.Common.Utilities;
using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
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
/// The default <see cref="IFileVersionProvider"/>, <see cref="ShellFileVersionProvider"/>, works just fine, but we need
/// to translate /mediatheme URLs.
/// </para>
/// </remarks>
internal sealed class FileVersionProviderDecorator : IFileVersionProvider
{
    private readonly IFileVersionProvider _decorated;
    private readonly IMediaFileStore _mediaFileStore;
    private readonly IOptions<MediaOptions> _mediaOption;
    private readonly NonSecurityRandomizer _randomizer = new();

    public FileVersionProviderDecorator(
        IFileVersionProvider decorated,
        IMediaFileStore mediaFileStore,
        IOptions<MediaOptions> mediaOptions)
    {
        _decorated = decorated;
        _mediaFileStore = mediaFileStore;
        _mediaOption = mediaOptions;
    }

    public
        string AddFileVersionToPath(PathString requestPathBase, string path)
    {
        var isMediaThemePath = path.StartsWithOrdinalIgnoreCase(Routes.MediaThemeAssets) ||
            path.ContainsOrdinalIgnoreCase(Routes.MediaThemeAssets + "/");

        if (!isMediaThemePath)
        {
            return _decorated.AddFileVersionToPath(requestPathBase, path);
        }

        var assetsSubPath = _mediaFileStore.Combine(
            _mediaOption.Value.AssetsRequestPath, Paths.MediaThemeRootFolder, Paths.MediaThemeAssetsFolder);

        // Note that this will work all the time for local files. When a remote storage implementation is used to store
        // Media files though (like Azure Blob Storage) then Media Cache will mirror the files locally. Since this only
        // happens on the first request to the file, until then in the HTML output you'll see a URL without the cache
        // busting parameter.
        // This is an issue because if the browser of a reverse proxy caches the URL without the cache busting parameter
        // then the original file will get stuck, and no cache busting parameter will be added until the new file is
        // accessed with some other cache busting parameter. So, before the actual cache busting parameter can be added,
        // we need to add a random parameter.
        var cacheBustedPath = _decorated.AddFileVersionToPath(requestPathBase, path);

        // This check could be more sophisticated with UriBuilder, but let's keep it simple, since it'll run frequently.
        if (!cacheBustedPath.Contains("?v="))
        {
            return QueryHelpers.AddQueryString(path, "mediatheme", _randomizer.Get().ToTechnicalString());
        }

        return cacheBustedPath.Replace(assetsSubPath, Routes.MediaThemeAssets);
    }
}
