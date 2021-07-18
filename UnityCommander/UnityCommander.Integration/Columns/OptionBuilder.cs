
namespace UnityCommander.Integration.Columns
{
    using System;

    using UnityCommander.Integration.Options;

    /// <summary>
    /// The option builder.
    /// </summary>
    public class OptionBuilder
    {
        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        public Delegate Handler { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public Type Source { get; set; }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the option render.
        /// </summary>
        public OptionRender OptionRender { get; set; }

        /// <summary>
        /// The get list.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetList()
        {
            var propertyInfo = this.Source.GetProperty(this.PropertyName);
            var instance = Activator.CreateInstance(this.Source);
            var val = propertyInfo?.GetValue(instance);
            return val;
        }
    }
}
