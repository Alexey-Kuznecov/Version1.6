using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace UnityCommander.Integration.Commands
{
    public class GlobalCommandExecute : ICommand
    {
        private readonly Delegate command;
        private readonly Type commandSource;

        public GlobalCommandExecute(Delegate cmd, Type source)
        {
            this.command = cmd;
            this.commandSource = source;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Type type = commandSource;
            ConstructorInfo magicConstructor = type.GetConstructor(Type.EmptyTypes);
            object magicClassObject = magicConstructor?.Invoke(new object[] { });

            if (parameter != null)
            {
                command.Method.Invoke(magicClassObject, parameter as object[]);
                return;
            }

            ParameterInfo[] parameterInfos = command.Method.GetParameters();
            object[] parameters = new object[parameterInfos.Length]; 
            parameters[0] = parameterInfos[0].ParameterType.TypeInitializer;
            command.Method.Invoke(magicClassObject, parameters);
        }
    }
}
