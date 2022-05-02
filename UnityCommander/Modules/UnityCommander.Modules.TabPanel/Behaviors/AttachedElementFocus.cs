
namespace UnityCommander.Modules.TabPanel.Behaviors
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using UnityCommander.Common.Module;

    /// <summary>
    /// The attached element focus.
    /// </summary>
    public class AttachedElementFocus
    {

        /// <summary>
        /// The focused element name property.
        /// </summary>
        public static readonly DependencyProperty FocusedElementNameProperty =
            DependencyProperty.RegisterAttached(
                "FocusedElementName",
                typeof(bool),
                typeof(AttachedElementFocus),
                new UIPropertyMetadata(false, ElementNamePropertyChanged));

        /// <summary>
        /// The get focused element name.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetFocusedElementName(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusedElementNameProperty);
        }

        /// <summary>
        /// The set focused element name.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetFocusedElementName(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusedElementNameProperty, value);
        }

        /// <summary>
        /// The element name property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void ElementNamePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false) return;

            if (sender is FrameworkElement element)
            {
                element.GotFocus += Element_GotFocus;
                element.LostFocus += Element_LostFocus;

                Task.Delay(100).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                element.Focus();
                            }));
                    });
            }
        }

        /// <summary>
        /// The element_ got focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(125, 162, 230));

            if (border.DataContext is IElementFocusable tabPanel)
            {
                tabPanel.FocusElementDataProvider(new ElementFocusData
                                                      {
                                                          ElementFocusable = border, 
                                                          TabPanel = (ITabPanel)tabPanel, 
                                                          TabContent = (e.Source as UserControl)?.DataContext as ITabPanelContent
                                                      });
                border.BorderThickness = ((ITabPanel)tabPanel).RegionContentName == NestedRegionNames.LeftFilePanelRegion
                                             ? new System.Windows.Thickness(0, 0, 1, 1)
                                             : new System.Windows.Thickness(1, 0, 0, 1);
            }
        }

        /// <summary>
        /// The element_ lost focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = Brushes.White;
            }
        }
    }
}
