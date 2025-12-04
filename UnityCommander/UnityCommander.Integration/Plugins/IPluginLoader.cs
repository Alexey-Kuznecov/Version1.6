
namespace UnityCommander.Integration.Plugins
{
    using System;
    using System.Collections.Generic;
    using Integration.Contracts;
    using Integration.Dialog;
    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Factories;

    /// <summary>
    /// Интерфейс загрузчика плагинов, отвечающий за обнаружение, загрузку, управление и выгрузку подключаемых модулей.
    /// </summary>
    /// <remarks>
    /// Этот интерфейс определяет контракт для работы с плагинами в системе UnityCommander.
    /// Он позволяет загружать плагины, управлять их ресурсами, получать доступ к их командам, сервисам и настройкам, 
    /// а также безопасно выгружать их при необходимости.
    ///
    /// <para><strong>Назначение:</strong></para>
    /// <list type="bullet">
    ///     <item>Обеспечивает гибкость и расширяемость системы за счет поддержки модульной архитектуры.</item>
    ///     <item>Позволяет динамически подключать и отключать функциональность без изменения основного кода.</item>
    ///     <item>Упрощает управление плагинами, абстрагируя детали их загрузки и инициализации.</item>
    /// </list>
    ///
    /// <para><strong>Преимущества использования:</strong></para>
    /// <list type="bullet">
    ///     <item>Упрощение поддержки и расширения функционала программы.</item>
    ///     <item>Обеспечение безопасности за счет изоляции плагинов от основного приложения.</item>
    ///     <item>Снижение зависимости между модулями, что способствует более чистой архитектуре.</item>
    /// </list>
    ///
    /// <para><strong>Где применяется:</strong></para>
    /// Этот интерфейс используется в системе управления плагинами UnityCommander для загрузки сторонних расширений, 
    /// включая пользовательские команды, обработчики файлов, дополнительные инструменты и сервисы.
    /// </remarks>
    public interface IPluginLoader
    {
        /// <summary>
        /// Проверяет наличие указанного DLL-файла плагина в загруженных модулях.
        /// </summary>
        /// <param name="nameDll">Имя файла DLL.</param>
        /// <returns>Возвращает <c>true</c>, если DLL-файл найден, иначе <c>false</c>.</returns>
        bool GetPluginDll(string nameDll);

        /// <summary>
        /// Выгружает текущий плагин и освобождает его ресурсы.
        /// </summary>
        /// <returns>Возвращает <c>true</c>, если плагин успешно выгружен, иначе <c>false</c>.</returns>
        bool Unload();


        /// <summary>
        /// Получает коллекцию сервисов, загруженных из подключаемых плагинов.
        /// </summary>
        /// <typeparam name="T">Тип требуемого сервиса. Должен реализовывать <see cref="IPluginService"/>.</typeparam>
        /// <returns>Возвращает коллекцию загруженных сервисов указанного типа.</returns>
        IEnumerable<T> GetServices<T>() where T : IPluginService;

        /// <summary>
        /// Получает коллекцию дескрипторов плагинов, доступных в системе.
        /// </summary>
        /// <returns>Коллекция дескрипторов плагинов <see cref="IPluginDescriptor"/>.</returns>
        IEnumerable<IPluginDescriptor> GetDescriptors();

        /// <summary>
        /// Получает коллекцию диалоговых сервисов, доступных в плагинах.
        /// </summary>
        /// <returns>Коллекция сервисов диалогов <see cref="IDialogService"/>.</returns>
        IEnumerable<IDialogService> GetDialogs();

        /// <summary>
        /// Получает коллекцию команд, доступных в загруженных плагинах.
        /// </summary>
        /// <returns>Коллекция команд <see cref="ICommandBase"/>.</returns>
        IEnumerable<ICommandBase> GetPluginCommands();

        /// <summary>
        /// Получает коллекцию базовых команд, доступных в плагинах.
        /// </summary>
        /// <returns>Коллекция базовых команд <see cref="BaseCommand"/>.</returns>
        IEnumerable<BaseCommand> GetCommands();

        /// <summary>
        /// Получает связанные с плагином типы, используемые при его работе.
        /// </summary>
        /// <returns>Объект <see cref="AssociatedTypes"/>, содержащий информацию о связанных типах.</returns>
        AssociatedTypes GetAssociatedTypes();
    }
}
