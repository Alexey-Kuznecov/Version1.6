#define ADORNER

namespace UnityCommander.Controls.Ribbon
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using AlexeyKuznecov.Library.Mvvm.Base;

    /// <summary>
    /// This class is responsible for creating a ribbon of tools and tabs.
    /// </summary>
    public class RibbonBuilder
    {
        /// <summary>
        /// The ribbon builder.
        /// </summary>
        private static Grid sectionContainer;

        /// <summary>
        /// The ribbon.
        /// </summary>
        private readonly Ribbon ribbon = new ();

        /// <summary>
        /// The ribbon section.
        /// </summary>
        private readonly RibbonSection ribbonSection;

        /// <summary>
        /// The ribbon tab.
        /// </summary>
        private readonly RibbonTab ribbonTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonBuilder"/> class.
        /// </summary>
        /// <param name="tabHeader">
        /// The tab Header.
        /// </param>
        public RibbonBuilder(string tabHeader)
        {
            this.ribbonSection = new RibbonSection();
            this.ribbonTab = new RibbonTab();

            Button button = new Button();
            {
                button.Command = this.TabCommand;
                button.Width = 100;
                button.Content = tabHeader;
                button.CommandParameter = this;
            }

            // Here order is very important.
            Tabs.Add(button);
            this.CurrentTab = button;
            FirstTab ??= button;
        }

        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        public static HashSet<ContentControl> Tabs { get; set; } = new ();

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        public Button CurrentTab { get; set; }

        /// <summary>
        /// Gets or sets the first tab.
        /// </summary>
        public static Button FirstTab { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public HashSet<RibbonGroupAdorner> AdornerGroups { get; set; } = new ();

        /// <summary>
        /// Gets or sets the tab.
        /// Todo: Why was the collection named first?
        /// </summary>
        public static HashSet<RibbonGroupAdorner> FirstAdornerGroup { get; set; }

        /// <summary>
        /// Gets or sets a command that set a section and makes the current button unavailable.
        /// </summary>
        public ICommand TabCommand => new RelayCommand((obj) => 
            {
                if (obj is not RibbonBuilder section) return;
                
                foreach (var tab in Tabs)
                {
                    tab.IsEnabled = tab.GetHashCode() != section.CurrentTab.GetHashCode();
                }

                if (sectionContainer.Children.Count == 2)
                {
                    sectionContainer.Children.RemoveAt(1);
                    this.ribbonSection.Children.Clear();
                }

                foreach (var group in section.AdornerGroups)
                {
                    if (!this.ribbonSection.Children.Contains(@group))
                    {
                        // Todo: Resolve the error associated with going to the first tab.
                        this.ribbonSection.Children.Add(@group);
                    }

                    if (!sectionContainer.Children.Contains(this.ribbonSection))
                    {
                        sectionContainer.Children.Add(this.ribbonSection);
                        Grid.SetRow(this.ribbonSection, 1);
                    }
                }
            });

        /// <summary>
        /// The get section.
        /// </summary>
        /// <returns>
        /// The <see cref="Ribbon"/>.
        /// </returns>
        public Ribbon Build()
        {
            this.GridBuild();
            this.TabBuild();
            
            foreach (var group in FirstAdornerGroup)
            {
                this.ribbonSection.Children.Add(group);
            }

            sectionContainer.Children.Add(this.ribbonTab);
            sectionContainer.Children.Add(this.ribbonSection);
            Grid.SetRow(this.ribbonTab, 0);
            Grid.SetRow(this.ribbonSection, 1);

            this.ribbon.Children.Add(sectionContainer);
            return this.ribbon;
        }

        /// <summary>
        /// The get section.
        /// </summary>
        /// <returns>
        /// The <see cref="Ribbon"/>.
        /// </returns>
        public Grid BuildGrid()
        {
            this.GridBuild();
            this.TabBuild();
            
            foreach (var group in FirstAdornerGroup)
            {
                this.ribbonSection.Children.Add(group);
            }

            sectionContainer.Children.Add(this.ribbonTab);
            sectionContainer.Children.Add(this.ribbonSection);
            Grid.SetRow(this.ribbonTab, 0);
            Grid.SetRow(this.ribbonSection, 1);
            return sectionContainer;
        }

        /// <summary>
        /// The add section.
        /// </summary>
        /// <param name="controlGroupBuilder">
        /// The section Builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        public RibbonBuilder SetSection(RibbonControlGroupBuilder controlGroupBuilder)
        {
            this.AdornerGroups.Add(controlGroupBuilder.GetAdorner);
            FirstAdornerGroup ??= this.AdornerGroups;
            return this;
        }

        /// <summary>
        /// The create tab.
        /// </summary>
        private void TabBuild()
        {
            Grid grid = new Grid();
            ColumnDefinition columnDefinition = new ColumnDefinition { Width = new GridLength(800) };
            ColumnDefinition columnDefinition1 = new ColumnDefinition { Width = new GridLength(100) };
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.ColumnDefinitions.Add(columnDefinition1);

            foreach (var button in Tabs)
            {
                button.IsEnabled = !button.Equals(FirstTab);
                grid.Children.Add(button);
                Grid.SetColumn(button, 0);
            }

            this.ribbonTab.Children.Add(grid);
        }

        /// <summary>
        /// The create grid.
        /// </summary>
        private void GridBuild()
        {
            Grid dynamicGrid = new Grid();
            RowDefinition tabRow = new () { Height = new GridLength(25) };
            RowDefinition sectionRow = new () { Height = new GridLength(1, GridUnitType.Star) };
            dynamicGrid.RowDefinitions.Add(tabRow);
            dynamicGrid.RowDefinitions.Add(sectionRow);
            sectionContainer = dynamicGrid;
        }
    }
}
