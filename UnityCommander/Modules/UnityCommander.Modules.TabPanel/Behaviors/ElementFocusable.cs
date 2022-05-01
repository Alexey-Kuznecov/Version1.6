using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Core;
using UnityCommander.Core.Modules;

namespace UnityCommander.Modules.TabPanel.Behaviors
{
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

            var element = sender as FrameworkElement;
            element.GotFocus += Element_GotFocus;
            element.LostFocus += Element_LostFocus;
            //element.Focus();
        }

        private static void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(125, 162, 230));

            if (border.DataContext is ITabPanel tabPanel)
            {
                tabPanel.SetCurrentTabPanel(tabPanel);

                if (tabPanel.CurrentRegionName == NestedRegionNames.LeftFilePanelRegion)
                    border.BorderThickness = new System.Windows.Thickness(0, 0, 1, 1);
                else
                    border.BorderThickness = new System.Windows.Thickness(1, 0, 0, 1);

                if (e.Source is UserControl userControl)
                {
                    tabPanel.SetActiveTabPanelContent(userControl.DataContext as ITabPanelContent);
                }
            }
            
            //gotFocusCommand?.Execute(new object[] { border.DataContext, e.Source });
        }

        private static void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            border.BorderBrush = Brushes.White;
        }

        #endregion
    }
}
