
namespace W3Manager.WP1
{
    using System;
    using System.Collections.Generic;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game status column.
    /// </summary>
    public class GameStatusColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor
    {
        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStatusColumn"/> class.
        /// </summary>
        public GameStatusColumn()
        {
            this.GameStatusFormat = new List<object>
            {
                "Option One",
                "Option Two",
                "Option Three",
            };
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "Game Status Column";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Plugin add new status column.";

        /// <summary>
        /// Gets the date time format.
        /// </summary>
        public List<object> GameStatusFormat { get; private set; }

        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Game Status", 50);
            builder.AddContextItem("Install", this.InstallMod);
        }

        /// <summary>
        /// The column value validate.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ColumnValueValidate(IPluginContext context)
        {
            return context;
        }

        /// <summary>
        /// The column value handler.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ColumnValueHandler(string path)
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        /// <summary>
        /// The column value render.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionRender"/>.
        /// </returns>
        public OptionRender ColumnValueRender()
        {
            return OptionRender.TextBlock;
        }

        /// <summary>
        /// The option build.
        /// </summary>
        /// <param name="optionBuilder">
        /// The option builder.
        /// </param>
        public void OptionBuild(OptionBuilder optionBuilder)
        {
            optionBuilder.Add("Game Status Option", this.GameStatusFormat, this.GameStatusHandler, OptionRender.DropBox);
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        private void InstallMod()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void DisplayAsHandler(object selected)
        {
            if (selected is OptionRender render)
            {
                this.optionRender = render;
            }
        }
        
        /// <summary>
        /// The date time format handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void GameStatusHandler(object selected)
        {
            // throw new NotImplementedException();
        }
    }
}
