using Microsoft.AspNetCore.Http;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Liquid;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaTemplatesShapeBindingResolver : IShapeBindingResolver
{
    private readonly ILiquidTemplateManager _liquidTemplateManager;
    private readonly IHttpContextAccessor _hca;
    private readonly HtmlEncoder _htmlEncoder;
    private readonly IMediaThemeManager _mediaThemeManager;

    public MediaTemplatesShapeBindingResolver(
        ILiquidTemplateManager liquidTemplateManager,
        IHttpContextAccessor hca,
        HtmlEncoder htmlEncoder,
        IMediaThemeManager mediaThemeManager)
    {
        _liquidTemplateManager = liquidTemplateManager;
        _hca = hca;
        _htmlEncoder = htmlEncoder;
        _mediaThemeManager = mediaThemeManager;
    }

    public async Task<ShapeBinding> GetShapeBindingAsync(string shapeType)
    {
        if (AdminAttribute.IsApplied(_hca.HttpContext))
        {
            return null;
        }

        return await _mediaThemeManager.GetMediaTemplateByShapeTypeAsync(shapeType) is not { } mediaTemplate
            ? null
            : BuildShapeBinding(shapeType, mediaTemplate.Content);
    }

    private ShapeBinding BuildShapeBinding(string shapeType, string text) =>
        new()
        {
            BindingName = shapeType,
            BindingSource = shapeType,
            BindingAsync = async displayContext =>
            {
                var content = await _liquidTemplateManager.RenderHtmlContentAsync(text, _htmlEncoder, displayContext.Value);
                return content;
            },
        };
}
