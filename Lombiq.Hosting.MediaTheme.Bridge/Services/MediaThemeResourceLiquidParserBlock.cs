using Fluid;
using Fluid.Ast;
using Lombiq.HelpfulLibraries.OrchardCore.Liquid;
using OrchardCore;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.Hosting.MediaTheme.Bridge.Services;

public class MediaThemeResourceLiquidParserBlock : ILiquidParserBlock
{
    private const string Stylesheet = "stylesheet";
    private const string Script = "script";

    private readonly IOrchardHelper _orchardHelper;
    private readonly IResourceManager _resourceManager;

    public MediaThemeResourceLiquidParserBlock(IOrchardHelper orchardHelper, IResourceManager resourceManager)
    {
        _orchardHelper = orchardHelper;
        _resourceManager = resourceManager;
    }

    public async ValueTask<Completion> WriteToAsync(
        IReadOnlyList<FilterArgument> argumentsList,
        IReadOnlyList<Statement> statements,
        TextWriter writer,
        TextEncoder encoder,
        TemplateContext context)
    {
        // One argument is required, which indicates the resource type. It defaults to "stylesheet"
        var type = (await argumentsList.Single().Expression.EvaluateAsync(context)).ToStringValue();
        if (string.IsNullOrWhiteSpace(type) || type.StartsWithOrdinalIgnoreCase("style")) type = Stylesheet;

        var root = Stylesheet.EqualsOrdinalIgnoreCase(type) ? "css" : "js";
        var content = await statements.RenderAsync(encoder, context);

        content?
            .Split('\n')
            .SelectWhere(line => line.Trim(), line => line is { Length: > 0 })
            .Select(line => _orchardHelper.ResourceUrl($"~/mediatheme/{root}/{line}"))
            .ForEach(url =>
            {
                switch (type.ToUpperInvariant())
                {
                    case "STYLESHEET":
                        _resourceManager.RegisterUrl(Stylesheet, url, url);
                        break;
                    case "SCRIPT" or "FOOTSCRIPT" or "FOOTERSCRIPT":
                        _resourceManager.RegisterUrl(Script, url, url).AtFoot();
                        break;
                    case "HEADSCRIPT" or "HEADERSCRIPT":
                        _resourceManager.RegisterUrl(Script, url, url).AtHead();
                        break;
                    default:
                        _resourceManager.RegisterUrl(type, url, url);
                        break;
                }
            });

        return Completion.Normal;
    }
}
