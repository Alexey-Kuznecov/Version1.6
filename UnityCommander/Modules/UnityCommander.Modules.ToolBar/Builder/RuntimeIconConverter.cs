
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Rendering.Converters;
using UnityCommander.Ribbon.Services.Icon;

namespace UnityCommander.Modules.ToolBar.Builder
{
    public sealed class RuntimeIconConverter
    {
        readonly IDiagnosticTrace _trace;

        public RuntimeIconConverter(IDiagnosticTrace trace)
        {
            _trace = trace;
        }

        public IconDefinition Convert(RuntimeIcon icon, string key)
        {
            using var trace = _trace.Begin(
                "ribbon.icon.converter",
                "convert",
                DiagnosticTraceData.Of(
                    ("key", key),
                    ("type", icon.IconType),
                    ("layerCount", icon.Layers.Count)));

            var layers = icon.Layers
                .Select((layer, index) =>
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

                    return new IconLayer
                    {
                        Geometry = Geometry.Parse(layer.Data),
                        Fill = ResolveBrush(layer.Fill),
                        Stroke = ResolveBrush(layer.Stroke),
                        //StrokeWidth = layer.StrokeWidth,
                        //StrokeLineCap = layer.StrokeLineCap,
                        //StrokeLineJoin = layer.StrokeLineJoin,
                        Order = index
                    };
                })
                .ToList();

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
