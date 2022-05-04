
namespace UnityCommander.Integration.Contracts
{
    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Factories;

    /// <summary>
    /// The PluginFactory interface.
    /// </summary>
    public interface IPluginFactory
    {
        /// <summary>
        /// The configure.
        /// TODO: Add description here.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        void Configure(IServiceCollection services);

        /// <summary>
        /// The set associated types.
        /// </summary>
        /// <param name="typesRegister">
        /// The types Register.
        /// </param>
        void SetAssociatedTypes(AssociatedTypesRegister typesRegister);
    }
}
