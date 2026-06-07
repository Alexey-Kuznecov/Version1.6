
using System;
using System.Windows;
using UnityCommander.Core.DragDrop;

namespace UnityCommander.Core.Behaviors
{
    public sealed class DragDropResult
    {
        public bool IsAllowed { get; init; }

        public DragDropEffects Effect { get; init; }

        public Type? Adorner { get; init; }

        public string? Message { get; init; }

        public static DragDropResult AllowCopy()
        {
            return new()
            {
                IsAllowed = true,
                Effect = DragDropEffects.Copy,
                Adorner = DropTargetAdorners.Highlight
            };
        }

        public static DragDropResult Deny()
        {
            return new()
            {
                IsAllowed = false,
                Effect = DragDropEffects.None
            };
        }
    }
}
