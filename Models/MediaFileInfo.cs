using Microsoft.Extensions.FileProviders;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using System;
using System.IO;

namespace Lombiq.Hosting.MediaTheme.Models;

public class MediaFileInfo : IFileInfo
{
    private readonly IFileStoreEntry _fileStoreEntry;
    private readonly IMediaFileStore _mediaFileStore;

    public MediaFileInfo(IFileStoreEntry fileStoreEntry, IMediaFileStore mediaFileStore, string originalPath)
    {
        _fileStoreEntry = fileStoreEntry;
        _mediaFileStore = mediaFileStore;
        PhysicalPath = originalPath;
    }

    public Stream CreateReadStream() => _mediaFileStore.GetFileStreamAsync(_fileStoreEntry).GetAwaiter().GetResult();

    public bool Exists => _fileStoreEntry != null;
    public bool IsDirectory => _fileStoreEntry?.IsDirectory ?? default;
    public DateTimeOffset LastModified => _fileStoreEntry != null
        ? new DateTimeOffset(_fileStoreEntry.LastModifiedUtc)
        : default;

    public long Length => _fileStoreEntry?.Length ?? default;
    public string Name => _fileStoreEntry?.Name;
    public string PhysicalPath { get; }
}
