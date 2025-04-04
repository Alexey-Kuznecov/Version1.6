namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Plugins;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// Класс <see cref="PluginLoader"/> предназначен для загрузки, выполнения и выгрузки плагинов 
    /// в изолированной среде. Реализует интерфейсы <see cref="IPluginLoader"/> и <see cref="IPluginServicesRegister"/>.
    /// </summary>
    /// <remarks>
    /// <para>Этот класс отвечает за динамическую загрузку библиотек плагинов, управление их жизненным циклом 
    /// и обеспечение их изоляции от основного приложения.</para>
    /// <para>Плагины должны соответствовать стандартной структуре: быть DLL-файлом и реализовывать 
    /// интерфейс <see cref="IPlugin"/>.</para>
    /// </remarks>
    public class PluginLoader : IPluginLoader, IPluginServicesRegister
    {
        #region Поля и Свойства

        /// <summary>
        /// Коллекция зарегистрированных сервисов плагинов.
        /// Эти сервисы предоставляются заыгруженными плагинами и могут быть использованы в системе.
        /// </summary>
        private readonly IReadOnlyList<IEnumerable<IPluginService>> pluginsRegistered = new List<IEnumerable<IPluginService>>();

        /// <summary>
        /// Объект, содержащий информацию о связанных типах,
        /// используемых плагинами, таких как команды, диалоги и настройки.
        /// </summary>
        private readonly AssociatedTypes associatedTypes = new();

        #region Загруженные контракты

        /// <summary>
        /// Коллекция зарегистрированных сервисов диалоговых окон, 
        /// предоставляемых плагинами.
        /// </summary>
        private IEnumerable<IDialogService> dialogService;

        /// <summary>
        /// Коллекция дескрипторов загруженных плагинов, 
        /// содержащих метаданные (имя, описание, версия и т. д.).
        /// </summary>
        private IEnumerable<IPluginDescriptor> pluginDescriptors;

        /// <summary>
        /// Коллекция объектов настроек плагинов,
        /// позволяющих управлять конфигурацией загруженных модулей.
        /// </summary>
        private IEnumerable<IPluginSettings> pluginSettings;

        /// <summary>
        /// Коллекция зарегистрированных глобальных команд, предоставленных плагинами.
        /// Эти команды могут использоваться во всей системе.
        /// </summary>
        private IEnumerable<BaseCommand> commandBuilders = new List<BaseCommand>();

        /// <summary>
        /// Коллекция команд, относящихся к конкретным плагинам.
        /// Позволяет расширять функциональность через команды, реализованные в плагинах.
        /// </summary>
        private IEnumerable<ICommandBase> pluginCommandsBuilder = new List<ICommandBase>();

        #endregion

        /// <summary>
        /// Контекст загрузки плагина, используемый для управления сборками и их изоляцией.
        /// </summary>
        private HostPluginLoadContext alc;

        /// <summary>
        /// Слабая ссылка на объект контекста загрузки плагина.
        /// Используется для безопасного управления памятью и выгрузки плагинов.
        /// </summary>
        private WeakReference weakReference;

        /// <summary>
        /// Коллекция ресурсов плагинов (например, стили, шаблоны), 
        /// которые могут быть загружены и использованы в приложении.
        /// </summary>
        private HashSet<ResourceDictionary> pluginResources = new();

        /// <summary>
        /// Провайдер сервисов, созданный на основе загруженных плагинов.
        /// Позволяет получать экземпляры зарегистрированных сервисов.
        /// </summary>
        private ServiceProvider serviceProvider;

        #endregion

        #region Методы для получения данных о плагинах

        /// <summary>
        /// Получает коллекцию загружаемых служб, которые были найдены в подключаемых модулях.
        /// </summary>
        /// <typeparam name="T">
        /// Тип службы которую нужна получить.
        /// </typeparam>
        /// <returns>
        /// Возвращает коллекцию загружаемых служб.
        /// </returns>
        public IEnumerable<T> GetServices<T>() where T : IPluginService
        {
            return this.pluginsRegistered.Where(enumerable => enumerable is IEnumerable<T>)
                .SelectMany(registered => registered)
                .Cast<T>();
        }

        /// <summary>
        /// Получает коллекцию метаданных плагинов, 
        /// предоставляющих информацию о загруженных модулях.
        /// </summary>
        /// <returns>Коллекция объектов <see cref="IPluginDescriptor"/>.</returns>
        public IEnumerable<IPluginDescriptor> GetDescriptors() => this.pluginDescriptors;

        /// <summary>
        /// Получает коллекцию сервисов диалоговых окон, 
        /// реализованных в загруженных плагинах.
        /// </summary>
        /// <returns>Коллекция сервисов <see cref="IDialogService"/>.</returns>
        public IEnumerable<IDialogService> GetDialogs() => this.dialogService;

        /// <summary>
        /// Получает коллекцию команд, относящихся к конкретным плагинам,
        /// и предназначенных для расширения их функциональности.
        /// </summary>
        /// <returns>Коллекция объектов <see cref="ICommandBase"/>.</returns>
        public IEnumerable<ICommandBase> GetPluginCommands() => this.pluginCommandsBuilder;

        /// <summary>
        /// Получает коллекцию глобальных команд,
        /// доступных во всей системе независимо от конкретного плагина.
        /// </summary>
        /// <returns>Коллекция объектов <see cref="BaseCommand"/>.</returns>
        public IEnumerable<BaseCommand> GetCommands() => this.commandBuilders;

        #endregion

        #region Загрузка и инициализация плагина

        /// <summary>
        /// Загружает и инициализирует плагин, а затем регистрирует его сервисы.
        /// </summary>
        /// <param name="assemblyPath">Путь к сборке плагина.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ExecuteAndUnload(string assemblyPath)
        {
            this.InitializePluginContext(assemblyPath);
            Assembly assembly = this.LoadPluginAssembly(assemblyPath);
            this.ExtractPluginResources(assembly);
            string pluginToken = this.GeneratePluginToken();

            this.InitializePluginFactories(assembly, pluginToken);
            this.RegisterPluginServices();
        }

        /// <summary>
        /// Инициализирует контекст загрузки плагина.
        /// </summary>
        /// <param name="assemblyPath">Путь к сборке плагина.</param>
        private void InitializePluginContext(string assemblyPath)
        {
            this.alc = new HostPluginLoadContext(assemblyPath);
            this.weakReference = new WeakReference(this.alc);
        }

        /// <summary>
        /// Загружает сборку плагина из указанного пути.
        /// </summary>
        /// <param name="assemblyPath">Путь к сборке.</param>
        /// <returns>Загруженная сборка.</returns>
        private Assembly LoadPluginAssembly(string assemblyPath)
        {
            return this.alc.LoadFromAssemblyPath(assemblyPath);
        }

        /// <summary>
        /// Извлекает ресурсы из загруженной сборки плагина.
        /// </summary>
        /// <param name="assembly">Загруженная сборка плагина.</param>
        private void ExtractPluginResources(Assembly assembly)
        {
            this.GetPluginResources(assembly);
        }

        /// <summary>
        /// Генерирует уникальный токен для идентификации плагина.
        /// </summary>
        /// <returns>Сгенерированный уникальный токен.</returns>
        private string GeneratePluginToken()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Инициализирует фабрики плагинов, регистрирует команды и связанные типы.
        /// </summary>
        /// <param name="assembly">Загруженная сборка плагина.</param>
        /// <param name="token">Уникальный токен плагина.</param>
        private void InitializePluginFactories(Assembly assembly, string token)
        {
            var services = new ServiceCollection();
            var typesRegister = new AssociatedTypesRegister(this.associatedTypes);

            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPluginFactory).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var plugin = Activator.CreateInstance(type) as IPluginFactory;
                    plugin?.Configure(services);
                    plugin?.SetAssociatedTypes(typesRegister);
                    plugin?.SetToken(token);

                    if (typeof(ICommandFactory).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var commandBuilder = new CommandBuilder();
                        var command = (ICommandFactory)plugin;
                        command?.CommandFactory(commandBuilder);
                        this.commandBuilders = commandBuilder.GetCommands();
                        this.pluginCommandsBuilder = commandBuilder.GetPluginCommands();
                    }
                }
            }

            this.serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Регистрирует сервисы плагинов в системе, подключая необходимые реализации для 
        /// работы с различными функциональными частями плагинов, такими как настройки, 
        /// описания и диалоговые сервисы.
        /// </summary>
        /// <remarks>
        /// Этот метод отвечает за получение сервисов из контейнера зависимостей и их регистрацию 
        /// в системе с помощью обобщенного метода <see cref="Register{TI, TS}"/>. Он регистрирует
        /// различные сервисы, такие как <see cref="IDialogService"/>, <see cref="IPluginDescriptor"/> и другие, 
        /// для дальнейшего использования в процессе работы с плагинами.
        /// </remarks>
        private void RegisterPluginServices()
        {
            // Получаем все сервисы диалогов из контейнера зависимостей
            this.dialogService = this.serviceProvider.GetServices<IDialogService>();

            // Получаем все дескрипторы плагинов из контейнера зависимостей
            this.pluginDescriptors = this.serviceProvider.GetServices<IPluginDescriptor>();

            // Регистрируем различные сервисы плагинов в систему, связывая их с базовым интерфейсом IPluginService
            this.Register<IDialogService, IPluginService>(this.dialogService);
            this.Register<IPluginDescriptor, IPluginService>(this.pluginDescriptors);
            this.Register<IPluginSettings, IPluginService>(this.serviceProvider.GetServices<IPluginSettings>());
            this.Register<IColumnBuilder, IPluginService>(this.serviceProvider.GetServices<IColumnBuilder>());
            this.Register<IOptionBuilder, IPluginService>(this.serviceProvider.GetServices<IOptionBuilder>());
        }

        #endregion

        #region Регистрация и выгрузка плагинов

        /// <summary>
        /// Регистрирует службы подключаемых модулей для управления настройками,
        /// получения описания и внедрения реализации расширяющий функционал программы. 
        /// </summary>
        /// <param name="instance">
        /// Экземпляр класса подключаемого модуля который реализует контракт, например <see cref="IDialogService"/>.
        /// </param>
        /// <typeparam name="TI">
        /// Тип службы наследует общий для всех служб <see cref="IPluginService"/> контракт.
        /// </typeparam>
        /// <typeparam name="TS">
        /// Общий контракт служб, на данный момент нужен лишь для того чтобы регистрировать службы в одной категории.
        /// А так же избежать ошибок связанных приведением к типу.
        /// </typeparam>
        public void Register<TI, TS>(IEnumerable<TS> instance)
            where TI : TS
        {
            if (instance is IEnumerable<IPluginService> registered)
            {
                ((List<IEnumerable<IPluginService>>)this.pluginsRegistered).Add(registered);
            }

        }

        /// <summary>
        /// Выгружает плагин из памяти, освобождая все связанные ресурсы.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="bool"/>, указывающий, был ли плагин успешно выгружен.
        /// </returns>
        /// <remarks>
        /// Метод выгружает все ресурсы, связанные с плагином, включая:
        /// - Освобождение контекста загрузки и ссылок на дескрипторы плагинов.
        /// - Удаление ресурсов плагина из глобальных ресурсов приложения.
        /// - Принудительный запуск сборщика мусора для освобождения памяти.
        /// </remarks>
        public bool Unload()
        {
            // Освобождаем контекст загрузки
            this.alc.Unload();

            // Обнуляем ссылки на дескрипторы плагинов и сервисы
            this.pluginDescriptors = null;
            this.dialogService = null;
            this.alc = null;

            // Удаляем ресурсы плагина из глобального словаря ресурсов приложения, если они существуют
            if (this.pluginResources != null && this.pluginResources.Count > 0)
            {
                foreach (var resource in this.pluginResources)
                {
                    var dictionary = Application.Current.Resources.MergedDictionaries;
                    dictionary.Remove(resource);
                }
            }

            this.pluginResources = null;

            // Принудительный запуск сборщика мусора для освобождения памяти
            // Метод используется для ускоренного освобождения ресурсов, однако в реальных условиях 
            // стоит использовать его с осторожностью, так как он может влиять на производительность.
            for (var i = 0; this.weakReference.IsAlive && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            // Логируем результат выгрузки плагина
            Debug.WriteLine($"Unload success: {!this.weakReference.IsAlive}");

            // Возвращаем результат: плагин выгружен, если слабая ссылка на него мертва
            return !this.weakReference.IsAlive;
        }

        #endregion

        #region Работа с ресурсами

        /// <summary>
        /// Загружает и регистрирует ресурсы плагина в глобальном словаре ресурсов WPF.
        /// </summary>
        /// <param name="assembly">
        /// Сборка, из которой будут извлечены ресурсы.
        /// </param>
        private void GetPluginResources(Assembly assembly)
        {
            // Получаем ресурсы из менеджера ресурсов плагинов
            this.pluginResources = PluginResourceManager.GetResourceDictionary(assembly);

            // Если ресурсы присутствуют, добавляем их в глобальные ресурсы приложения
            if (this.pluginResources?.Count != 0 && this.pluginResources != null)
            {
                var dictionary = Application.Current.Resources.MergedDictionaries;

                foreach (var resource in this.pluginResources)
                {
                    dictionary.Add(resource);
                }
            }
        }

        #endregion

        #region Утилитарные методы

        /// <summary>
        /// Получает зарегистрированные типы, ассоциированные с плагинами.
        /// </summary>
        /// <returns>
        /// Объект <see cref="AssociatedTypes"/>, содержащий информацию о зарегистрированных типах.
        /// </returns>
        public AssociatedTypes GetAssociatedTypes() => this.associatedTypes;

        /// <summary>
        /// Проверяет наличие DLL файла плагина в директории плагинов.
        /// </summary>
        /// <param name="nameDll">Имя DLL файла плагина (без пути).</param>
        /// <returns>
        /// Возвращает <c>true</c>, если DLL файл существует в директории плагинов, иначе <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">Выбрасывается, если передано пустое или null имя DLL.</exception>
        public bool GetPluginDll(string nameDll)
        {
            // Проверка на валидность имени DLL
            if (string.IsNullOrWhiteSpace(nameDll))
            {
                throw new ArgumentException("Имя DLL не может быть пустым или состоять только из пробелов.", nameof(nameDll));
            }

            // Формируем путь к директории плагинов
            string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");

            // Перебираем все доступные каталоги плагинов
            foreach (var pluginDir in Directory.GetDirectories(pluginsDirectory))
            {
                string potentialDllPath = Path.Combine(pluginDir, "net9.0-windows", nameDll);

                if (File.Exists(potentialDllPath))
                {
                    Debug.WriteLine($"Файл плагина найден: {potentialDllPath}");
                    return true;
                }
            }

            Debug.WriteLine($"Файл плагина '{nameDll}' не найден в каталоге плагинов.");
            return false;
        }

        #endregion
    }
}