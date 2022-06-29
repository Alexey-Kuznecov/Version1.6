
namespace UnityCommander.Integration.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;

    using AlexeyKuznecov.Library.Mvvm.Base;

    using UnityCommander.Common.Commands;
    using UnityCommander.Core;
    using UnityCommander.Core.Generators;
    using UnityCommander.Integration.Contracts;

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
            var instance = new TO() as BaseCommand;
            var baseInstance = Activator.CreateInstance<TV>() as BaseCommand;
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
            
                foreach (var method in instance?.GetType().GetMethods(Flags)!)
                {

                    if (!method.CustomAttributes.Any(p => p.AttributeType.Name == "GlobalCommandAttribute")) continue;

                    var action = default(Delegate); // Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                    var command = default(GlobalCommandExecute); // new GlobalCommandExecute(action, instance ?? instance2);

                    if (typeof(TO).GetMethod(method.Name)?.DeclaringType == typeof(TO))
                    {
                        action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), instance, method);
                        command = new GlobalCommandExecute(action, instance);
                    }
                    else
                    {
                        action = Delegate.CreateDelegate(DelegateTypeFactory.Create(method), baseInstance, method);
                        command = new GlobalCommandExecute(action, baseInstance);
                    }


                    var globalCommand = new GlobalCommand
                                        {
                                            BaseType = typeof(TV),
                                            Name = method.Name,
                                            Command = command,
                                            SourceFullName = instance?.GetType().FullName,
                                            SourceType = instance?.GetType(),
                                            SourceMethod = method,
                                            Delegate = action
                                        };

                    foreach (var attribute in method.GetCustomAttributes())
                    {
                        // globalCommand.Name = (string)attribute.GetType().GetProperty("Name")?.GetValue(attribute);
                        globalCommand.ShortcutKey = (InputGesture)attribute.GetType().GetProperty("Hotkey")?.GetValue(attribute);
                    }

                    this.pluginCommands.Add(globalCommand);
                }
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
            var commandName = default(string);
            var commandMethod = default(MethodInfo);
            var map = implementation.GetType().GetInterfaceMap(typeof(TInt));
            
            // Todo: Оптимизировать этот участок кода.
            foreach (var mapTargetMethod in map.TargetMethods)
            {
                foreach (var parameterInfo in mapTargetMethod.GetParameters())
                {
                    if (parameters.GetType().BaseType != parameterInfo.ParameterType) continue;
                    commandName = mapTargetMethod.Name;
                    commandMethod = mapTargetMethod;
                }
            }

            var command = new GlobalCommand
            {
                BaseType = typeof(TInt),
                SourceMethod = commandMethod,
                SourceType = implementation?.GetType(),
                SourceFullName = implementation.GetType().FullName,
                CommandParameter = parameters,
                Command = new RelayCommand(
                    (parameter) =>
                        {
                            var map = implementation.GetType().GetInterfaceMap(typeof(TInt));

                            foreach (var mapTargetMethod in map.TargetMethods)
                            {
                                foreach (var parameterInfo in mapTargetMethod.GetParameters())
                                {
                                    var intType = parameterInfo.ParameterType;
                                    var implType = parameters.GetType();

                                    if (implType.BaseType != intType) continue;
                                    commandName = mapTargetMethod.Name;
                                    var method = implementation.GetType().GetMethod(mapTargetMethod.Name);
                                    method?.Invoke(implementation, new[] { parameters });
                                }
                            }
                        }),
                Name = commandName
            };

            this.pluginCommands.Add(command);
        }

        /// <summary>
        /// Получает зарегистрированные команды которые реализуются плагином.
        /// </summary>
        /// <returns>
        /// Команды подключаемых модулей.
        /// </returns>
        public IEnumerable<BaseCommand> GetCommands() => this.globalCommands;

        /// <summary>
        /// Получает зарегистрированные команды которые реализуются плагином.
        /// </summary>
        /// <returns>
        /// Команды подключаемых модулей.
        /// </returns>
        public IEnumerable<ICommandBase> GetPluginCommands() => this.pluginCommands;

        private static MethodInfo GetOverriddenMethodInfo(MethodInfo methodOverride)
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
            var baseType = methodOverride.GetBaseDefinition().DeclaringType;
            return baseType?.GetMethods(Flags).FirstOrDefault(methodVirtual => methodVirtual.Name == methodOverride.Name);
        }
    }
}
