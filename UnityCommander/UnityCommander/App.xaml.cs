using CommandSystem.Abstractions;
using CommandSystem.Core.Factory;
using CommandSystem.Core.UndoRedo;
using CommandSystem.Gui.Integraion;
using CommandSystem.Infrastructure.Execution;
using CommandSystem.Infrastructure.Lifecycle;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using UnityCommander.Abstractions.Completion;
using UnityCommander.AI.ImageSearch;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Autocomplete.Infrastructure.Analyze;
using UnityCommander.Bootstrap;
using UnityCommander.CLI.Bootstrap;
using UnityCommander.Commands;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Performance;
using UnityCommander.Commands.Rendering;
using UnityCommander.Commands.Services;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Docking;
using UnityCommander.Common.Layout;
using UnityCommander.Common.Selection;
using UnityCommander.Common.Styling;
using UnityCommander.Core;
using UnityCommander.Core.Behaviors.Selection;
using UnityCommander.Core.Navigation;
using UnityCommander.Core.Theming;
using UnityCommander.Dependencies;
using UnityCommander.Integration.Plugins;
using UnityCommander.Logging;
using UnityCommander.Logging.Abstractions;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Filters;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Logging.Sinks;
using UnityCommander.Modules.BottomPanel;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Docking.Services;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.SettingsPanel;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Modules.Viewer;
using UnityCommander.Modules.Viewer.Views;
using UnityCommander.Modules.WebBrowser;
using UnityCommander.Operation;
using UnityCommander.Ribbon.Core.Services;
using UnityCommander.Services;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using UnityCommander.Services.Interfaces.Settings;
using UnityCommander.Services.Selection;
using UnityCommander.Services.Settings;
using UnityCommander.Sinks;
using UnityCommander.ViewModels;
using UnityCommander.ViewModels.Dialogs;
using UnityCommander.Views;
using UnityCommander.Views.CopyDialogs;

namespace UnityCommander
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            var catalog = new ThemeCatalog();

            ThemeManager.Initialize(catalog, "Material");

            //var resources =
            //    ThemeManager.GetResourceUris()
            //        .Concat(ModuleResources.ResourceUris);

            foreach (var dictionary in SharedDictionaryManager.Load(ModuleResources.ResourceUris))
            {
                Resources.MergedDictionaries.Add(dictionary);
            }

            return this.Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            AppInfrastructureRegistration.Register(containerRegistry);
            CLIInfrastructureRegistration.Register(containerRegistry);
            CommandRegistration.Register(containerRegistry);
            ConsoleCommandRegistration.Register(containerRegistry);
            DialogModuleRegistration.Register(containerRegistry);
            FilePanelRegistration.Register(containerRegistry);
            LoggingModuleRegistration.Register(containerRegistry);
            PluginModuleRegistration.Register(containerRegistry);
            AiRegistration.Register(containerRegistry);
            AutocompleteRegistration.Register(containerRegistry);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }
   
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Модули 
            moduleCatalog.AddModule<FilePanelModule>();       
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<ViewerModule>();
            moduleCatalog.AddModule<SettingsPanelModule>();
            moduleCatalog.AddModule<WebBrowserModule>();
            moduleCatalog.AddModule<BottomPanelModule>();

            // Регистрация команд модулей
            moduleCatalog.AddModule<FilePanelCommandModule>(); // Команды

            // Инициализация после загрузки все модулей
            moduleCatalog.AddModule<AppLoadModule>();
        }
    }
}
