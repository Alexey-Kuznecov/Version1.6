
namespace UnityCommander.Integration.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// The option builder.
    /// </summary>
    public class OptionBuilder
    {
        /// <summary>
        /// The plugin options.
        /// </summary>
        private readonly List<IOption> pluginOptions = new ();

        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        /// List of <see cref="PluginOption"/>.
        /// </returns>
        public List<IOption> GetOptions() => this.pluginOptions;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <param name="render">
        /// The render.
        /// </param>
        public void Add(string title, List<object> option, object defaultValue, Selector handler, OptionRender render = OptionRender.Default)
        {
            this.pluginOptions.Add(new PluginOption
            {
                Title = title,
                Option = option,
                DefaultOption = defaultValue,
                Handler = handler,
                Render = render
            });
        }

        public void Add(string title, bool value, Predictor handler, OptionRender render = OptionRender.Checkbox)
        {
            this.pluginOptions.Add(new PluginOption
            {
                Title = title,
                Option = value,
                DefaultOption = value,
                Handler = handler,
                Render = render
            });
        }
    }
}
