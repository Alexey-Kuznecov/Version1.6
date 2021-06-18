
namespace UnityCommander.Modules.ChildWindow
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;

    using UnityCommander.Modules.ChildWindow.Views;

    /// <summary>
    /// The child window module.
    /// </summary>
    public class ChildWindowModule : IModule
    {
        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider">
        /// The container provider.
        /// </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry">
        /// The container registry.
        /// </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}