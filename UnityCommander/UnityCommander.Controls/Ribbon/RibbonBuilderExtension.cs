
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Controls;

    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
        public static class RibbonBuilderExtension
        {
            private static readonly List<RibbonControlGroupBuilder> ControlGroupBuilders = new ();
            private static RibbonBuilder ribbonBuilders;
            private static RibbonBuilder ribbonBuilderInstance;

            #region MyRegion

            public static RibbonBuilder SetSection(this RibbonBuilder builder, RibbonBuilder ribbonBuilder)
            {
                ribbonBuilders = ribbonBuilder ?? builder;
                return ribbonBuilders;
            }

            public static RibbonBuilder SetSection(RibbonBuilder ribbonBuilder)
            {
                ribbonBuilders = ribbonBuilder;
                return ribbonBuilders;
            }

            public static Ribbon Build(this RibbonBuilder controlGroupBuilder)
            {
                return ribbonBuilders.Build();
            }

            public static Grid BuildGrid(this RibbonBuilder controlGroupBuilder)
            {
                return ribbonBuilders.BuildGrid();
            }

            #endregion

            #region Single Group 

            [Obsolete]
            public static RibbonBuilder Build(this RibbonControlGroupBuilder controlGroupBuilder, string tabName)
            {
                var ribbonBuilder = new RibbonBuilder(tabName);
                ribbonBuilder.SetSection(controlGroupBuilder);
                return ribbonBuilder;
            }

            #endregion

            #region Full Bundle 

            public static RibbonBuilder Build(this RibbonControlGroupBuilder controlGroupBuilder)
            {
                foreach (var builder in ControlGroupBuilders)
                    ribbonBuilderInstance.SetSection(builder);
                ControlGroupBuilders.Clear();
                return ribbonBuilderInstance;
            }

            public static RibbonControlGroupBuilder Initial(string tabName)
            {
                ribbonBuilderInstance = new RibbonBuilder(tabName);
                return new RibbonControlGroupBuilder();
            }

            public static RibbonControlGroupBuilder Initial(this RibbonBuilder ribbonBuilder)
            {
                ribbonBuilderInstance = ribbonBuilder;
                return new RibbonControlGroupBuilder();
            }

            public static RibbonControlGroupBuilder SetGroup(
                this RibbonControlGroupBuilder controlGroupBuilder,
                string groupName,
                Action<RibbonControlGroupBuilder> callback)
            {
                var groupBuilder = controlGroupBuilder ?? new RibbonControlGroupBuilder();
                groupBuilder = groupBuilder.AddGroup(groupName, callback);
                ControlGroupBuilders.Add(groupBuilder);

                return !ControlGroupBuilders.Contains(groupBuilder) ? new RibbonControlGroupBuilder() : null;
            }

            #endregion
        }
    }
