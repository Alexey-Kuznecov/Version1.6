using AlexeyKuznecov.Library.Mvvm.Base;
using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Common.Module;
using UnityCommander.Core.Commands;

namespace UnityCommander.Modules.Viewer.ViewModels
{
    public class PluginSettingsViewModel : PropertiesChanged, ITabPanelContent
    {
        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        /// <summary>
        /// The navigation command.   
        /// </summary>
        private NavigationInvoker navigationCommand;

        private List<PluginSettingsModel> pluginSettingsModels;

        public PluginSettingsViewModel(CommandManager manager)
        {
            this.commandManager = manager;

            this.PluginSettingsModels = new List<PluginSettingsModel>
            {
                new PluginSettingsModel
                {
                    Category = "Files",
                    Tags = new string[] { "Files", "Folders" },
                    Description = "Попробуйте новую кроссплатформенную оболочку PowerShell (https://aka.ms/pscore6)",
                    Options = new object[]
                    {
                        "Palit",
                        "Tree",
                        "List",
                        "Cards"
                    }
                },
                new PluginSettingsModel
                {
                    Category = "Accessibility Support",
                    Tags = new string[] { "Files", "Folders" },
                    Description = "Controls whether the editor should run in a mode where it is optimized for screen readers. Setting to on will disable word wrapping.",
                    Options = new object[]
                    {
                        "Auto",
                        "On",
                        "Off"
                    }
                }
            };
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        public string CurrentPath { get; set; }

        public List<PluginSettingsModel> PluginSettingsModels 
        {
            get => this.pluginSettingsModels;
            set
            {
                this.pluginSettingsModels = value;
                this.OnPropertyChanged("PluginSettingsModels");
            }
        }

        public string GetCurrentFilePath()
        {
            return this.CurrentPath;
        }

        public string GetCurrentPath()
        {
            return this.CurrentPath;
        }

        public Guid GetPanelToken()
        {
            return this.Token;
        }

        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            this.CurrentPath = path;
            this.Token = token;
            this.navigationCommand = (NavigationInvoker)this.commandManager.CommandRegister(token, new NavigationInvoker());
            return this;
        }
    }
}
