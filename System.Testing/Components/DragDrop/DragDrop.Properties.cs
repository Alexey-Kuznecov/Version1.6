
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Components.DragDrop
{
    /// <summary>
    /// The drag drop.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1116")]
    public static partial class DragDrop
    {
        #region Default Drag Adorner Opacity

        /// <summary>
        /// Gets or Sets the opacity of the default DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DefaultDragAdornerOpacityProperty
            = DependencyProperty.RegisterAttached("DefaultDragAdornerOpacity",
                typeof(double),
                typeof(DragDrop),
                new PropertyMetadata(0.8));

        /// <summary>
        /// Gets the opacity of the default DragAdorner.
        /// </summary>
        public static double GetDefaultDragAdornerOpacity(UIElement target)
        {
            return (double)target.GetValue(DefaultDragAdornerOpacityProperty);
        }

        /// <summary>
        /// Sets the opacity of the default DragAdorner.
        /// </summary>
        public static void SetDefaultDragAdornerOpacity(UIElement target, double value)
        {
            target.SetValue(DefaultDragAdornerOpacityProperty, value);
        }

        #endregion

        #region Drag Mouse Anchor Point

        /// <summary>
        /// Gets or Sets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragMouseAnchorPointProperty
            = DependencyProperty.RegisterAttached("DragMouseAnchorPoint",
                typeof(Point),
                typeof(DragDrop),
                new PropertyMetadata(new Point(0, 1)));

        /// <summary>
        /// Gets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
        /// </summary>
        public static Point GetDragMouseAnchorPoint(UIElement target)
        {
            return (Point)target.GetValue(DragMouseAnchorPointProperty);
        }

        /// <summary>
        /// Sets the horizontal and vertical proportion at which the pointer will anchor on the DragAdorner.
        /// </summary>
        public static void SetDragMouseAnchorPoint(UIElement target, Point value)
        {
            target.SetValue(DragMouseAnchorPointProperty, value);
        }

        #endregion

        #region Drag Adorner Translation

        /// <summary>
        /// Gets or Sets the translation transform which will be used for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTranslationProperty
            = DependencyProperty.RegisterAttached("DragAdornerTranslation",
                typeof(Point),
                typeof(DragDrop),
                new PropertyMetadata(new Point(-4, -4)));

        /// <summary>
        /// Gets the translation transform which will be used for the DragAdorner.
        /// </summary>
        public static Point GetDragAdornerTranslation(UIElement element)
        {
            return (Point)element.GetValue(DragAdornerTranslationProperty);
        }

        /// <summary>
        /// Sets the translation transform which will be used for the DragAdorner.
        /// </summary>
        public static void SetDragAdornerTranslation(UIElement element, Point value)
        {
            element.SetValue(DragAdornerTranslationProperty, value);
        }

        #endregion

        #region Drop Adorner Template

        /// <summary>
        /// Gets or Sets a DataTemplate for the DragAdorner based on the DropTarget.
        /// </summary>
        public static readonly DependencyProperty DropAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("DropAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDrop));

        /// <summary>
        /// Gets the DataTemplate for the DragAdorner based on the DropTarget.
        /// </summary>
        public static DataTemplate GetDropAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DropAdornerTemplateProperty);
        }

        /// <summary>
        /// Sets the DataTemplate for the DragAdorner based on the DropTarget.
        /// </summary>
        public static void SetDropAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DropAdornerTemplateProperty, value);
        }

        #endregion

        #region Drag Adorner Template

        /// <summary>
        /// Gets or Sets a DataTemplate for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateProperty
            = DependencyProperty.RegisterAttached("DragAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDrop));

        /// <summary>
        /// Gets the DataTemplate for the DragAdorner.
        /// </summary>
        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        /// <summary>
        /// Sets the DataTemplate for the DragAdorner.
        /// </summary>
        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        #endregion

        #region Drag Adorner Template Selector

        /// <summary>
        /// Gets or Sets a DataTemplateSelector for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateSelectorProperty
            = DependencyProperty.RegisterAttached("DragAdornerTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(DragDrop),
                new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// Gets the DataTemplateSelector for the DragAdorner.
        /// </summary>
        public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DragAdornerTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets the DataTemplateSelector for the DragAdorner.
        /// </summary>
        public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DragAdornerTemplateSelectorProperty);
        }

        #endregion

        #region Drop Adorner Template Selector

        /// <summary>
        /// Gets or Sets a DataTemplateSelector for the DragAdorner based on the DropTarget.
        /// </summary>
        public static readonly DependencyProperty DropAdornerTemplateSelectorProperty
            = DependencyProperty.RegisterAttached("DropAdornerTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(DragDrop),
                new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// Gets the DataTemplateSelector for the DragAdorner based on the DropTarget.
        /// </summary>
        public static void SetDropAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DropAdornerTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets the DataTemplateSelector for the DragAdorner based on the DropTarget.
        /// </summary>
        public static DataTemplateSelector GetDropAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DropAdornerTemplateSelectorProperty);
        }

        #endregion

        #region Use Visual Source Item Size For Drag Adorner

        /// <summary>
        /// Use descendant bounds of the VisualSourceItem as MinWidth for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty UseVisualSourceItemSizeForDragAdornerProperty
            = DependencyProperty.RegisterAttached("UseVisualSourceItemSizeForDragAdorner",
                typeof(bool),
                typeof(DragDrop),
                new PropertyMetadata(false));

        /// <summary>
        /// Get the flag which indicates if the DragAdorner use the descendant bounds of the VisualSourceItem as MinWidth.
        /// </summary>
        public static bool GetUseVisualSourceItemSizeForDragAdorner(UIElement target)
        {
            return (bool)target.GetValue(UseVisualSourceItemSizeForDragAdornerProperty);
        }

        /// <summary>
        /// Set the flag which indicates if the DragAdorner use the descendant bounds of the VisualSourceItem as MinWidth.
        /// </summary>
        public static void SetUseVisualSourceItemSizeForDragAdorner(UIElement target, bool value)
        {
            target.SetValue(UseVisualSourceItemSizeForDragAdornerProperty, value);
        }

        #endregion

        #region Use Default Drag Adorner

        /// <summary>
        /// Gets or Sets whether if the default DragAdorner should be use.
        /// </summary>
        public static readonly DependencyProperty UseDefaultDragAdornerProperty
            = DependencyProperty.RegisterAttached("UseDefaultDragAdorner",
                typeof(bool),
                typeof(DragDrop),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets whether if the default DragAdorner is used.
        /// </summary>
        public static bool GetUseDefaultDragAdorner(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultDragAdornerProperty);
        }

        /// <summary>
        /// Sets whether if the default DragAdorner should be use.
        /// </summary>
        public static void SetUseDefaultDragAdorner(UIElement target, bool value)
        {
            target.SetValue(UseDefaultDragAdornerProperty, value);
        }

        #endregion
    }
}
