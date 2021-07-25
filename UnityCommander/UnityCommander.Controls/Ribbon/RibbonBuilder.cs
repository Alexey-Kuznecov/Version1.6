#define ADORNER

namespace UnityCommander.Controls.Ribbon
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using AlexLibWpf.Mvvm.Base;

    /// <summary>
    /// The ribbon group builder.
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

            Button button = new ();
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
        public static HashSet<Button> Tabs { get; set; } = new ();

        /// <summary>
        /// Gets or sets the tab.
        /// </summary>
        public Button CurrentTab { get; set; }

        /// <summary>
        /// Gets or sets the tab.
        /// </summary>
        public static Button FirstTab { get; set; }

#if ADORNER
        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public HashSet<RibbonGroupAdorner> GroupsAdorner { get; set; } = new ();

        /// <summary>
        /// Gets or sets the tab.
        /// </summary>
        public static HashSet<RibbonGroupAdorner> FirstAdorner { get; set; }
#else
        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public HashSet<RibbonGroup> Groups { get; set; } = new ();

        /// <summary>
        /// Gets or sets the tab.
        /// </summary>
        public static HashSet<RibbonGroup> FirstSection { get; set; }
#endif

        /// <summary>
        /// Gets or sets a command that set a section and makes the current button unavailable.
        /// </summary>
        public ICommand TabCommand => new RelayCommand((obj) =>
        {
            if (obj is RibbonBuilder section)
            {
                foreach (var tab in Tabs)
                {
                    tab.IsEnabled = tab.GetHashCode() != section.CurrentTab.GetHashCode();
                }

                if (sectionContainer.Children.Count == 2)
                {
                    sectionContainer.Children.RemoveAt(1);
                    this.ribbonSection.Children.Clear();
                }

                foreach (var group in section.GroupsAdorner)
                {
                    if (!this.ribbonSection.Children.Contains(group))
                    {
                        this.ribbonSection.Children.Add(group);
                    }

                    if (!sectionContainer.Children.Contains(this.ribbonSection))
                    {
                        sectionContainer.Children.Add(this.ribbonSection);
                        Grid.SetRow(this.ribbonSection, 1);
                    }
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
            sectionContainer = this.BuildGrid();

            foreach (var button in Tabs)
            {
                this.ribbonTab.Children.Add(button);
                button.IsEnabled = !button.Equals(FirstTab);
            }
            
#if ADORNER
            foreach (var group in FirstAdorner)
            {
                this.ribbonSection.Children.Add(group);
            }
#else
            foreach (var group in FirstSection)
            {
                this.ribbonSection.Children.Add(group);
            }
#endif

            sectionContainer.Children.Add(this.ribbonTab);
            sectionContainer.Children.Add(this.ribbonSection);
            Grid.SetRow(this.ribbonTab, 0);
            Grid.SetRow(this.ribbonSection, 1);

            this.ribbon.Children.Add(sectionContainer);
            return this.ribbon;
        }

        /// <summary>
        /// The add section.
        /// </summary>
        /// <param name="groupBuilder">
        /// The section Builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        public RibbonBuilder SetSection(RibbonGroupBuilder groupBuilder)
        {
            RibbonGroup ribbonGroup = groupBuilder.GetGroup();
#if ADORNER
            var adorner = new RibbonGroupAdorner().SetAdorner(ribbonGroup);
            GroupsAdorner.Add(adorner);
            FirstAdorner ??= GroupsAdorner;
#else
            this.Groups.Add(ribbonGroup);
            FirstSection ??= this.Groups;
#endif
            return this;
        }

        /// <summary>
        /// The create grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Grid"/>.
        /// </returns>
        private Grid BuildGrid()
        {
            Grid dynamicGrid = new ();
            RowDefinition tabRow = new () { Height = new GridLength(25) };
            RowDefinition sectionRow = new () { Height = new GridLength(1, GridUnitType.Star) };
            dynamicGrid.RowDefinitions.Add(tabRow);
            dynamicGrid.RowDefinitions.Add(sectionRow);
            return dynamicGrid;
        }
    }
}
