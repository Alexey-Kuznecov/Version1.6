
namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using Prism.Mvvm;

    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The main tab view.
    /// </summary>
    public class MainTabViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainTabViewModel"/> class.
        /// </summary>
        public MainTabViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainTabViewModel"/> class.
        /// </summary>
        /// <param name="iconProvider">
        /// The icon provider.
        /// </param>
        public MainTabViewModel(IIconProviderService iconProvider)
        {
        }
    }
}
