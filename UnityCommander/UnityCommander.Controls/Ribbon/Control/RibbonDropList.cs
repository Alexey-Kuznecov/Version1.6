
namespace UnityCommander.Controls.Ribbon.Control
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using AlexeyKuznecov.Library.Mvvm.Base;

    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The ribbon combo box item.
    /// </summary>
    public class RibbonDropList
    {
        /// <summary>
        /// The combo box item.
        /// </summary>
        private readonly List<DropListPopupModel> dropListPopupModel = new ();

        /// <summary>
        /// The combo box.
        /// </summary>
        private readonly ComboBox comboBox = new ();

        public RibbonDropList(string text, IIcon icon)
        {

        }

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        public void AddItem(string text, IIcon icon, RibbonCommand command)
        {
            var listBoxItem = new DropListPopupModel 
            { 
                Content = text, 
                Command = command?.Command, 
                Icon = icon.GetIconPath()
            };

            //if (command != null)
            //{
            //    InputBinding inputBinding = new InputBinding(command.Command, new MouseGesture(MouseAction.LeftClick));
            //    listBoxItem.InputBindings.Add(inputBinding);
            //}
          
            this.dropListPopupModel.Add(listBoxItem);
        }


        /// <summary>   
        /// This method is created.
        /// </summary>
        public ListBoxItem Build()
        {
            var popButton = new ListBoxItem
            {
                Content = "DropList",
                Width = 100,
                Style = (Style)Application.Current.FindResource("RibbonListBoxItemStyles")
            };

            InputBinding inputBinding = new InputBinding(new RelayCommand(this.SetPopupNavigation), new MouseGesture(MouseAction.LeftClick));
            inputBinding.CommandParameter = popButton;
            popButton.InputBindings.Add(inputBinding);
            return popButton;
        }

        /// <summary>
        /// The set popup navigation.
        /// </summary>
        /// <param name="button">
        /// The button.
        /// </param>
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