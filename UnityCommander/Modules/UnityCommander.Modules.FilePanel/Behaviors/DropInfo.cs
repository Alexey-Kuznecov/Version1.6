
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    using NJsonSchema.Annotations;
    using UnityCommander.Modules.FilePanel.Behaviors.DragDrop;
    using UnityCommander.Modules.FilePanel.Behaviors.DragDrop.Utils;
    using UnityCommander.Modules.FilePanel.DragDrop.Utitilies;

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
        /// Initializes a new instance of the <see cref="DropInfo"/> class.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="dragInfo">
        /// The drag info.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        public DropInfo(object sender, DragEventArgs e, [CanBeNull] DragInfo dragInfo, EventType eventType)
        {
            this.DragInfo = dragInfo;
            
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

            // try find ScrollViewer
            //var dropTargetScrollViewer = DragDrop.GetDropTargetScrollViewer(this.VisualTarget);
            //if (dropTargetScrollViewer != null)
            //{
            //    this.TargetScrollViewer = dropTargetScrollViewer;
            //}
            if (this.VisualTarget is TabControl)
            {
                var tabPanel = this.VisualTarget.GetVisualDescendent<TabPanel>();
                this.TargetScrollViewer = tabPanel?.GetVisualAncestor<ScrollViewer>();
            }
            else
            {
                this.TargetScrollViewer = this.VisualTarget?.GetVisualDescendent<ScrollViewer>();
            }

            this.TargetScrollingMode = ScrollingMode.Both;

            // visual target can be null, so give us a point...
            this.DropPosition = this.VisualTarget != null ? e.GetPosition(this.VisualTarget) : new Point();

            if (this.VisualTarget is TabControl)
            {
                if (!HitTestUtilities.HitTest4Type<TabPanel>(this.VisualTarget, this.DropPosition))
                {
                    return;
                }
            }

            if (this.VisualTarget is ItemsControl)
            {
                var itemsControl = (ItemsControl)this.VisualTarget;
                //System.Diagnostics.Debug.WriteLine(">>> Name = {0}", itemsControl.Name);
                // get item under the mouse
                item = itemsControl.GetItemContainerAt(this.DropPosition);
                var directlyOverItem = item != null;

                this.TargetGroup = itemsControl.FindGroup(this.DropPosition);
                this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                this.VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (item == null)
                {
                    // ok, no item found, so maybe we can found an item at top, left, right or bottom
                    item = itemsControl.GetItemContainerAt(this.DropPosition, this.VisualTargetOrientation);
                    directlyOverItem = DropPosition.DirectlyOverElement(this.item, itemsControl);
                }

                if (item == null && this.TargetGroup != null && this.TargetGroup.IsBottomLevel)
                {
                    var itemData = this.TargetGroup.Items.FirstOrDefault();
                    if (itemData != null)
                    {
                        item = itemsControl.ItemContainerGenerator.ContainerFromItem(itemData) as UIElement;
                        directlyOverItem = DropPosition.DirectlyOverElement(this.item, itemsControl);
                    }
                }

                if (item != null)
                {
                    itemParent = ItemsControl.ItemsControlFromItemContainer(item);
                    this.VisualTargetOrientation = itemParent.GetItemsPanelOrientation();
                    this.VisualTargetFlowDirection = itemParent.GetItemsPanelFlowDirection();

                    this.InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    this.TargetCollection = itemParent.ItemsSource ?? itemParent.Items;

                    var tvItem = item as TreeViewItem;

                    if (directlyOverItem || tvItem != null)
                    {
                        this.VisualTargetItem = item;
                        this.TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    }

                    var expandedTVItem = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;
                    var itemRenderSize = expandedTVItem ? tvItem.GetHeaderSize() : item.RenderSize;

                    if (this.VisualTargetOrientation == Orientation.Vertical)
                    {
                        var currentYPos = e.GetPosition(item).Y;
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
                                this.InsertIndex = this.TargetCollection != null ? this.TargetCollection.OfType<object>().Count() : 0;
                            }

                            this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                        //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, idx={1}, Y={2}, Item={3}", this.InsertPosition, this.InsertIndex, currentYPos, item);
                    }
                    else
                    {
                        var currentXPos = e.GetPosition(item).X;
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
                                this.InsertIndex = this.TargetCollection != null ? this.TargetCollection.OfType<object>().Count() : 0;
                            }
                            this.InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                        //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, idx={1}, X={2}, Item={3}", this.InsertPosition, this.InsertIndex, currentXPos, item);
                    }
                }
                else
                {
                    this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    this.InsertIndex = itemsControl.Items.Count;
                    //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, item=NULL, idx={1}", this.InsertPosition, this.InsertIndex);
                }
            }
            else
            {
                this.VisualTargetItem = this.VisualTarget;
            }
        }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the visual target item.
        /// </summary>
        public UIElement VisualTargetItem { get; set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Gets or sets the visual target.
        /// </summary>
        public UIElement VisualTarget { get; set; }

        /// <summary>
        /// Gets the target collection.
        /// </summary>
        public IEnumerable TargetCollection { get; }

        /// <summary>
        /// Gets or sets the insert index.
        /// </summary>
        public int InsertIndex { get; set; }

        /// <summary>
        /// Gets collection group If the drag is currently over an ItemsControl with groups, describes the group that
        /// the drag is currently over.
        /// </summary>
        public CollectionViewGroup TargetGroup { get; internal set; }

        /// <summary>
        /// Gets or sets the target item.
        /// </summary>
        public object TargetItem { get; set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public Orientation VisualTargetOrientation { get; private set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public FlowDirection VisualTargetFlowDirection { get; private set; }

        /// <summary>
        /// Gets or sets the insert position.
        /// </summary>
        public RelativeInsertPosition InsertPosition { get; set; }

        /// <summary>
        /// Gets the drag info.
        /// </summary>
        public DragInfo DragInfo { get; }

        /// <summary>
        /// Gets the key states.
        /// </summary>
        public DragDropKeyStates KeyStates { get; }

        /// <summary>
        /// Gets or Sets the ScrollingMode for the drop action.
        /// </summary>
        public ScrollingMode TargetScrollingMode { get; set; }

        /// <summary>
        /// Gets the target scroll viewer.
        /// </summary>
        public ScrollViewer TargetScrollViewer { get; }
        
        /// <summary>
        /// Gets the mouse position relative to the VisualTarget
        /// </summary>
        public Point DropPosition { get; private set; }
    }
}
