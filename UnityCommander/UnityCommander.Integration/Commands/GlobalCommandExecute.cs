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

            if (parameter == null)
            {
                object[] parameterInfos = command.Method.GetParameters();
                var parameters = new object[] { };

                foreach (var parameterInfo in parameterInfos.ToArray())
                {
                    parameters = parameterInfos;
                }

                //command.Method.Invoke(magicClassObject, parameters);
            }
            else
            {
                //command.Method.Invoke(magicClassObject, parameter as object[]);
            }
        }

        public Delegate GetCommand()
        {
            return command;
        }
    }
}
