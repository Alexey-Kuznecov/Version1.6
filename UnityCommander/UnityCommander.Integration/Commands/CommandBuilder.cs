
namespace UnityCommander.Integration.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using UnityCommander.Common.Commands;
    using UnityCommander.Core;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Mvvm.Base;

    /// <summary>
    /// Построитель команд подключаемых модулей.
    /// </summary>
    public class CommandBuilder
    {
        /// <summary>
        /// Содержит команды плагина.
        /// </summary>
        private readonly List<BaseCommand> globalCommands = new ();

        /// <summary>
        /// The plugin commands.
        /// </summary>
        private readonly List<GlobalCommand> pluginCommands = new ();

        /// <summary>
        /// Регистрирует команды плагина которые будут переопределять команды по умолчанию.
        /// Например, можно переопределить команду которая отвечает за копирования файлов или папок. 
        /// </summary>
        /// <typeparam name="TO">
        /// To это Type Override. Методы этого класса плагина переопределяют команды по умолчанию.
        /// </typeparam>
        /// <typeparam name="TV">
        /// Tv это Type Virtual. Команды этого класса будут замещены командами плагина.
        /// </typeparam>
        public void Register<TO, TV>() where TO : TV, new ()
        {
            this.globalCommands.Add(new TO() as BaseCommand);
        }

        /// <summary>
        /// Регистрирует команду с параметрами. Внимание, аргументы должны совпадать с сигнатурой реализуемых интерфейсов.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <param name="parameters">
        /// Параметр(ы) метода интерфейса который реализуется <c>TImpl</c> типом.
        /// </param>
        /// <typeparam name="TInt">
        /// Где <c>TInt</c> это Type Interface. Методы буду искаться в соответствии с указанным интерфейсом в классе который справа. 
        /// </typeparam>
        /// <typeparam name="TImpl">
        /// Где <c>TImpl</c> это Type Implementation. Класс реализующий интерфейс слева, подбор параметров будет осуществляться здесь.
        /// </typeparam>
        public void RegisterWithArgument<TInt, TImpl>(TImpl implementation, object parameters) where TImpl : IPluginService
        {
            //var command = new GlobalCommand
            //{
            //    Name = implementation.GetType().FullName,
            //    Command = new GlobalCommandExecute(
            //        (parameter) =>
            //            {
            //                var map = implementation.GetType().GetInterfaceMap(typeof(TInt));

            //                foreach (var mapTargetMethod in map.TargetMethods)
            //                {
            //                    foreach (var parameterInfo in mapTargetMethod.GetParameters())
            //                    {
            //                        var intType = parameterInfo.ParameterType;
            //                        var implType = parameters.GetType();

            //                        if (implType.BaseType != intType) continue;
            //                        var method = implementation.GetType().GetMethod(mapTargetMethod.Name);
            //                        method?.Invoke(implementation, new[] { parameters });
            //                    }
            //                }
            //            }),
            //    CommandParameter = parameters,
            //};

            //this.pluginCommands.Add(command);
        }

        /// <summary>
        /// Получает зарегистрированные команды которые реализуются плагином.
        /// </summary>
        /// <returns>
        /// Команды подключаемых модулей.
        /// </returns>
        public IEnumerable<BaseCommand> GetCommands() => this.globalCommands;

        /// <summary>
        /// The get plugin commands.
        /// </summary>
        /// <returns>
        /// Команды подключаемых модулей.
        /// </returns>
        public IEnumerable<ICommandBase> GetPluginCommands() => this.pluginCommands;
    }
}
