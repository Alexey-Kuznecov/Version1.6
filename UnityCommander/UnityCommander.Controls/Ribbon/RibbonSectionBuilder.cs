
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using AlexLibWpf.Mvvm.Base;

    /// <summary>
    /// TODO The ribbon section builder.
    /// </summary>
    public class RibbonSectionBuilder
    {
        /// <summary>
        /// The Sections.
        /// </summary>
        private static readonly HashSet<Section> Sections = new ();

        /// <summary>
        /// The ribbon builder.
        /// </summary>
        private static Grid sectionContainer;

        /// <summary>
        /// The ribbon.
        /// </summary>
        private readonly Ribbon ribbon = new ();

        /// <summary>
        /// The current section.
        /// </summary>
        private readonly Section currentSection = new ();

        /// <summary>
        /// The ribbon section.
        /// </summary>
        private readonly RibbonSection ribbonSection;

        /// <summary>
        /// The ribbon tab.
        /// </summary>
        private readonly RibbonTab ribbonTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonSectionBuilder"/> class.
        /// </summary>
        /// <param name="tabHeader">
        /// The tab Header.
        /// </param>
        public RibbonSectionBuilder(string tabHeader)
        {
            this.ribbonSection = new RibbonSection();
            this.ribbonTab = new RibbonTab();

            Button button = new ();
            {
                button.Command = this.TabCommand;
                button.Width = 100;
                button.Content = tabHeader;
                button.CommandParameter = this.currentSection;
            }

            // Here order is very important.
            Section.Tabs.Add(button);
            this.currentSection.CurrentTab = button;
            Sections.Add(this.currentSection);
        }

        /// <summary>
        /// Gets or sets a command that set a section and makes the current button unavailable.
        /// </summary>
        public ICommand TabCommand => new RelayCommand((obj) =>
        {
            if (obj is Section section)
            {
                foreach (var tab in Section.Tabs)
                {
                    tab.IsEnabled = tab.GetHashCode() != section.CurrentTab.GetHashCode();
                }

                if (sectionContainer.Children.Count == 2)
                {
                    sectionContainer.Children.RemoveAt(1);
                    this.ribbonSection.Children.Clear();
                }

                foreach (var group in section.Groups)
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

            foreach (var button in Section.Tabs)
            {
                this.ribbonTab.Children.Add(button);
            }

            foreach (var group in this.currentSection.Groups)
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
        /// The add section.
        /// </summary>
        /// <param name="groupBuilder">
        /// The section Builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonSectionBuilder"/>.
        /// </returns>
        public RibbonSectionBuilder SetSection(RibbonGroupBuilder groupBuilder)
        {
            RibbonGroup ribbonGroup = groupBuilder.GetGroup();
            this.currentSection.Groups.Add(ribbonGroup);
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

        /// <summary>
        /// The section.
        /// </summary>
        internal class Section
        {
            /// <summary>
            ///  The Token.
            /// </summary>
            public readonly Guid Token = Guid.NewGuid();

            /// <summary>
            /// Gets or sets the tabs.
            /// </summary>
            public static HashSet<Button> Tabs { get; set; } = new ();

            /// <summary>
            /// Gets or sets the tab.
            /// </summary>
            public Button CurrentTab { get; set; }

            /// <summary>
            /// Gets or sets the ribbon section.
            /// </summary>
            public RibbonSection CurrentSection { get; set; }

            /// <summary>
            /// Gets or sets the groups.
            /// </summary>
            public HashSet<RibbonGroup> Groups { get; set; } = new ();
            
            /// <summary>
            /// Gets or sets the ribbon tab.
            /// </summary>
            public RibbonTab RibbonTab { get; set; } = new ();

            /// <summary>
            /// The get hash code.
            /// </summary>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            public override int GetHashCode()
            {
                return this.Token.GetHashCode();
            }

            /// <summary>
            /// The equals.
            /// </summary>
            /// <param name="window">
            /// The window.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public override bool Equals(object window)
            {
                if (ReferenceEquals(window, this))
                {
                    return true;
                }

                return Equals(window as Section);
            }

            /// <summary>
            /// The equals.
            /// </summary>
            /// <param name="other">
            /// The other.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool Equals(Section other)
            {
                if (other == null)
                {
                    return false;
                }

                return Equals(this.Token, other.Token);
            }
        }
    }
}
