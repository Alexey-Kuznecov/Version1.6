using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace UnityCommander.Integration.Commands
{
    public class GlobalCommandExecute : ICommand
    {
        private readonly Delegate command;
        private readonly Type DeclaringType;
        private readonly object ViewModelInstance;

        public GlobalCommandExecute(Delegate cmd, Type declaringType)
        {
            this.command = cmd;
            this.DeclaringType = declaringType;
        }

        public GlobalCommandExecute(Delegate cmd, object viewModelInsatance)
        {
            this.command = cmd;
            this.ViewModelInstance = viewModelInsatance;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Type type = this.DeclaringType;
            object[] parameters = GetDefaultParameterValues();

            if (type == null)
            {
                command.Method.Invoke(this.ViewModelInstance, parameters);
                return;
            }

            ConstructorInfo contructor = type.GetConstructor(Type.EmptyTypes);
            object instance = contructor?.Invoke(new object[] { });

            if (parameter != null)
            {
                command.Method.Invoke(instance, parameter as object[]);
                return;
            }

            command.Method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Используется как временная затычка, чтобы небыло ошибки. 
        /// Нужно подумать как можно передавать реальные данные в методы
        /// которые не используют <see cref="MultiCommandParameter"/>. 
        /// </summary>
        /// <returns></returns>
        public object[] GetDefaultParameterValues()
        {
            ParameterInfo[] parameterInfos = command.Method.GetParameters();
            object[] parameters = new object[parameterInfos.Length];
            parameters[0] = parameterInfos[0].ParameterType.TypeInitializer;

            return parameters;
        }
    }
}
