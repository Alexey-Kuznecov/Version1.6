
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Controls;

    /// <summary>
    /// The ribbon builder extension.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public static class RibbonBuilderExtension
    {
        /// <summary>
        /// The builders.
        /// </summary>
        private static readonly List<RibbonControlGroupBuilder> ControlGroupBuilders = new ();

        /// <summary>
        /// The ribbon builders.
        /// </summary>
        private static RibbonBuilder ribbonBuilders;

        /// <summary>
        /// The ribbon builder instance.
        /// </summary>
        private static RibbonBuilder ribbonBuilderInstance;

        #region MyRegion

        /// <summary>
        /// The build section.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="ribbonBuilder">
        /// The ribbon Builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        public static RibbonBuilder SetSection(this RibbonBuilder builder, RibbonBuilder ribbonBuilder)
        {
            ribbonBuilders = ribbonBuilder ?? builder;
            return ribbonBuilders;
        }

        /// <summary>
        /// The build section.
        /// </summary>
        /// <param name="ribbonBuilder">
        /// The ribbon Builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        public static RibbonBuilder SetSection(RibbonBuilder ribbonBuilder)
        {
            ribbonBuilders = ribbonBuilder;
            return ribbonBuilders;
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="controlGroupBuilder">
        /// The control group builder.
        /// </param>
        /// <returns>
        /// The <see cref="Ribbon"/>.
        /// </returns>
        public static Ribbon Build(this RibbonBuilder controlGroupBuilder)
        {
            return ribbonBuilders.Build();
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="controlGroupBuilder">
        /// The control group builder.
        /// </param>
        /// <returns>
        /// The <see cref="Ribbon"/>.
        /// </returns>
        public static Grid BuildGrid(this RibbonBuilder controlGroupBuilder)
        {
            return ribbonBuilders.BuildGrid();
        }

        #endregion

        #region Single Group 

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="controlGroupBuilder">
        /// The control group builder.
        /// </param>
        /// <param name="tabName">
        /// The tab name.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        [Obsolete]
        public static RibbonBuilder Build(this RibbonControlGroupBuilder controlGroupBuilder, string tabName)
        {
            var ribbonBuilder = new RibbonBuilder(tabName);
            ribbonBuilder.SetSection(controlGroupBuilder);
            return ribbonBuilder;
        }

        #endregion

        #region Full Bundle 

        /// <summary>
        /// The build section.
        /// </summary>
        /// <param name="controlGroupBuilder">
        /// The control group builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonBuilder"/>.
        /// </returns>
        public static RibbonBuilder Build(this RibbonControlGroupBuilder controlGroupBuilder)
        {
            foreach (var builder in ControlGroupBuilders)
                ribbonBuilderInstance.SetSection(builder);
            ControlGroupBuilders.Clear();
            return ribbonBuilderInstance;
        }

        /// <summary>
        /// The initial.
        /// </summary>
        /// <param name="tabName">
        /// The tab Name.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonControlGroupBuilder"/>.
        /// </returns>
        public static RibbonControlGroupBuilder Initial(string tabName)
        {
            ribbonBuilderInstance = new RibbonBuilder(tabName);
            return new RibbonControlGroupBuilder();
        }

        /// <summary>
        /// The initial.
        /// </summary>
        /// <param name="ribbonBuilder">
        /// The ribbon builder.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonControlGroupBuilder"/>.
        /// </returns>
        public static RibbonControlGroupBuilder Initial(this RibbonBuilder ribbonBuilder)
        {
            ribbonBuilderInstance = ribbonBuilder;
            return new RibbonControlGroupBuilder();
        }

        /// <summary>
        /// The build group.
        /// </summary>
        /// <param name="controlGroupBuilder">
        /// The ribbon builder.
        /// </param>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonControlGroupBuilder"/>.
        /// </returns>
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
