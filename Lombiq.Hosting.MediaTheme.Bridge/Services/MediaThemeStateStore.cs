using Lombiq.Hosting.MediaTheme.Bridge.Models;
using OrchardCore.Documents;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeStateStore(IDocumentManager<MediaThemeStateDocument> documentManager) : IMediaThemeStateStore
{
    public Task<MediaThemeStateDocument> LoadMediaThemeStateAsync() =>
        documentManager.GetOrCreateMutableAsync();

    public Task<MediaThemeStateDocument> GetMediaThemeStateAsync() =>
        documentManager.GetOrCreateImmutableAsync();

    public Task SaveMediaThemeStateAsync(MediaThemeStateDocument state) =>
        documentManager.UpdateAsync(state);
}
