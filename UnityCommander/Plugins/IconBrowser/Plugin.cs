
using IconBrowser.Commands;
using IconBrowser.Converters;
using IconBrowser.Services;
using IconBrowser.Services.Search;
using IconBrowser.ViewModels;
using IconBrowser.Views;
using IconMaker.Core.ImportExport;
using IconMaker.Core.Services;
using IconMaker.Core.Storage;
using Microsoft.Extensions.DependencyInjection;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using System;
using System.IO;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Common.Dialog;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Ribbon.Abstractions.Models;

[assembly: PluginInfo(
    name: "Icon Maker Plugin",
    developerId: "icon-maker-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Плагин для создания иконок"
)]
namespace IconBrowser
{
    public class Plugin : IPlugin, IDisposable
    {
        private ILogger _logger;

        public string Name => "icon_maker";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "plugins",
                "IconBrowser",
                "Data");

            init.RegisterInstance(new IconPaths(path));
            init.RegisterSingleton<FileSystem>();

      
            init.RegisterSingleton<IIconStorage, JsonIconStorage>();
            init.RegisterSingleton<IIconStore, IconStore>();
            init.RegisterSingleton<IIconService, IconService>();
            init.RegisterSingleton<IIconSerializer, JsonIconSerializer>();
            init.RegisterSingleton<IIconImporter, SvgIconImporter>();

            init.RegisterSingleton<IIconThemeStorage, JsonIconThemeStorage>();
            init.RegisterSingleton<IIconThemeStore, IconThemeStore>();
            init.RegisterSingleton<IIconThemeService, IconThemeService>();
            init.RegisterSingleton<IThemeSerializer, JsonIconThemeSerializer>();

            init.RegisterSingleton<IconIndexBuilder>();
            init.RegisterSingleton<IconSearchIndex>();

            init.RegisterSingleton<IIconSearchService>(sp =>
            {
                var builder = sp.GetRequiredService<IconIndexBuilder>();
                var search = sp.GetRequiredService<IconSearchIndex>();

                return new IconSearchService(search, builder, path);
            });

            init.RegisterCommand(new CommandDefinition()
            {
                Id = "open-icon-maker",
                IconKey = "core.file",
                CommandType = typeof(OpenIconMakerWindowCommand),
            });

            init.ConfigureRibbon(r =>
            {
                r.Tab("home", "Главная")
                    .Group("tools", "Инструменты")
                        .Section("main", RibbonGroupLayout.Inline)
                            .Button("open-icon-maker", "directory.create2");
            });

            //init.RegisterDialog(
            //    new DialogDefinition(
            //        "icon_maker-1.0",
            //        typeof(IconBrowserControl), 
            //        typeof(IconBrowserViewModel)
            //));

            init.RegisterDialog(
                 new DialogDefinition(
                     "icon_maker-1.0-new",
                     typeof(IconMakerView),
                     typeof(IconMakerViewModel)
            ));

            init.RegisterSingleton<IconDefinitionCompiler>();
            init.RegisterSingleton<IconMakerIconSource>();
            init.RegisterIconSource<IconMakerIconSource>();
        }

        public void Start(IPluginContext context)
        {
            var loggerCreate = context.Services.Get<LoggerCreator>();
            _logger = loggerCreate.ForPlugin();
            
            _logger.Info($"{Name} is ready!!!");
        }

        public void Stop()
        {
            _logger = null;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
