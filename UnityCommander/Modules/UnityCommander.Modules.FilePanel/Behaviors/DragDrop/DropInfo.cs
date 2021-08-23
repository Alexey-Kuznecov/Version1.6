
namespace UnityCommander.Modules.FilePanel.Behaviors.DragDrop
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using GongSolutions.Wpf.DragDrop;

    using NJsonSchema.Annotations;

    using UnityCommander.Modules.FilePanel.Behaviors.DragDrop.Utils;
    using UnityCommander.Modules.FilePanel.DragDrop.Utitilies;

    using DragInfo = UnityCommander.Modules.FilePanel.Behaviors.DragInfo;
    using ScrollingMode = UnityCommander.Modules.FilePanel.Behaviors.ScrollingMode;

    /// <summary>
    /// The relative insert position.
    /// </summary>
    [Flags]
    public enum RelativeInsertPosition
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The before target item.
        /// </summary>
        BeforeTargetItem = 1,

        /// <summary>
        /// The after target item.
        /// </summary>
        AfterTargetItem = 2,

        /// <summary>
        /// The target item center.
        /// </summary>
        TargetItemCenter = 4
    }

    /// <summary>
    /// The drop info.
    /// </summary>
    public class DropInfo
    {
        /// <summary>
        /// The item parent.
        /// </summary>
        private ItemsControl itemParent = null;

        /// <summary>
        /// The item.
        /// </summary>
        private UIElement item = null;
        
        /// <summary>
        /// Gets the collection that the target ItemsControl is bound to.
        /// </summary>
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable TargetCollection { get; private set; }

        /// <summary>
        /// Gets the object that the current drop target is bound to.
        /// </summary>
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object TargetItem { get; private set; }

        /// <summary>
        /// Gets the current group target.
        /// </summary>
        /// <remarks>
        /// If the drag is currently over an ItemsControl with groups, describes the group that
        /// the drag is currently over.
        /// </remarks>
        public CollectionViewGroup TargetGroup { get; private set; }

        /// <summary>
        /// Gets the ScrollViewer control for the visual target.
        /// </summary>
        public ScrollViewer TargetScrollViewer { get; private set; }

        /// <summary>
        /// Gets or sets the ScrollingMode for the drop action.
        /// </summary>
        public ScrollingMode TargetScrollingMode { get; set; }

        /// <summary>
        /// Gets the control that is the current drop target.
        /// </summary>
        public UIElement VisualTarget { get; private set; }

        /// <summary>
        /// Gets the item in an ItemsControl that is the current drop target.
        /// </summary>
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public UIElement VisualTargetItem { get; private set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public Orientation VisualTargetOrientation { get; private set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public FlowDirection VisualTargetFlowDirection { get; private set; }

        /// <summary>
        /// Gets or sets the text displayed in the DropDropEffects adorner.
        /// </summary>
        public string DestinationText { get; set; }

        /// <summary>
        /// Gets or sets the effect text displayed in the DropDropEffects adorner.
        /// </summary>
        public string EffectText { get; set; }

        /// <summary>
        /// Gets the relative position the item will be inserted to compared to the TargetItem
        /// </summary>
        public RelativeInsertPosition InsertPosition { get; private set; }

        /// <summary>
        /// Gets a flag enumeration indicating the current state of the SHIFT, CTRL, and ALT keys, as well as the state of the mouse buttons.
        /// </summary>
        public DragDropKeyStates KeyStates { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether not handled.
        /// </summary>
        public bool NotHandled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the target is in the same context as the source, <see cref="DragDrop.DragDropContextProperty" />.
        /// </summary>
        public bool IsSameDragDropContextAsSource
        {
            get
            {
                // Check if DragInfo stuff exists
                if (this.DragInfo == null || this.DragInfo.VisualSource == null)
                {
                    return true;
                }
                
                // A target should be exists
                if (this.VisualTarget == null)
                {
                    return true;
                }

                // Source element has a drag context constraint, we need to check the target property matches.
                //var sourceContext = this.DragInfo.VisualSource.GetValue(DragDrop.DragDropContextProperty) as string;

                //if (string.IsNullOrEmpty(sourceContext))
                //{
                //    return true;
                //}

                //var targetContext = this.VisualTarget.GetValue(DragDrop.DragDropContextProperty) as string;
                //return string.Equals(sourceContext, targetContext);
                return false;
            }
        }

        /// <summary>
        /// Gets the current mode of the underlying routed event.
        /// </summary>
        public EventType EventType { get; }

        /// <summary>
        /// Initializes a new instance of the DropInfo class.
        /// </summary>
        /// <param name="sender">
        /// The sender of the drag event.
        /// </param>
        /// <param name="e">
        /// The drag event.
        /// </param>
        /// <param name="dragInfo">
        /// Information about the source of the drag, if the drag came from within the framework.
        /// </param>
        /// <param name="eventType">
        /// The type of the underlying event (tunneled or bubbled).
        /// </param>
        public DropInfo(object sender, DragEventArgs e, [CanBeNull] DragInfo dragInfo, EventType eventType)
        {
            //this.DragInfo = dragInfo;
            this.KeyStates = e.KeyStates;
            this.EventType = eventType;
            var dataFormat = dragInfo?.DataFormat;
            this.Data = dataFormat != null && e.Data.GetDataPresent(dataFormat.Name) ? e.Data.GetData(dataFormat.Name) : e.Data;

            this.VisualTarget = sender as UIElement;
            
            // if there is no drop target, find another
            if (!this.VisualTarget.IsDropTarget())
            {
                // try to find next element
                var element = this.VisualTarget.TryGetNextAncestorDropTargetElement();
                if (element != null)
                {
                    this.VisualTarget = element;
                }
            }

            if (this.VisualTarget is ItemsControl)
            {
                var itemsControl = (ItemsControl)this.VisualTarget;

                // System.Diagnostics.Debug.WriteLine(">>> Name = {0}", itemsControl.Name);
                // get item under the mouse
                this.item = itemsControl.GetItemContainerAt(this.DropPosition);
                var directlyOverItem = this.item != null;

                this.TargetGroup = itemsControl.FindGroup(this.DropPosition);
                this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                this.VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (this.item == null)
                {
                    // ok, no item found, so maybe we can found an item at top, left, right or bottom
                    this.item = itemsControl.GetItemContainerAt(this.DropPosition, this.VisualTargetOrientation);
                    directlyOverItem = this.DropPosition.DirectlyOverElement(this.item, itemsControl);
                }

                if (this.item == null && this.TargetGroup != null && this.TargetGroup.IsBottomLevel)
                {
                    var itemData = this.TargetGroup.Items.FirstOrDefault();
                    if (itemData != null)
                    {
                        this.item = itemsControl.ItemContainerGenerator.ContainerFromItem(itemData) as UIElement;
                        directlyOverItem = this.DropPosition.DirectlyOverElement(this.item, itemsControl);
                    }
                }

                if (this.item != null)
                {
                    this.itemParent = ItemsControl.ItemsControlFromItemContainer(this.item);
                    this.VisualTargetOrientation = this.itemParent.GetItemsPanelOrientation();
                    this.VisualTargetFlowDirection = this.itemParent.GetItemsPanelFlowDirection();

                    this.InsertIndex = this.itemParent.ItemContainerGenerator.IndexFromContainer(this.item);
                    this.TargetCollection = this.itemParent.ItemsSource ?? this.itemParent.Items;

                    var tvItem = (TreeViewItem)this.item;

                    if (directlyOverItem || tvItem != null)
                    {
                        this.VisualTargetItem = this.item;
                        this.TargetItem = this.itemParent.ItemContainerGenerator.ItemFromContainer(this.item);
                    }

                    var expandedTVItem = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;
                    var itemRenderSize = expandedTVItem ? tvItem.GetHeaderSize() : this.item.RenderSize;

                    if (this.VisualTargetOrientation == Orientation.Vertical)
                    {
                        var currentYPos = e.GetPosition(this.item).Y;
                        var targetHeight = itemRenderSize.Height;

                        var topGap = targetHeight * 0.25;
                        var bottomGap = targetHeight * 0.75;
                        if (currentYPos > targetHeight / 2)
                        {
                            if (expandedTVItem && (currentYPos < topGap || currentYPos > bottomGap))
                            {
                                this.VisualTargetItem = tvItem.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
                                this.TargetItem = this.VisualTargetItem != null ? tvItem.ItemContainerGenerator.ItemFromContainer(this.VisualTargetItem) : null;
                                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                this.InsertIndex = 0;
                                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                            else
                            {
                                this.InsertIndex++;
                                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                        }
                        else
                        {
                            this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (currentYPos > topGap && currentYPos < bottomGap)
                        {
                            if (tvItem != null)
                            {
                                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                this.InsertIndex = this.TargetCollection?.OfType<object>().Count() ?? 0;
                            }
                            
                            this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                    }
                    else
                    {
                        var currentXPos = e.GetPosition(this.item).X;
                        var targetWidth = itemRenderSize.Width;

                        if (this.VisualTargetFlowDirection == FlowDirection.RightToLeft)
                        {
                            if (currentXPos > targetWidth / 2)
                            {
                                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                            else
                            {
                                this.InsertIndex++;
                                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                        }
                        else if (this.VisualTargetFlowDirection == FlowDirection.LeftToRight)
                        {
                            if (currentXPos > targetWidth / 2)
                            {
                                this.InsertIndex++;
                                this.InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                            else
                            {
                                this.InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                        }

                        if (currentXPos > targetWidth * 0.25 && currentXPos < targetWidth * 0.75)
                        {
                            if (tvItem != null)
                            {
                                this.TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                this.InsertIndex = this.TargetCollection?.OfType<object>().Count() ?? 0;
                            }

                            this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                    }
                }
                else
                {
                    this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    this.InsertIndex = itemsControl.Items.Count;
                }
            }
            else
            {
                this.VisualTargetItem = this.VisualTarget;
            }
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <remarks>
        /// If the drag came from within the framework, this will hold:
        /// - The dragged data if a single item was dragged.
        /// - A typed IEnumerable if multiple items were dragged.
        /// </remarks>
        public object Data { get; private set; }

        /// <summary>
        /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
        /// if the drag came from within the framework.
        /// </summary>
        public IDragInfo DragInfo { get; private set; }

        /// <summary>
        /// Gets the mouse position relative to the VisualTarget
        /// </summary>
        public Point DropPosition { get; private set; }

        /// <summary>
        /// Gets or sets the class of drop target to display.
        /// </summary>
        /// <remarks>
        /// The standard drop target adorner classes are held in the <see cref="DropTargetAdorners"/>
        /// class.
        /// </remarks>
        public Type DropTargetAdorner { get; set; }

        /// <summary>
        /// Gets or sets the allowed effects for the drop.
        /// </summary>
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drop handler in order 
        /// for a drop to be possible.
        /// </remarks>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets the current insert position within <see cref="TargetCollection"/>.
        /// </summary>
        public int InsertIndex { get; private set; }

        /// <summary>
        /// Gets the current insert position within the source (unfiltered) <see cref="TargetCollection"/>.
        /// </summary>
        /// <remarks>
        /// This should be only used in a Drop action.
        /// This works only correct with different objects (string, int, etc won't work correct).
        /// </remarks>
        public int UnfilteredInsertIndex
        {
            get
            {
                var insertIndex = this.InsertIndex;
                if (this.itemParent != null)
                {
                    var itemSourceAsList = this.itemParent.ItemsSource.TryGetList();
                    if (itemSourceAsList != null && this.itemParent.Items != null && this.itemParent.Items.Count != itemSourceAsList.Count)
                    {
                        if (insertIndex >= 0 && insertIndex < this.itemParent.Items.Count)
                        {
                            var indexOf = itemSourceAsList.IndexOf(this.itemParent.Items[insertIndex]);
                            if (indexOf >= 0)
                            {
                                return indexOf;
                            }
                        }
                        else if (this.itemParent.Items.Count > 0 && insertIndex == this.itemParent.Items.Count)
                        {
                            var indexOf = itemSourceAsList.IndexOf(this.itemParent.Items[insertIndex - 1]);
                            if (indexOf >= 0)
                            {
                                return indexOf + 1;
                            }
                        }
                    }
                }
                return insertIndex;
            }
        }
    }
}
