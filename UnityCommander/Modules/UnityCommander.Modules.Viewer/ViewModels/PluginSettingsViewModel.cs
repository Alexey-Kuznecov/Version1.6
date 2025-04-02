using AlexeyKuznecov.Library.Mvvm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models;
using UnityCommander.Common.Module;
using UnityCommander.Core.Commands;
using UnityCommander.Integration.Attributes;
using UnityCommander.Integration.Options;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.Viewer.ViewModels
{
    [Obsolete]
    public class PluginSettingsViewModel : PropertiesChanged, ITabPanelContent, IPluginSettingsViewer
    {
        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        private readonly IPluginLoaderService pluginLoaders;

        private readonly IGlobalCommandManager globalCommandManager;
        
        private IPluginSettings pluginSettings;
        
        private SettingsBase pluginSettingsModel;

        private PluginSettingsModel selectOption;
        private object selectedOption;
        private object listOptions;
        private int selectIndexOption;

        /// <summary>
        /// The navigation command.   
        /// </summary>
        private NavigationInvoker navigationCommand;

        private List<PluginSettingsModel> pluginSettingsModels;

        public PluginSettingsViewModel(CommandManager manager, IPluginLoaderService pluginService, IGlobalCommandService globalCommandService)
        {
            this.globalCommandManager = globalCommandService.GetCommandManager();
            this.commandManager = manager;
            this.pluginLoaders = pluginService;
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        public string CurrentPath { get; set; }

        public PluginSettingsModel SelectOption
        {
            get => this.selectOption;
            set
            {
                this.selectOption = value;
                this.OnPropertyChanged("SelectOption");
                this.pluginSettings.OnSettingsChanged(null);
            }
        }

        public object ListOptions
        {
            get => this.listOptions;
            set
            {
                this.listOptions = value;
                this.OnPropertyChanged("ListOptions");
            }
        }

        public int SelectIndexOption
        {
            get => this.selectIndexOption;
            set
            {
                this.selectIndexOption = value;
                this.OnPropertyChanged("SelectIndexOption");
            }
        }

        public object SelectedOption
        {
            get => this.selectedOption;
            set
            {
                this.selectedOption = value;

                this.OnPropertyChanged("SelectedOption");

                if (this.selectedOption != null)
                {
                    //SelectOption.SelectedOption = (string)this.selectedOption;
                    
                    if (this.SelectOption != null)
                    {
                        this.SelectOption.SetValue(this.selectedOption);
                        this.pluginSettings.OnSettingsChanged(this.pluginSettingsModel);
                    }
                }
            }
        }

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

        public void SetPluginSettingsContent(object pluginObj)
        {
            if (pluginObj is not IPluginSettings)
                return;

            this.PluginSettingsModels = new List<PluginSettingsModel>();

            foreach (var pluginContext in pluginLoaders.GetPluginContext())
            {
                var ass = pluginContext.GetAssociatedTypes();
           
                foreach (var keyValuePair in ass.Types)
                {
                    if (keyValuePair.Key is IPluginSettings settings)
                    {
                        if (pluginObj != settings)
                            continue;

                        this.pluginSettings = settings;
                        this.pluginSettingsModel = keyValuePair.Value as SettingsBase;

                        PropertyInfo[] propertiesInfo = this.pluginSettingsModel?.GetType().GetProperties();

                        foreach (var property in propertiesInfo)
                        {
                            if (property.CustomAttributes.All(p => p.AttributeType.Name != "OptionAttribute")) continue;

                            string description = null;
                            string title = null;
                            string category = null;

                            foreach (var attribute in property.GetCustomAttributes())
                            {
                                description = (string)attribute.GetType().GetProperty("Description")?.GetValue(attribute);
                                title = (string)attribute.GetType().GetProperty("Title")?.GetValue(attribute);
                                category = (string)attribute.GetType().GetProperty("Category")?.GetValue(attribute);
                            }

                            this.PluginSettingsModels.Add(new PluginSettingsModel
                            {
                                Category = title,
                                Tags = new string[] { "Files", "Folders" },
                                Description = description,
                                Options = property.GetValue(this.pluginSettingsModel),
                                Source = this.pluginSettingsModel,
                                OptionName = property.Name
                            });
                        }
                    }
                }
            }
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
