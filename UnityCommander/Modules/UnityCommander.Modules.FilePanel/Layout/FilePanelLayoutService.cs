
using System;
using System.Windows.Controls;
using UnityCommander.Controls.Layout;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public sealed class FilePanelLayoutService
    {
        public LayoutNode Build(
            FilePanelLayoutMode mode,
            RegionNode header,
            RegionNode main,
            RegionNode secondary,
            RegionNode? unified = null)
        {
            return mode switch
            {
                FilePanelLayoutMode.Split =>
                    BuildSplit(header, main, secondary),

                FilePanelLayoutMode.Unified =>
                    BuildUnified(header, unified ?? main),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private static LayoutNode BuildSplit(
            RegionNode header,
            RegionNode main,
            RegionNode secondary)
        {
            return new StackNode
            {
                Orientation = Orientation.Horizontal,
                Children =
            {
                new FixedNode
                {
                    Size = 25,
                    Content = header
                },

                new SplitNode
                {
                    Orientation = Orientation.Vertical,
                    Ratio = 0.5,
                    First = main,
                    Second = secondary
                }
            }
            };
        }

        private static LayoutNode BuildUnified(
            RegionNode header,
            RegionNode content)
        {
            return new StackNode
            {
                Orientation = Orientation.Horizontal,
                Children =
            {
                new FixedNode
                {
                    Size = 25,
                    Content = header
                },

                content
            }
            };
        }
    }
}
