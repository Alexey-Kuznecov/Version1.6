
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Controls.Ribbon.Control;
    using UnityCommander.Controls.Ribbon.Subgroup;

    public class RibbonControlGroupBuilder
    {
        private BaseSubgroup baseSubgroup;

        private RibbonGroup group;

        public RibbonControlGroupBuilder AddButton(string name, IIcon icon, RibbonCommand command)
        {
            RibbonElement element = new RibbonElement(new RibbonButton(name, icon, command));
            this.group.Children.Add(element);
            return this;
        }

        public void AddList(Action<RibbonControlListBuilder> callback)
        {
            using var ribbonControlList = new RibbonControlListBuilder();
            
            ribbonControlList.AddItem(callback);
            //this.SetCanvas(ribbonControlList);
            this.group.Children.Add(ribbonControlList.ControlsStackGroup);
        }

        internal RibbonControlGroupBuilder AddGroup(string groupName, Action<RibbonControlGroupBuilder> callback)
        {
            var rga = new RibbonGroupAdorner();
            this.group = new RibbonGroup();
            this.GetAdorner = rga.SetAdorner(groupName, this.group);
            callback(this);
            return this;
        }

        internal void SetCanvas(RibbonControlListBuilder builder)
        {
            Canvas canvas = new Canvas
            {
                Width = 200,
                Height = 200,
                Background = new SolidColorBrush(Color.FromRgb(190, 190, 190))
            };
            Grid grid = new Grid();
            Button button = new Button { Content = "Clollapse" };

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

            grid.Children.Add(builder.ControlsStackGroup);
            grid.Children.Add(button);
            Grid.SetRow(builder.ControlsStackGroup, 0);
            Grid.SetRow(button, 1);
            canvas.Children.Add(grid);
            Canvas.SetZIndex(canvas, 100);
            this.group.Children.Add(canvas);
        }

        internal RibbonGroupAdorner GetAdorner { get; private set; }
    }
}
