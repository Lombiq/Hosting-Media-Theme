using Lombiq.Hosting.MediaTheme.Constants;
using Lombiq.Hosting.MediaTheme.Services;
using Microsoft.Extensions.FileProviders;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.Hosting.MediaTheme.Models;

public class MediaDirectoryContents : IDirectoryContents
{
    private readonly IEnumerable<IFileStoreEntry> _fileStoreEntries;
    private readonly IMediaFileStore _mediaFileStore;
    private readonly string _originalSubPath;

    public MediaDirectoryContents(IEnumerable<IFileStoreEntry> fileStoreEntries, IMediaFileStore mediaFileStore, string originalSubPath)
    {
        _fileStoreEntries = fileStoreEntries;
        _mediaFileStore = mediaFileStore;
        _originalSubPath = originalSubPath;
    }

    public IEnumerator<IFileInfo> GetEnumerator() =>
        _fileStoreEntries?.Select(entry =>
            new MediaFileInfo(
                entry,
                _mediaFileStore,
                _mediaFileStore.Combine(
                    _originalSubPath,
                    entry.Path.Replace(
                        _mediaFileStore.Combine(
                            Paths.MediaThemeRootPath,
                            Paths.MediaThemeTemplatesFolder),
                        string.Empty)))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Exists => _fileStoreEntries != null;
}
