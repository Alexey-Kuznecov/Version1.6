
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Controls.Navigation
{
    internal class NavigationItemBuilder
    {
        internal UIElement Build(
            PopupParameters parameters,
            ICommand popupCommand,
            ICommand navigateCommand)
        {
            var popButton = new Button
            {
                Style = (Style)Application.Current.FindResource(
                    "NavigationPopupButtonStyle"),
                Command = popupCommand,
                CommandParameter = parameters.Anchor,
            };

            var navButton = new Button
            {
                Style = (Style)Application.Current.FindResource(
                    "NavigationBackButtonStyle"),
                Content = parameters.CurrentItem.Name,
                Command = navigateCommand,
                CommandParameter = parameters.CurrentItem.Path
            };

            NavigationButtonDragDrop.SetEnable(navButton, true);
            NavigationButtonDragDrop.SetDropPath(
                navButton,
                parameters.CurrentItem.Path);

            var grid = CreateGridNavigationItem(
                navButton,
                popButton);

            parameters.Anchor = grid;
            popButton.CommandParameter = parameters;

            return grid;
        }

        private static Grid CreateGridNavigationItem(Button navButton, Button popButton)
        {
            Grid grid = new Grid();
            ColumnDefinition gridColumn = new ColumnDefinition();
            ColumnDefinition gridColumn2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(gridColumn);
            grid.ColumnDefinitions.Add(gridColumn2);
            Grid.SetColumn(navButton, 0);
            Grid.SetColumn(popButton, 1);
            grid.Children.Add(navButton);
            grid.Children.Add(popButton);
            grid.Style = (Style)Application.Current.FindResource("NavigationButtonShadowStyle");

            return grid;
        }
    }
}
