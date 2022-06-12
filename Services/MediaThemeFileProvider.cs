using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using System;
using System.Linq;

namespace Lombiq.Hosting.MediaTheme.Services;

public class MediaThemeFileProvider : IFileProvider
{
    private readonly IMediaFileStore _mediaFileStore;

    public MediaThemeFileProvider(IMediaFileStore mediaFileStore) => _mediaFileStore = mediaFileStore;

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        if (AdjustPath(subpath) is not { } adjustedPath) return NotFoundDirectoryContents.Singleton;

        if (!_mediaFileStore.DirectoryExistsAsync(adjustedPath).GetAwaiter().GetResult())
        {
            return NotFoundDirectoryContents.Singleton;
        }

        var fileStoreEntriesValueTask = _mediaFileStore
            .GetDirectoryContentAsync(adjustedPath, includeSubDirectories: true)
            .ToListAsync();

        if (!fileStoreEntriesValueTask.IsCompletedSuccessfully) return NotFoundDirectoryContents.Singleton;

        return new MediaDirectoryContents(fileStoreEntriesValueTask.GetAwaiter().GetResult(), _mediaFileStore, subpath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (AdjustPath(subpath) is not { } adjustedPath) return new NotFoundFileInfo(subpath);

        var fileStoreEntry = _mediaFileStore.GetFileInfoAsync(adjustedPath).GetAwaiter().GetResult();
        return new MediaFileInfo(fileStoreEntry, _mediaFileStore, subpath);
    }

    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;

    private string AdjustPath(string path)
    {
        var adjustedPath = path.Replace('\\', '/').Trim('/');
        if (!adjustedPath.StartsWith(FeatureNames.MediaThemeModuleRoot, StringComparison.Ordinal)) return null;
        adjustedPath = adjustedPath[FeatureNames.MediaThemeModuleRoot.Length..];

        return _mediaFileStore.Combine(Paths.MediaThemeRootPath, adjustedPath);
    }
}
