
using System;
using System.Windows.Media;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Rendering.Converters;
using UnityCommander.Ribbon.Icon;
using UnityCommander.Ribbon.Services.Icon;

namespace UnityCommander.Modules.ToolBar.Builder
{
    public sealed class RibbonIconProvider : IRibbonIconProvider
    {
        private readonly CompositeIconResolver _iconResolver;
        
        private readonly ILogger _logger;

        public RibbonIconProvider(CompositeIconResolver iconResolver, LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<RibbonIconProvider>(LogScope.Runtime);
            _iconResolver = iconResolver;
        }

        public IconDefinition GetIcon(string iconKey)
        {
            var icon = _iconResolver.Resolve(iconKey);

            if (string.IsNullOrWhiteSpace(icon.Data))
            {
                _logger.Info(
                    $"Icon '{iconKey}' contains no geometry data.");
                return null;
            }

            var geometry = Geometry.Parse(icon.Data);

            return new IconDefinition(
                iconKey,
                geometry,
                true,
                BrushColorHelper.StringFormatToSolidColor(icon.Color ?? "#FF000000"));
        }
    }
}
