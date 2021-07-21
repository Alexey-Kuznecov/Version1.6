
namespace UnityCommander.Integration.Options
{
    using System;

    /// <summary>
    /// The option builder.
    /// </summary>
    public class PluginOption : IOption
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        public object Option { get; set; }

        /// <summary>
        /// Gets or sets the default option.
        /// </summary>
        public object DefaultOption { get; set; }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        public Delegate Handler { get; set; }

        /// <summary>
        /// Gets or sets the option render.
        /// </summary>
        public OptionRender Render { get; set; }

        /// <summary>
        /// Gets or sets the option builders.
        /// </summary>
        public IOptionBuilder OptionBuilders { get; set; }
    }
}
