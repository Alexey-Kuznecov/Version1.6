
namespace UnityCommander.Integration.Options
{
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The OptionBuilder interface.
    /// </summary>
    public interface IOptionBuilder : IPluginService
    {
        /// <summary>
        /// The option build.
        /// </summary>
        /// <param name="optionBuilder">
        /// The option builder.
        /// </param>
        void OptionBuild(OptionBuilder optionBuilder);
    }
}
