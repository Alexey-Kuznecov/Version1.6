
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using UnityCommander.Common.Commands;
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Common.Styling
{
    public sealed class MaterialIconSource
     : IIconSource
    {
        private readonly Dictionary<string, PackIconKind> _icons
            = new();

        public int Priority => 0;

        public MaterialIconSource()
        {
            _icons.Add("core.foldertree", PackIconKind.FileTree);
            _icons.Add("core.column", PackIconKind.TableColumn);
            _icons.Add("core.tag", PackIconKind.Tag);
            _icons.Add("core.commnet", PackIconKind.Comment);
            _icons.Add("core.plugins", PackIconKind.Plugin);
            _icons.Add("Settings", PackIconKind.Cog);
            _icons.Add("core.git", PackIconKind.Git);
            _icons.Add("core.sack", PackIconKind.Sack);
            _icons.Add("core.drive", PackIconKind.Scanner);
            _icons.Add("core.file", PackIconKind.File);
            _icons.Add("core.folder", PackIconKind.Folder);

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
            out RuntimeIcon icon)
        {
            if (!_icons.TryGetValue(key, out var kind))
            {
                icon = null!;
                return false;
            }

            icon = CreateIcon(key, kind);
            return true;
        }

        private static RuntimeIcon CreateIcon(string key,
            PackIconKind kind)
        {
            var packIcon = new PackIcon
            {
                Kind = kind
            };

            //var results = Export();

            return new RuntimeIcon
            {
                Key = key,
                Data = packIcon.Data
            };
        }
    }
}
