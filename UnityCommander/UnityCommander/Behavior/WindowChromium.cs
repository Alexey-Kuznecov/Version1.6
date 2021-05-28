using System.ComponentModel;

#if HAS_WINUI
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

using UnityCommander.Common.Styling;

namespace UnityCommander.Behavior
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the WindowUnityStyle in Prism.Mvvm.
    /// </summary>
    public static class WindowUnityStyle
    {
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private static Window mWindow;

        /// <summary>
        /// The WindowInstance attached property.
        /// </summary>
        public static DependencyProperty WindowInstanceProperty = DependencyProperty.RegisterAttached("WindowInstance", typeof(Window), typeof(WindowUnityStyle), new PropertyMetadata(defaultValue: null, propertyChangedCallback: WindowInstanceChanged));

        /// <summary>
        /// Gets the value for the <see cref="WindowInstanceProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowInstanceProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static Window GetWindowInstance(DependencyObject obj)
        {
            return (Window)obj.GetValue(WindowInstanceProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowInstanceProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The value to attach.</param>
        public static void SetWindowInstance(DependencyObject obj, Window value)
        {
            obj.SetValue(WindowInstanceProperty, value);
        }

        private static void WindowInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if !HAS_WINUI
            if (!DesignerProperties.GetIsInDesignMode(d))
#endif
            {
                mWindow = (Window)e.NewValue;
                
                // Listen out for the window resizing
                //mWindow.StateChanged += (sender, e) =>
                //{
                //    // Fire off events for all properties that are affected by a resize
                //   // WindowResized();
                //};
            }
        }
    }
}