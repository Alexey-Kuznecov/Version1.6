
namespace UnityCommander.Controls.Ribbon.Control
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using AlexeyKuznecov.Library.Mvvm.Base;

    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models.Icons;

    public class RibbonDropListBuilder
    {
        private readonly List<DropListPopupModel> dropListPopupModel = new ();

        private readonly ComboBox comboBox = new ();
        
        public ListBoxItem DropListItem { get; set; }

        public RibbonDropListBuilder(string content, IIcon icon)
        {
            var popButton = new ListBoxItem()
            {
                Template = (ControlTemplate)Application.Current.FindResource("RibbonListBoxItemTemplate"),
                Style = (Style)Application.Current.FindResource("RibbonListBoxItemStyles"), 
            };

            popButton.DataContext = new DataBindingControl
            {
                Content = content,
                Icon = icon.GetIconPath(),
                GlobalCommand = new RibbonCommand
                {
                    Command = new RelayCommand(this.SetPopupNavigation),
                    CommandParameter = popButton
                }
            };

            this.DropListItem = popButton;
        }
        
        public void AddItem(string text, IIcon icon, IGlobalCommand command)
        {
            var listBoxItem = new DropListPopupModel 
            { 
                Content = text, 
                Command = command?.Command, 
                Icon = icon.GetIconPath()
            };

            this.dropListPopupModel.Add(listBoxItem);
        }

        private void SetPopupNavigation(object button)
        {
            DropListPopup popupControl = new DropListPopup();
            DropListPopupViewMdoel popupViewModel = new DropListPopupViewMdoel(this.dropListPopupModel);
            popupControl.DataContext = popupViewModel;

            // Popup creation.
            Popup popupBox = new Popup();

            popupBox.Child = popupControl;
            popupBox.AllowsTransparency = true;
            popupBox.IsOpen = true;
            popupBox.PlacementTarget = button as UIElement;
            popupBox.Placement = PlacementMode.Bottom;
           
            popupBox.Height = Double.NaN;
            popupBox.Width = Double.NaN;
            popupBox.VerticalOffset = -40;
            popupBox.HorizontalOffset = -15;
            popupBox.HorizontalAlignment = HorizontalAlignment.Center;
            popupBox.VerticalAlignment = VerticalAlignment.Center;
            popupBox.PopupAnimation = PopupAnimation.Fade;
            popupBox.StaysOpen = false;
            Panel.SetZIndex(popupBox, 1);

            Binding bind = new Binding("IsPopupOpen") { Mode = BindingMode.TwoWay, Source = popupViewModel };
            BindingOperations.SetBinding(popupBox, Popup.IsOpenProperty, bind);
        }
    }
}