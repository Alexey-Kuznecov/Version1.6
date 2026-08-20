
using System;
using System.Collections.Generic;
using System.IO;

namespace UnityCommander.Controls.Navigation
{
    internal sealed class NavigationPathParser
    {
        public NavigationPath Parse(string path)
        {
            var items = new List<NavigationPathItem>();

            var root = Path.GetPathRoot(path);

            if (root == null)
                return new NavigationPath(items);

            var currentPath = root;

            items.Add(
                new NavigationPathItem(
                    root,
                    root,
                    root));

            var relativePath = path[root.Length..];

            foreach (var part in relativePath.Split(
                '\\',
                StringSplitOptions.RemoveEmptyEntries))
            {
                var parentPath = currentPath;

                currentPath = Path.Combine(
                    currentPath,
                    part);

                items.Add(
                    new NavigationPathItem(
                        part,
                        currentPath,
                        parentPath));
            }

            return new NavigationPath(items);
        }
    }
}
