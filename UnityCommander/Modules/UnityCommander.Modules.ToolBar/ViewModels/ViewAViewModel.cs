
namespace UnityCommander.Modules.ToolBar.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;

    using AlexLibWpf.Mvvm.Base;

    using Prism.Commands;
    using Prism.Mvvm;

    using UnityCommander.Modules.ToolBar.Views;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ViewAViewModel : BindableBase
    {
        /// <summary>
        /// The _message.
        /// </summary>
        private string message;

        /// <summary>
        /// The user controls.
        /// </summary>
        private UserControl userControls;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// </summary>
        public ViewAViewModel()
        {
            this.Message = "This Toolbar View";
            this.UserControls = new MainTabControl();
        }

        /// <summary>
        /// The set ribbon.
        /// </summary>
        public ICommand SetRibbon => new RelayCommand(
            (obj) =>
        {
            if (obj is Button bt)
            {
                switch (bt.Content)
                {
                    case "Main":
                        this.UserControls = new MainTabControl();
                        break;
                    case "View":
                        this.UserControls = new FileOperationControl();
                        break;
                    default:
                        this.UserControls = new MainTabControl();
                        break;
                }
            }
        });

        /// <summary>
        /// Gets or sets the user controls.
        /// </summary>
        public UserControl UserControls
        {
            get => this.userControls;
            set => this.SetProperty(ref this.userControls, value);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }
    }
}
