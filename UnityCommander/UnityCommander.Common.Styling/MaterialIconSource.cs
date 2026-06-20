
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Common.Commands;

namespace UnityCommander.Common.Styling
{
    public sealed class MaterialIconSource
     : IIconSource
    {
        private readonly Dictionary<string, PackIconKind> _icons
            = new();

        public MaterialIconSource()
        {
            _icons.Add("FileTree", PackIconKind.FileTree);
            _icons.Add("TableColumn", PackIconKind.TableColumn);
            _icons.Add("Tag", PackIconKind.Tag);
            _icons.Add("Comment", PackIconKind.Comment);
            _icons.Add("Plugin", PackIconKind.Plugin);
            _icons.Add("Settings", PackIconKind.Settings);
            _icons.Add("Git", PackIconKind.Git);
            _icons.Add("Sack", PackIconKind.Sack);

            _icons.Add(
                CommandNames.Navigation.Drives,
                PackIconKind.LaptopWindows);

            _icons.Add(
                CommandNames.Navigation.Goto,
                PackIconKind.Arrow);

            _icons.Add(
                CommandNames.Navigation.Refresh,
                PackIconKind.Refresh);

            _icons.Add(
                CommandNames.Navigation.Back,
                PackIconKind.ArrowBack);

            _icons.Add(
                CommandNames.Navigation.Forward,
                PackIconKind.ArrowForward);
        }

        public bool TryGet(
            string key,
            out IconDefinition icon)
        {
            if (!_icons.TryGetValue(key, out var kind))
            {
                icon = null!;
                return false;
            }

            icon = CreateIcon(kind);

            return true;
        }

        private static IconDefinition CreateIcon(
            PackIconKind kind)
        {
            var packIcon = new PackIcon
            {
                Kind = kind
            };

            return new IconDefinition
            {
                Key = kind.ToString(),
                Data = packIcon.Data
            };
        }
    }
}
