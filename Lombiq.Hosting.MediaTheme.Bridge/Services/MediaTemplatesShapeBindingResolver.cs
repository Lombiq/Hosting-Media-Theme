using Microsoft.AspNetCore.Http;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Liquid;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaTemplatesShapeBindingResolver(
    ILiquidTemplateManager liquidTemplateManager,
    IHttpContextAccessor hca,
    HtmlEncoder htmlEncoder,
    IMediaThemeCachingService mediaThemeCachingService) : IShapeBindingResolver
{
    public async Task<ShapeBinding> GetShapeBindingAsync(string shapeType)
    {
        if (AdminAttribute.IsApplied(hca.HttpContext))
        {
            return null;
        }

        return await mediaThemeCachingService.GetMemoryCachedMediaTemplateAsync(shapeType) is not { } mediaTemplate
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
                var content = await liquidTemplateManager.RenderHtmlContentAsync(text, htmlEncoder, displayContext.Value);
                return content;
            },
        };
}
