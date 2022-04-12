using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Controls.Ribbon.Control;

namespace UnityCommander.Controls.Ribbon.Subgroup
{
    public class ControlsStackGroup : StackPanel
    {
        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        public ControlsStackGroup()
        {
            this.Margin = new Thickness(10, 5, 0, 0);
            this.AllowDrop = true;
            this.Drop += sb_OnPreviewDrop;
            this.DragEnter += sb_OnPreviewDragEnter;
        }

        #region Override Members

        public override void OnApplyTemplate()
        {
            this.Margin = new Thickness(10, 5, 0, 0);
            base.OnApplyTemplate();
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="arrangeBounds">
        /// The arrange bounds.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        //protected override Size ArrangeOverride(Size arrangeBounds)
        //{
        //    foreach (UIElement child in this.Children)
        //    {
        //        child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
        //    }

        //    return arrangeBounds;
        //}

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    Size size = new Size(170, 125);
        //    foreach (UIElement child in this.Children)
        //    {
        //        child.Measure(size);
        //    }

        //    return new Size(0, 0);
        //}

        #endregion

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source == this)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(this);
            }

            //base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            _realDragSource.ReleaseMouseCapture();
            //base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
            //base.OnPreviewMouseMove(e);
        }

        protected void sb_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }

            //object data;
            ////var dataObj = e.Data as DataObject;
            ////var dragged = dataObj.GetData(typeof(UnityCommander.Common.Models.Directory.FolderModel));
            //foreach (string format in e.Data.GetFormats(true))
            //{
            //    data = e.Data.GetData(format); //Returns null
            //}

            //e.Effects = DragDropEffects.Copy;
            base.OnPreviewDragEnter(e);
        }

        protected void sb_OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                UIElement droptarget = e.Source as UIElement;
                int droptargetIndex = -1, i = 0;
                foreach (UIElement element in this.Children)
                {
                    if (element.Equals(droptarget))
                    {
                        droptargetIndex = i;
                        break;
                    }
                    i++;
                }
                if (droptargetIndex != -1)
                {
                    this.Children.Remove(_realDragSource);
                    
                    if (_realDragSource != null)
                        this.Children.Insert(droptargetIndex, _realDragSource);
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
            }

            base.OnPreviewDrop(e);
        }
    }
}
