
using UnityCommander.Abstractions.Resources;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Ribbon.Services.Icon;

namespace UnityCommander.Modules.ToolBar.Builder
{
    public sealed class RibbonIconProvider : IRibbonIconProvider
    {
        private readonly CompositeIconResolver _iconResolver;
        private readonly RuntimeIconConverter _converter; 
        private readonly IDiagnosticTrace _trace;
        private readonly ILogger _logger;

        public RibbonIconProvider(
            CompositeIconResolver iconResolver, 
            RuntimeIconConverter converter, 
            LoggerCreator loggerCreator, 
            IDiagnosticTrace trace)
        {
            _trace = trace;
            _logger = loggerCreator.For<RibbonIconProvider>(LogScope.Runtime);
            _iconResolver = iconResolver;
            _converter = converter;
        }

        public IconDefinition GetIcon(string iconKey)
        {
            using var trace = _trace.Begin(
                "ribbon.icon.provider",
                "resolve",
                DiagnosticTraceData.Of(
                    ("key", iconKey)));

            var icon = _iconResolver.Resolve(iconKey);

            trace.Write(
                "resolved",
                DiagnosticTraceData.Of(
                    ("found", icon != null),
                    ("type", icon?.IconType),
                    ("layerCount", icon?.Layers.Count ?? 0)));

            var result = _converter.Convert(icon, iconKey);

            trace.Write(
                "converted",
                DiagnosticTraceData.Of(
                    ("layerCount", result.Layers.Count)));

            trace.Complete();

            return result;
        }
    }
}
