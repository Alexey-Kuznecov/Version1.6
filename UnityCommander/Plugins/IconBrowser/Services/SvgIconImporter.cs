
using IconMaker.Core.Models;

namespace IconBrowser.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public sealed class SvgIconImporter : IIconImporter
    {
        public IconDefinition Import(string path, string name)
        {
            var document = XDocument.Load(path);

            var root = document.Root
                ?? throw new InvalidDataException(
                    "SVG document does not contain a root element.");

            if (!root.Name.LocalName.Equals("svg", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Root element is not <svg>.");

            var layers = new List<IconLayer>();

            // Атрибуты <svg> являются значениями по умолчанию
            // для всех дочерних элементов.
            var svgAttributes = ReadAttributes(root);

            foreach (var element in root.Descendants())
            {
                if (!element.Name.LocalName.Equals(
                        "path",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Наследуем атрибуты SVG, затем перекрываем
                // их атрибутами конкретного path.
                var attributes = new Dictionary<string, string>(
                    svgAttributes,
                    StringComparer.OrdinalIgnoreCase);

                foreach (var pair in ReadAttributes(element))
                    attributes[pair.Key] = pair.Value;

                if (!attributes.TryGetValue("d", out var geometry))
                    continue;

                if (IsEmptyPath(element, attributes))
                    continue;

                layers.Add(new IconLayer
                {
                    Geometry = geometry,

                    Fill = ResolveFill(attributes),
                    Stroke = ResolveStroke(attributes),

                    StrokeWidth = GetDouble(
                      attributes,
                      "stroke-width"),

                    StrokeLineCap = GetValue(
                      attributes,
                      "stroke-linecap"),

                    StrokeLineJoin = GetValue(
                      attributes,
                      "stroke-linejoin"),

                    Order = layers.Count
                });
            }

            return new IconDefinition
            {
                Id = Guid.NewGuid(),
                Name = name,
                Scale = 1,
                Background = "transparent",
                Foreground = "#FFFFFF",
                Tags = null,
                Layers = layers
            };
        }

        private static string? ResolveFill(
            Dictionary<string, string> attributes)
        {
            var value = GetValue(attributes, "fill");

            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return value;
        }

        private static string? ResolveStroke(
            Dictionary<string, string> attributes)
        {
            var value = GetValue(attributes, "stroke");

            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return value;
        }

        private static Dictionary<string, string> ReadAttributes(XElement element)
        {
            var result = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var attribute in element.Attributes())
            {
                if (attribute.IsNamespaceDeclaration)
                    continue;

                var name = attribute.Name.LocalName;

                if (name.Equals("style", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var style in ParseStyle(attribute.Value))
                        result[style.Key] = style.Value;

                    continue;
                }

                result[name] = attribute.Value;
            }

            return result;
        }

        private static Dictionary<string, string> ParseStyle(string style)
        {
            var result = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var declaration in style.Split(';'))
            {
                var separator = declaration.IndexOf(':');

                if (separator <= 0)
                    continue;

                var name = declaration[..separator].Trim();
                var value = declaration[(separator + 1)..].Trim();

                if (name.Length == 0)
                    continue;

                result[name] = value;
            }

            return result;
        }

        private static string? GetValue(
            IReadOnlyDictionary<string, string> attributes,
            string name)
        {
            return attributes.TryGetValue(name, out var value)
                ? value
                : null;
        }

        private static double? GetDouble(
            IReadOnlyDictionary<string, string> attributes,
            string name)
        {
            if (!attributes.TryGetValue(name, out var value))
                return null;

            return double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var result)
                    ? result
                    : null;
        }

        private static bool IsEmptyPath(
            XElement element,
            IReadOnlyDictionary<string, string> attributes)
        {
            var stroke = GetValue(attributes, "stroke");
            var fill = GetValue(attributes, "fill");

            return string.Equals(stroke, "none", StringComparison.OrdinalIgnoreCase)
                && string.Equals(fill, "none", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeFill(string? fill)
        {
            if (string.IsNullOrWhiteSpace(fill) ||
                fill.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                fill.Equals("currentColor", StringComparison.OrdinalIgnoreCase))
            {
                return "#000000";
            }

            return fill;
        }
    }
}
