
namespace UnityCommander.Modules.TabPanel.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using UnityCommander.Common.Module;

    public class AttachedElementFocus
    {
        private static ICommand gotFocusCommand;

        #region Raise Command

        public static ICommand GetRaiseCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(RaiseCommandProperty);
        }

        public static void SetRaiseCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(RaiseCommandProperty, value);
        }

        public static readonly DependencyProperty RaiseCommandProperty =
            DependencyProperty.RegisterAttached("RaiseCommand", typeof(ICommand), typeof(AttachedElementFocus), new UIPropertyMetadata(null, RaiseCommandChanged));

        static void RaiseCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;

            if (e.NewValue is ICommand command)
            {
                gotFocusCommand = command;
            }
        }

        #endregion

        #region Focussed Element Name

        public static bool GetFocussedElementName(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocussedElementNameProperty);
        }

        public static void SetFocussedElementName(DependencyObject obj, bool value)
        {
            obj.SetValue(FocussedElementNameProperty, value);
        }

        public static readonly DependencyProperty FocussedElementNameProperty =
            DependencyProperty.RegisterAttached("FocussedElementName", typeof(bool), typeof(AttachedElementFocus), new UIPropertyMetadata(false, ElementNamePropertyChanged));

        static void ElementNamePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false) return;

            if (sender is FrameworkElement element)
            {
                element.GotFocus += Element_GotFocus;
                element.LostFocus += Element_LostFocus;
            }

            //element.Focus();
        }

        private static void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            border.BorderBrush = new SolidColorBrush(Color.FromRgb(125, 162, 230));

            if (border.DataContext is ITabPanel tabPanel)
            {
                tabPanel.SetCurrentTabPanel(tabPanel);

                border.BorderThickness = tabPanel.CurrentRegionName == NestedRegionNames.LeftFilePanelRegion
                                             ? new System.Windows.Thickness(0, 0, 1, 1)
                                             : new System.Windows.Thickness(1, 0, 0, 1);

                if (e.Source is UserControl userControl)
                {
                    tabPanel.SetActiveTabPanelContent(userControl.DataContext as ITabPanelContent);
                }
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

        #endregion
    }
}
