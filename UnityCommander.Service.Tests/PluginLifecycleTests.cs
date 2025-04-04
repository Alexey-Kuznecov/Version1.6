namespace UnityCommander.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Moq;
    using Xunit;
    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Dialog;
    using UnityCommander.Services.Plugins;
    using UnityCommander.Integration.Contracts;
    using System.Runtime.CompilerServices;
    using UnityCommander.Services.Interfaces;

    public class PluginLifecycleTests
    {
        private readonly string _pluginDirectory = "F:\\UnityCommander\\Version3.9.7\\UnityCommander\\UnityCommander\\bin\\Debug\\net9.0-windows\\plugins";

        [Fact]
        public void Plugin_Loads_Successfully()
        {
            var pluginLoader = new PluginLoaderService();

            // Загружаем плагины
            pluginLoader.LoadPlugins(_pluginDirectory);

            // Получаем загруженные плагины
            var plugins = pluginLoader.GetPluginDescriptors();

            // Выводим их количество
            Console.WriteLine($"Количество загруженных плагинов: {plugins.Count()}");

            Assert.NotEmpty(plugins);
        }

        [Fact]
        public void Plugin_Registers_Services_Correctly()
        {
            var pluginLoader = new PluginLoaderService();
            var dialogServices = pluginLoader.GetDialogService();

            Assert.NotEmpty(dialogServices);
        }

        [Fact]
        public void Plugin_Unloads_Successfully()
        {
            // Создаём фейковый плагин
            var pluginPath = Path.Combine(_pluginDirectory, "plugin.dll");
            File.WriteAllText(pluginPath, "Some plugin data");

            var pluginLoader = new PluginLoaderService();
            pluginLoader.LoadPlugins(_pluginDirectory);

            var plugin = pluginLoader.UnloadPlugin("plugin");

            // Проверяем, что плагин был загружен
            Assert.NotNull(plugin);

            // Выгружаем плагин
            pluginLoader.UnloadPlugin("plugin");

            // Проверяем, что плагин был выгружен
            var unloadedPlugin = pluginLoader.UnloadPlugin("plugin");
            Assert.Null(unloadedPlugin);

            File.Delete(pluginPath);
        }

        [Fact]
        public void Plugin_Reloads_Correctly()
        {
            // Arrange: подготовка данных
            string pluginDirectory = @"путь_к_плагинам";  // укажи путь к папке с плагинами
            var pluginLoaderService = new PluginLoaderService();  // создаём сервис для загрузки плагинов

            // Act: 1) загрузка плагинов
            pluginLoaderService.LoadPlugins(pluginDirectory);  // вызываем метод загрузки плагинов

            // Проверяем, что плагины загружены
            var pluginsBeforeReload = pluginLoaderService.GetLoadedPlugins();  // получаем список загруженных плагинов
            Assert.NotEmpty(pluginsBeforeReload);  // проверяем, что плагины были загружены

            // Act: 2) выгрузка и перезагрузка плагинов
            pluginLoaderService.UnloadPlugins();  // метод для выгрузки плагинов
            pluginLoaderService.LoadPlugins(pluginDirectory);  // повторная загрузка плагинов

            // Assert: Проверка, что плагины успешно перезагружены
            var pluginsAfterReload = pluginLoaderService.GetLoadedPlugins();  // получаем список плагинов после перезагрузки
            Assert.NotEmpty(pluginsAfterReload);  // проверяем, что плагины были перезагружены

            // Проверка на равенство плагинов до и после перезагрузки
            Assert.Equal(pluginsBeforeReload.Count, pluginsAfterReload.Count);  // количество плагинов должно остаться одинаковым

            // Проверяем, что хотя бы один из плагинов перезагружен (например, проверка по имени плагина)
            var pluginBefore = pluginsBeforeReload.FirstOrDefault(p => p.GetPluginName() == "НекоторыйПлагин");
            var pluginAfter = pluginsAfterReload.FirstOrDefault(p => p.GetPluginName() == "НекоторыйПлагин");
            Assert.NotNull(pluginAfter);  // проверяем, что плагин с таким именем все еще существует
            Assert.Equal(pluginBefore?.AssemblyPath, pluginAfter?.AssemblyPath);  // проверяем, что путь к DLL совпадает
        }

        [Fact]
        public void Plugin_Isolated_From_Main_App()
        {
            var pluginLoader = new PluginLoaderService();
            pluginLoader.LoadPlugins(_pluginDirectory);

            var pluginContexts = pluginLoader.GetPluginContext();
            Assert.All(pluginContexts, context =>
            {
                Assert.NotNull(context);
            });
        }

        [Fact]
        public void Plugin_Handles_Invalid_Assembly()
        {
            Console.WriteLine("Тест Plugin_Handles_Invalid_Assembly начал выполнение!");
            var invalidPluginPath = Path.Combine(_pluginDirectory, "invalid.dll");
            File.WriteAllText(invalidPluginPath, "Invalid Data"); // Создаем некорректный файл

            var pluginLoader = new PluginLoaderService();

            Exception exception = Record.Exception(() => pluginLoader.LoadPlugins(_pluginDirectory));

            Console.WriteLine("Тест Plugin_Handles_Invalid_Assembly находится перед вызывом alert!");

            Assert.NotNull(exception); // Проверяем, что была выброшена ошибка
            File.Delete(invalidPluginPath);

            Console.WriteLine("Тест Plugin_Handles_Invalid_Assembly закончит выполнение!");
        }
    }
}
