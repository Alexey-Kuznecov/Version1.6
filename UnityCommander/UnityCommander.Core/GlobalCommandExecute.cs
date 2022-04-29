
namespace UnityCommander.Core
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    public class GlobalCommandExecute : ICommand
    {
        public Delegate Command { get; }
        public Type DeclaringType { get; }
        public object ViewModelInstance { get; }

        public GlobalCommandExecute(Delegate cmd, Type declaringType)
        {
            this.Command = cmd;
            this.DeclaringType = declaringType;
        }

        public GlobalCommandExecute(Delegate cmd, object viewModelInsatance)
        {
            this.Command = cmd;
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
            object[] parameters = this.GetDefaultParameterValues(parameter);

            if (type == null)
            {
                this.Command.Method.Invoke(this.ViewModelInstance, parameters);
                return;
            }

            ConstructorInfo contructor = type.GetConstructor(Type.EmptyTypes);
            object instance = contructor?.Invoke(new object[] { });

            if (parameter != null)
            {
                this.Command.Method.Invoke(instance, parameter as object[]);
                return;
            }

            this.Command.Method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Используется как временная затычка, чтобы не было ошибки. 
        /// Нужно подумать как можно передавать реальные данные в методы
        /// которые не используют <see cref="MultiCommandParameter"/>. 
        /// </summary>
        /// <returns></returns>
        public object[] GetDefaultParameterValues(object parameter)
        {
            ParameterInfo[] parameterInfos = this.Command.Method.GetParameters();
            object[] parameters = new object[parameterInfos.Length];
            parameters[0] = parameterInfos[0].ParameterType.TypeInitializer;
            parameters[0] = parameter;
            return parameters;
        }
    }
}
