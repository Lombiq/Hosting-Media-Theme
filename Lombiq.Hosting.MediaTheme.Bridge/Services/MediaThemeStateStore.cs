using Lombiq.Hosting.MediaTheme.Bridge.Models;
using OrchardCore.Documents;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeStateStore : IMediaThemeStateStore
{
    private readonly IDocumentManager<MediaThemeStateDocument> _documentManager;

    public MediaThemeStateStore(IDocumentManager<MediaThemeStateDocument> documentManager) =>
        _documentManager = documentManager;

    public Task<MediaThemeStateDocument> LoadMediaThemeStateAsync() =>
        _documentManager.GetOrCreateMutableAsync();

    public Task<MediaThemeStateDocument> GetMediaThemeStateAsync() =>
        _documentManager.GetOrCreateImmutableAsync();

    public Task SaveMediaThemeStateAsync(MediaThemeStateDocument state) =>
        _documentManager.UpdateAsync(state);
}
