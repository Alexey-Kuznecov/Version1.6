
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Controls.Layout
{
    public sealed class LayoutEngine
    {
        public Dictionary<LayoutNode, Rect> LayoutMap { get; } = new();

        public void Measure(LayoutNode node, Rect bounds)
        {
            LayoutMap.Clear();
            MeasureNode(node, bounds);
        }

        private void MeasureNode(LayoutNode node, Rect bounds)
        {
            if (node == null)
                return;

            switch (node)
            {
                case FixedNode fixedNode:
                    MeasureFixed(fixedNode, bounds);
                    break;

                case SplitNode splitNode:
                    MeasureSplit(splitNode, bounds);
                    break;

                case StackNode stackNode:
                    MeasureStack(stackNode, bounds);
                    break;

                case RegionNode regionNode:
                    MeasureRegion(regionNode, bounds);
                    break;
            }
        }

        private void MeasureFixed(FixedNode node, Rect bounds)
        {
            var rect = new Rect(
                bounds.X,
                bounds.Y,
                bounds.Width,
                node.Size
            );

            LayoutMap[node] = rect;

            if (node.Content is LayoutNode child)
                MeasureNode(child, rect);
        }

        private void MeasureSplit(SplitNode node, Rect bounds)
        {
            if (node.Orientation == Orientation.Vertical)
            {
                double firstH = bounds.Height * node.Ratio;
                double secondH = bounds.Height - firstH;

                var r1 = new Rect(bounds.X, bounds.Y, bounds.Width, firstH);
                var r2 = new Rect(bounds.X, bounds.Y + firstH, bounds.Width, secondH);

                LayoutMap[node.First] = r1;
                LayoutMap[node.Second] = r2;

                MeasureNode(node.First, r1);
                MeasureNode(node.Second, r2);
            }
            else
            {
                double firstW = bounds.Width * node.Ratio;
                double secondW = bounds.Width - firstW;

                var r1 = new Rect(bounds.X, bounds.Y, firstW, bounds.Height);
                var r2 = new Rect(bounds.X + firstW, bounds.Y, secondW, bounds.Height);

                LayoutMap[node.First] = r1;
                LayoutMap[node.Second] = r2;

                MeasureNode(node.First, r1);
                MeasureNode(node.Second, r2);
            }
        }

        private void MeasureStack(StackNode node, Rect bounds)
        {
            double y = bounds.Y;

            double totalFlex = 0;
            double usedFixed = 0;

            foreach (var child in node.Children)
            {
                if (child.Size.Type == SizeType.Flex)
                    totalFlex += child.Size.Flex;
                else if (child.Size.Type == SizeType.Fixed)
                    usedFixed += child.Size.Value;
            }

            double free = bounds.Height - usedFixed;

            foreach (var child in node.Children)
            {
                double h;

                if (child.Size.Type == SizeType.Fixed)
                    h = child.Size.Value;
                else
                    h = free * (child.Size.Flex / Math.Max(totalFlex, 1));

                var rect = new Rect(bounds.X, y, bounds.Width, h);

                LayoutMap[child] = rect;

                MeasureNode(child, rect);

                y += h;
            }
        }

        private void MeasureRegion(RegionNode node, Rect bounds)
        {
            LayoutMap[node] = bounds;
        }
    }
}
