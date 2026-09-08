
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Rendering.Converters;
using UnityCommander.Rendering.Icons;
using UnityCommander.Ribbon.Services.Icon;

namespace UnityCommander.Modules.ToolBar.Builder
{
    public sealed class RuntimeIconConverter
    {
        readonly IDiagnosticTrace _trace;
        readonly ILogger _logger;

        public RuntimeIconConverter(IDiagnosticTrace trace, LoggerCreator loggerCreator)
        {
            _trace = trace;
            _logger = loggerCreator.For<RuntimeIconConverter>(LogScope.Runtime);
        }

        public IconDefinition? Convert(RuntimeIcon? icon, string key)
        {
            if (icon is null || string.IsNullOrWhiteSpace(key))
                return null;

            using var trace = _trace.Begin(
                "ribbon.icon.converter",
                "convert",
                DiagnosticTraceData.Of(
                    ("key", key),
                    ("type", icon.IconType),
                    ("layerCount", icon.Layers.Count)));

            var layers = new List<IconLayer>();

            for (var index = 0; index < icon.Layers.Count; index++)
            {
                var layer = icon.Layers[index];

                if (layer is null)
                {
                    _logger.Warning(
                        $"Icon '{key}' contains null layer at index {index}.");

                    continue;
                }

                try
                {
                    trace.Write(
                        "layer.input",
                        DiagnosticTraceData.Of(
                            ("index", index),
                            ("fill", layer.Fill),
                            ("stroke", layer.Stroke),
                            ("strokeWidth", layer.StrokeWidth),
                            ("lineCap", layer.StrokeLineCap),
                            ("lineJoin", layer.StrokeLineJoin)));

                    if (string.IsNullOrWhiteSpace(layer.Data))
                    {
                        _logger.Warning(
                            $"Icon '{key}' contains empty geometry at layer {index}.");

                        continue;
                    }

                    layers.Add(new IconLayer
                    {
                        Geometry = Geometry.Parse(layer.Data),
                        Fill = ResolveBrush(layer.Fill),
                        Stroke = ResolveBrush(layer.Stroke),
                        Order = index
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        $"Failed to convert layer {index} of icon '{key}'.", ex);
                }
            }

            if (layers.Count == 0)
            {
                _logger.Warning(
                    $"Icon '{key}' contains no valid layers.");

                return null;
            }

            var result = new IconDefinition(
                key,
                290,
                200,
                layers);

            trace.Write(
                "result.created",
                DiagnosticTraceData.Of(
                    ("layerCount", result.Layers.Count)));

            trace.Complete();

            return result;
        }
        private static Brush? ResolveBrush(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals("none", StringComparison.OrdinalIgnoreCase))
                return null;

            if (value.Equals("currentColor", StringComparison.OrdinalIgnoreCase))
                return null;

            return BrushColorHelper.StringFormatToSolidColor(value);
        }
    }
}
