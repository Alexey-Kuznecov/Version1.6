
using UnityCommander.Abstractions.Resources;
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

        private readonly ILogger _logger;

        public RibbonIconProvider(
            CompositeIconResolver iconResolver, 
            RuntimeIconConverter converter, 
            LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<RibbonIconProvider>(LogScope.Runtime);
            _iconResolver = iconResolver;
            _converter = converter;
        }

        public IconDefinition GetIcon(string iconKey)
        {
            var icon = _iconResolver.Resolve(iconKey);

            return _converter.Convert(icon, iconKey);
        }
    }
}
