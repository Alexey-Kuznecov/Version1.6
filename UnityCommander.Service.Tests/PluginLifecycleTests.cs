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
    using UnityCommander.Core.Exceptions;
    using NLog;

    public class PluginLifecycleTests
    {
        private readonly string _pluginDirectory = "F:\\UnityCommander\\Version3.9.7\\UnityCommander\\UnityCommander\\bin\\Debug\\net9.0-windows\\plugins";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Unit-тест проверяет корректную работу загрузчика плагинов при наличии 
        /// файла с расширением .dll, но не являющегося валидной .NET-сборкой.
        /// </summary>
        /// <remarks>
        /// Тест создает структуру плагина с фейковым .dll-файлом, затем
        /// вызывает метод <see cref="PluginLoaderService.LoadPlugins"/>.
        /// Ожидается, что исключение не будет выброшено, а загрузчик проигнорирует
        /// невалидный файл.
        /// </remarks>
        /// <author>Автор: [Твое Имя]</author>
        /// <created>Дата создания: 2025-04-05</created>
        /// <test>
        /// Arrange:
        /// - Создается папка InvalidPlugin/net9.0-windows.
        /// - В неё помещается текстовый файл с расширением .dll.
        /// Act:
        /// - Вызывается метод LoadPlugins.
        /// Assert:
        /// - Проверяется, что исключения не произошло.
        /// </test>
        [Fact]
        public void Plugin_Loads_Successfully()
        {
            // Arrange
            var pluginPath = PrepareFakePluginDirectory("InvalidPlugin", "dll");
            var pluginLoader = new PluginLoaderService(_pluginDirectory);

            // Act
            try
            {
                pluginLoader.LoadPlugins(_pluginDirectory);
            }
            catch (Exception)
            {
                Assert.True(false, "Исключение не должно было быть выброшено");
            } 

            // Assert
            Assert.True(true);

            // Clean up
            CleanupPluginTest(pluginPath);
        }

        [Fact]
        public void Plugin_Implements_IPlugin_Interface()
        {
            // Arrange
            var pluginDirectory = Path.Combine(_pluginDirectory, "MultiColumns");
            //Directory.CreateDirectory(pluginDirectory);

            var netFolder = Path.Combine(pluginDirectory, "net9.0-windows");
            //Directory.CreateDirectory(netFolder);

            var validPluginPath = Path.Combine(netFolder, "MultiColumns.dll");
            //File.WriteAllText(validPluginPath, "Dummy Plugin Data");

            var pluginLoader = new PluginLoaderService(_pluginDirectory);

            // Act
            pluginLoader.LoadPlugins(_pluginDirectory);

            // Assert
            var loadedPlugin = pluginLoader.GetLoadedPlugins().FirstOrDefault(p => p.GetPluginPath() == validPluginPath);
            Assert.NotNull(loadedPlugin);  // Плагин должен быть найден
            Assert.IsAssignableFrom<IPluginFactory>(loadedPlugin);  // Проверяем, что плагин реализует интерфейс IPlugin

            // Clean up
            //File.Delete(validPluginPath);
            //Directory.Delete(netFolder);
            //Directory.Delete(pluginDirectory);
        }

        #region Методы отчиски и подготовки

        private string PrepareFakePluginDirectory(string pluginName, string extension)
        {
            var pluginDirectory = _pluginDirectory;
            Directory.CreateDirectory(pluginDirectory);

            var netFolder = Path.Combine(pluginDirectory, $"{pluginName}\\net9.0-windows");
            Directory.CreateDirectory(netFolder);

            var pluginFilePath = Path.Combine(netFolder, $"{pluginName}.{extension}");
            File.WriteAllText(pluginFilePath, "This is not a valid DLL");

            return pluginFilePath;
        }

        private void CleanupPluginTest(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Путь к директории не может быть пустым.", nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
                return;

            try
            {
                // Удаление всех файлов и подпапок
                Directory.Delete(directoryPath, recursive: true);
                //Directory.Delete(directoryPath);
            }
            catch (IOException)
            {
                // Иногда файлы могут быть в использовании, можно добавить retry или лог
                Console.WriteLine($"Ошибка при удалении директории: {directoryPath}. Возможно, она используется.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет прав для удаления директории: {directoryPath}.");
            }
        }

        #endregion
    }
}
