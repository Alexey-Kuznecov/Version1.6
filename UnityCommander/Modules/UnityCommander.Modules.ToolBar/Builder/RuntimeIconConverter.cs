
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
            {
                return null;
            }

            using var trace = _trace.Begin(
                "ribbon.icon.converter",
                "convert",
                DiagnosticTraceData.Of(
                    ("key", key),
                    ("type", icon.IconType),
                    ("layerCount", icon.Layers.Count)));

            try
            {
                var layers = new List<IconLayer>();
                var skippedLayerCount = 0;

                for (var index = 0; index < icon.Layers.Count; index++)
                {
                    var layer = icon.Layers[index];

                    if (layer is null)
                    {
                        skippedLayerCount++;

                        _logger.Warning(
                            $"Icon '{key}' contains null layer at index {index}.");

                        trace.Write(
                            "layer.skipped",
                            DiagnosticTraceData.Of(
                                ("index", index),
                                ("reason", "Layer is null")));

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
                            skippedLayerCount++;

                            _logger.Warning(
                                $"Icon '{key}' contains empty geometry at layer {index}.");

                            trace.Write(
                                "layer.skipped",
                                DiagnosticTraceData.Of(
                                    ("index", index),
                                    ("reason", "Empty geometry")));

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
                        skippedLayerCount++;

                        _logger.Error(
                            $"Failed to convert layer {index} of icon '{key}'.",
                            ex);

                        trace.Write(
                            "layer.failed",
                            DiagnosticTraceData.Of(
                                ("index", index),
                                ("exception", ex.GetType().FullName),
                                ("message", ex.Message)));

                        continue;
                    }
                }

                if (layers.Count == 0)
                {
                    trace.Fail(
                        new InvalidOperationException(
                            "Icon contains no valid layers."),
                        DiagnosticTraceData.Of(
                            ("inputLayerCount", icon.Layers.Count),
                            ("skippedLayerCount", skippedLayerCount)));

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
                        ("layerCount", result.Layers.Count),
                        ("skippedLayerCount", skippedLayerCount)));

                trace.Complete();

                return result;
            }
            catch (Exception ex)
            {
                trace.Fail(ex);
                throw;
            }
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
