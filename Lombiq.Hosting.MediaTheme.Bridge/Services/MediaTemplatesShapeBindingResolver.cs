using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.Liquid;
using OrchardCore.Themes.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaTemplatesShapeBindingResolver : IShapeBindingResolver
{
    private readonly ILiquidTemplateManager _liquidTemplateManager;
    private readonly IHttpContextAccessor _hca;
    private readonly HtmlEncoder _htmlEncoder;
    private readonly IMediaThemeCachingService _mediaThemeCachingService;
    private readonly ISiteThemeService _siteThemeService;

    public MediaTemplatesShapeBindingResolver(
        ILiquidTemplateManager liquidTemplateManager,
        IHttpContextAccessor hca,
        HtmlEncoder htmlEncoder,
        IMediaThemeCachingService mediaThemeCachingService,
        ISiteThemeService siteThemeService)
    {
        _liquidTemplateManager = liquidTemplateManager;
        _hca = hca;
        _htmlEncoder = htmlEncoder;
        _mediaThemeCachingService = mediaThemeCachingService;
        _siteThemeService = siteThemeService;
    }

    /// <summary>
    /// Resolves the shape binding for the given shape type. If the current site theme is the Media Theme, it will return a binding for the shape type
    /// using the media library content directly or from cache. Otherwise, it will return null, so the default shape binding will be used. Which means
    /// shapes will be loaded in the regular way.
    /// </summary>
    public async Task<ShapeBinding> GetShapeBindingAsync(string shapeType) =>
        !AdminAttribute.IsApplied(_hca.HttpContext) &&
        (await _siteThemeService.GetSiteThemeAsync()).Id == FeatureNames.MediaTheme &&
        await _mediaThemeCachingService.GetMemoryCachedMediaTemplateAsync(shapeType) is { } mediaTemplate
            ? new()
            {
                BindingName = shapeType,
                BindingSource = shapeType,
                BindingAsync = displayContext => BindingAsync(displayContext, mediaTemplate.Content),
            }
            : null;

    private async Task<IHtmlContent> BindingAsync(DisplayContext displayContext, string text)
    {
        var content = await _liquidTemplateManager.RenderHtmlContentAsync(text, _htmlEncoder, displayContext.Value);
        return content;
    }
}
