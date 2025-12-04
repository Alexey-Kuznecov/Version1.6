#define DEBUG

namespace UnityCommander.Integration.Plugins
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Integration.Contracts;
    using Integration.Dialog;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    /// <summary>
    ///  Это основной класс, который отвечает за загрузку плагинов, их регистрацию
    ///  и создание контекста для каждого плагина. Он инкапсулирует логику загрузки 
    ///  плагинов и управления их состоянием (например, выгрузка плагинов).
    /// </summary>
    public class PluginLoaderService : IPluginLoaderService
    {
        #region Поля и коллекции

        /// <summary>
        /// Статическая коллекция всех загрузчиков плагинов, доступных в системе.
        /// </summary>
        private static readonly List<IPluginLoader> PluginLoaders = new List<IPluginLoader>();

        /// <summary>
        /// Коллекция контекстов загруженных плагинов, содержащих информацию о загруженных типах и возможностях.
        /// </summary>
        private readonly List<IPluginContext> pluginContexts = new();

        /// <summary>
        /// Коллекция загруженных плагинов, хранящихся в расширенных контейнерах,
        /// содержащих информацию о загрузчике, контексте и метаданных.
        /// </summary>
        private readonly List<ExtendedPluginContainer> loadedPlugins = new List<ExtendedPluginContainer>();

        /// <summary>
        /// Объект для потокобезопасного доступа к коллекции загруженных плагинов.
        /// </summary>
        private object _loadedPlugins;

        /// <summary>
        /// Путь, по которому находятся плагины.
        /// </summary>
        private string _pluginPath;

#if DEBUG
        /// <summary>
        /// Логгер для текущего класса, используется для записи логов и отслеживания событий в приложении.
        /// </summary>
        /// <remarks>
        /// Это статическое поле инициализирует экземпляр логгера с помощью библиотеки NLog.
        /// Он автоматически привязывается к классу, в котором используется, что позволяет логировать события
        /// с указанием точного места в коде (например, имени класса) для лучшей диагностики.
        /// </remarks>
        //private static Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        #endregion

        #region Конструктор

        /// <summary>
        /// Инициализирует сервис загрузки плагинов с указанным путем для поиска плагинов.
        /// </summary>
        /// <param name="pluginPath">Путь к директории, где находятся плагины.</param>
        public PluginLoaderService(string pluginPath)
        {
            _pluginPath = pluginPath;
        }


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PluginLoaderService"/>.
        /// Отвечает за обнаружение, загрузку и управление плагинами в системе.
        /// </summary>
        public PluginLoaderService()
        {
            // Добавляем обработчик для разрешения сборок, чтобы загружать зависимости плагинов при необходимости.
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            // Определяем путь к корневой директории приложения
            string mainPath = _pluginPath ?? GetMainPath();
            string pluginsDir = Path.Combine(mainPath, "plugins");

            // Проверяем наличие каталога с плагинами
            if (!Directory.Exists(pluginsDir))
            {
                Console.WriteLine("Plugins directory not found.");
                return;
            }

            // Загружаем все доступные плагины из каталога
            LoadPlugins(pluginsDir);

            // Создаём контекст для загруженных плагинов, обеспечивая их инициализацию и доступ к службам
            this.CreatePluginContext();

            // Настройка логгера Nlog
            //NLogSettings.Configure("Logs", "UnityCommander");
            //_logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Загрузка плагинов

        /// <summary>
        /// Загружает все плагины из указанной директории.
        /// Каждая папка плагина должна содержать DLL с именем, совпадающим с названием папки.
        /// </summary>
        /// <param name="pluginDirectory">Путь к папке с плагинами.</param>
        public void LoadPlugins(string pluginDirectory)
        {
            try
            {
                foreach (var dir in GetPluginDirectories(pluginDirectory))
                {
                    string pluginDllPath = GetPluginDllPath(dir);

                    try
                    {
                        if (File.Exists(pluginDllPath))
                        {
                            // Проверка расширения файла на ".dll"
                            if (Path.GetExtension(pluginDllPath).ToLower() != ".dll")
                            {
                                throw new InvalidPluginException("Структура файла неверна. Файл должен иметь расширение .dll.");
                            }

                            LoadAndRegisterPlugin(dir, pluginDllPath);
                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine($"Плагин DLL не найден: {pluginDllPath}");
#endif
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG1
                        // Логируем исключение в режиме отладки
                        _logger.Error(ex, $"Ошибка при загрузке плагина: {pluginDllPath}");
#endif
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG1
                // Логируем исключение в режиме отладки
                _logger.Error(ex, "Ошибка при обработке плагинов");
#endif
            }
        }

        #region Вспомогательные методы

        /// <summary>
        /// Получает список директорий с плагинами.
        /// </summary>
        /// <param name="pluginDirectory">Путь к директории с плагинами.</param>
        /// <returns>Массив директорий.</returns>
        private string[] GetPluginDirectories(string pluginDirectory)
        {
            return Directory.GetDirectories(pluginDirectory);
        }

        /// <summary>
        /// Формирует путь к DLL файлу плагина.
        /// </summary>
        /// <param name="pluginDirectory">Папка плагина.</param>
        /// <returns>Путь к DLL файлу плагина.</returns>
        private string GetPluginDllPath(string pluginDirectory)
        {
            string dirName = Path.GetFileName(pluginDirectory);
            return Path.Combine(pluginDirectory, "net9.0-windows", $"{dirName}.dll");
        }

        #endregion

        #region Регистрация и выполнение плагинов

        /// <summary>
        /// Загружает и регистрирует плагин в системе.
        /// </summary>
        /// <param name="pluginDirectory">Папка плагина.</param>
        /// <param name="pluginDllPath">Путь к DLL файлу плагина.</param>
        private void LoadAndRegisterPlugin(string pluginDirectory, string pluginDllPath)
        {
            string pluginName = Path.GetFileName(pluginDirectory);

            // Создаем контекст плагина, который хранит связанные типы
            var pluginContext = new PluginContext(new AssociatedTypes());

            // Создаем загрузчик плагинов
            var pluginLoader = new PluginLoader();

            // Создаем расширенный контейнер для управления плагином
            var pluginContainer = new ExtendedPluginContainer(
                pluginName: pluginName,
                assemblyPath: pluginDllPath,
                loadContext: new AssemblyLoadContext("pluginLoadContext"),
                pluginContext: pluginContext,
                pluginLoader: pluginLoader
            );

            // Загружаем и выгружаем плагин
            pluginLoader.ExecuteAndUnload(pluginDllPath);

            // Добавляем загруженный плагин в список активных
            this.loadedPlugins.Add(pluginContainer);

            Console.WriteLine($"Плагин {pluginContainer.GetPluginName()} загружен.");
        }

        #endregion

        #endregion

        #region AssemblyResolutionHandling
        /// <summary>
        /// Метод OnAssemblyResolve используется для обработки событий загрузки сборок, 
        /// позволяя вручную загружать зависимости плагинов из заданного каталога 
        /// (например, из папки с плагинами) вместо стандартного поиска.
        /// </summary>
        /// <param name="sender">
        /// Объект, инициировавший событие загрузки сборки (обычно это сам процесс или приложение)
        /// </param>
        /// <param name="args">
        /// Аргументы события, содержащие информацию о запросе на загрузку сборки,
        /// включая название сборки (AssemblyName)
        /// </param>
        /// <returns></returns>
        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Debug.WriteLine($"Запрашивается загрузка: {args.Name}");
            string assemblyName = new AssemblyName(args.Name).Name;

            // Получаем путь к сборке
            string assemblyPath = GetAssemblyPath(assemblyName);

            // Загружаем сборку, если путь найден
            return LoadAssembly(assemblyPath);
        }
        #endregion

        #region GetAssemblyPath
        /// <summary>
        /// Получает путь к сборке из папки плагинов и основной директории.
        /// </summary>
        /// <param name="assemblyName">Название сборки.</param>
        /// <returns>Путь к сборке, если она найдена, иначе null.</returns>
        private string GetAssemblyPath(string assemblyName)
        {
            string pluginDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
            string assemblyPath = Path.Combine(pluginDirectory, assemblyName + ".dll");

            // Если сборка найдена в папке плагинов, возвращаем путь
            if (File.Exists(assemblyPath))
            {
                return assemblyPath;
            }

            // Ищем в основной директории
            string mainDirectory = AppContext.BaseDirectory;
            assemblyPath = Path.Combine(mainDirectory, assemblyName + ".dll");

            // Если сборка найдена в основной директории, возвращаем путь
            if (File.Exists(assemblyPath))
            {
                return assemblyPath;
            }

            return null; // Сборка не найдена
        }
        #endregion

        #region LoadAssembly
        /// <summary>
        /// Загружает сборку по указанному пути.
        /// </summary>
        /// <param name="assemblyPath">Путь к сборке.</param>
        /// <returns>Загруженная сборка или null, если сборка не найдена.</returns>
        private Assembly LoadAssembly(string assemblyPath)
        {
            if (!string.IsNullOrEmpty(assemblyPath) && File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }

            return null; // Сборка не найдена
        }
        #endregion

        #region Работа с плагинами

        #region Получение информации о плагинах

        // <summary>
        /// Возвращает список всех загруженных плагинов.
        /// </summary>
        public IReadOnlyList<PluginContainer> GetLoadedPlugins()
            => this.loadedPlugins.AsReadOnly();

        /// <summary>
        /// Возвращает базовый путь к приложению, который используется для поиска плагинов.
        /// </summary>
        /// <returns>Строка с путем к корневой директории приложения.</returns>
        private string GetMainPath() => AppContext.BaseDirectory; // Универсальный путь, учитывает публикацию и окружение  

        /// <summary>
        /// Получает интерфейсы для реализации функционала плагинов.
        /// </summary>
        /// <returns>
        /// Список реализаций интерфейса <see cref="IDialogService"/>.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogService()
            => PluginLoaders.SelectMany(loader => loader.GetDialogs());

        /// <summary>
        /// Получает интерфейсы для конфигурации плагинов.
        /// </summary>
        /// <returns>
        /// Список интерфейсов <see cref="IPluginDescriptor"/> для настройки плагинов.
        /// </returns>
        public IEnumerable<IPluginDescriptor> GetPluginDescriptors()
            => PluginLoaders.SelectMany(loader => loader.GetDescriptors());

        /// <summary>
        /// Получает коллекцию контекстов загруженных плагинов.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="IEnumerable{IPluginContext}"/> — коллекцию контекстов плагинов,
        /// которые содержат информацию о загруженных типах и возможностях.
        /// </returns>
        public IEnumerable<IPluginContext> GetPluginContext() => pluginContexts;

        #endregion

        /// <summary>
        /// Создает контексты для всех загруженных плагинов.
        /// </summary>
        public void CreatePluginContext()
        {
            foreach (var loader in loadedPlugins)
            {
                var pluginContext = BuildPluginContext(loader);
                this.pluginContexts.Add(pluginContext);
            }
        }

        /// <summary>
        /// Создает и настраивает контекст плагина.
        /// </summary>
        /// <param name="loader">Загрузчик плагина.</param>
        /// <returns>Настроенный контекст плагина.</returns>
        private PluginContext BuildPluginContext(ExtendedPluginContainer loader)
        {
            var pluginContext = new PluginContext(loader.PluginLoader.GetAssociatedTypes());
            var pluginContextBuilder = new PluginContextBuilder(pluginContext);

            AddServicesToContext(pluginContextBuilder, loader);
            AddCommandsToContext(pluginContextBuilder, loader);

            return pluginContext;
        }

        /// <summary>
        /// Добавляет зарегистрированные сервисы плагина в контекст.
        /// </summary>
        /// <param name="builder">Строитель контекста плагина.</param>
        /// <param name="loader">Загрузчик плагина.</param>
        private void AddServicesToContext(PluginContextBuilder builder, ExtendedPluginContainer loader)
        {
            foreach (var columnBuilder in loader.PluginLoader.GetServices<IColumnBuilder>())
            {
                builder.AddColumn(columnBuilder);
            }

            foreach (var optionBuilder in loader.PluginLoader.GetServices<IOptionBuilder>())
            {
                builder.AddOption(optionBuilder);
            }

            foreach (var pluginSettings in loader.PluginLoader.GetServices<IPluginSettings>())
            {
                // Если нужна логика обработки pluginSettings — добавить сюда
            }
        }

        /// <summary>
        /// Добавляет команды плагина в контекст.
        /// </summary>
        /// <param name="builder">Строитель контекста плагина.</param>
        /// <param name="loader">Загрузчик плагина.</param>
        private void AddCommandsToContext(PluginContextBuilder builder, ExtendedPluginContainer loader)
        {
            builder.AddPluginCommand(loader.PluginLoader.GetPluginCommands().ToList());
            builder.AddCommand(loader.PluginLoader.GetCommands().ToList());
        }

        #endregion

        #region Управление загрузкой и выгрузкой плагинов

        /// <summary>
        /// Осуществляет выгрузку всех плагинов из текущего списка плагинов.
        /// </summary>
        /// <remarks>
        /// Этот метод очищает хэш-таблицу плагинов и пытается выгрузить все плагины, находящиеся в коллекции. 
        /// Если хотя бы один плагин был успешно выгружен, метод возвращает <c>true</c>. 
        /// После выгрузки плагины удаляются из основной коллекции.
        /// </remarks>
        /// <returns>
        /// Возвращает <c>true</c>, если хотя бы один плагин был успешно выгружен, 
        /// и <c>false</c> в случае, если выгрузить плагины не удалось.
        /// </returns>
        public bool UnloadPlugins()
        {
            // Очищаем кеш плагинов перед выгрузкой
            this.ClearHashTable();

            // Выгружаем плагины
            var pluginsToUnload = PluginLoaders.Where(p => p.Unload()).ToList();

            if (pluginsToUnload.Any())
            {
                PluginLoaders.RemoveAll(p => pluginsToUnload.Contains(p));
                Debug.WriteLine("Часть или все плагины были успешно выгружены.");
                return true;
            }

            Debug.WriteLine("Не удалось выгрузить ни один плагин.");
            return false;
        }

        /// <summary>
        /// Выгружает конкретный плагин из системы.
        /// </summary>
        /// <param name="pluginToUnload">Плагин, который необходимо выгрузить.</param>
        /// <returns>
        /// Возвращает <c>true</c>, если плагин был успешно выгружен, 
        /// и <c>false</c>, если плагин не был найден или не удалось его выгрузить.
        /// </returns>
        public bool UnloadPlugin(IPluginLoader pluginToUnload)
        {
            if (pluginToUnload == null || !pluginToUnload.Unload())
                return false;

            ((List<IPluginLoader>)PluginLoaders).Remove(pluginToUnload);
            return true;
        }

        /// <summary>
        /// Выгружает указанный плагин по имени.
        /// </summary>
        /// <param name="pluginName">Имя плагина.</param>
        /// <returns>True, если выгрузка успешна, иначе false.</returns>
        public bool UnloadPlugin(string pluginName)
        {
            if (this.loadedPlugins.FirstOrDefault(p => p.PluginName == pluginName)
                is not ExtendedPluginContainer extendedContainer)
                return false;

            return UnloadPlugin(extendedContainer.PluginLoader);
        }

        /// <summary>
        /// Перезагружает все плагины.
        /// </summary>
        /// <param name="pluginDirectory">Путь к директории с плагинами.</param>
        public void ReloadPlugins(string pluginDirectory)
        {
            UnloadPlugins();  // Сначала выгружаем старые плагины
            LoadPlugins(pluginDirectory);  // Затем загружаем новые
        }

        #endregion

        #region Обслуживание кеша

        /// <summary>
        /// Очищает внутренний кеш атрибутов в ReflectTypeDescriptionProvider.
        /// Это необходимо для предотвращения утечек памяти при динамической загрузке плагинов.
        /// </summary>
        private void ClearHashTable()
        {
            var typeConverterAssembly = typeof(TypeConverter).Assembly;
            var reflectTypeDescriptionProviderType = typeConverterAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");

            var attributeCacheField = reflectTypeDescriptionProviderType?
                .GetField("s_attributeCache", BindingFlags.Static | BindingFlags.NonPublic);

            (attributeCacheField?.GetValue(null) as Hashtable)?.Clear();
        }

        #endregion
    }
}
