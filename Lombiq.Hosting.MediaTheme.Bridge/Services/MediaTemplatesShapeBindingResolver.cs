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
    private readonly IMediaTemplateService _mediaTemplateService;

    public MediaTemplatesShapeBindingResolver(
        ILiquidTemplateManager liquidTemplateManager,
        IHttpContextAccessor hca,
        HtmlEncoder htmlEncoder,
        IMediaTemplateService mediaTemplateService)
    {
        _liquidTemplateManager = liquidTemplateManager;
        _hca = hca;
        _htmlEncoder = htmlEncoder;
        _mediaTemplateService = mediaTemplateService;
    }

    public async Task<ShapeBinding> GetShapeBindingAsync(string shapeType)
    {
        if (AdminAttribute.IsApplied(_hca.HttpContext))
        {
            return null;
        }

        return await _mediaTemplateService.GetMediaTemplateByShapeTypeAsync(shapeType) is not { } mediaTemplate
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
