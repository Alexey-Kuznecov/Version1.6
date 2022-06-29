
namespace UnityCommander.Core
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    /// <summary>
    /// The global command execute.
    /// </summary>
    public class GlobalCommandExecute : ICommand
    {
        /// <summary>
        /// The action.
        /// </summary>
        private readonly Action<object> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandExecute"/> class.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="declaringType">
        /// The declaring type.
        /// </param>
        public GlobalCommandExecute(Delegate cmd, Type declaringType)
        {
            this.Command = cmd;
            this.DeclaringType = declaringType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandExecute"/> class.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="args">
        /// The view model instance.
        /// </param>
        public GlobalCommandExecute(Delegate cmd, object args)
        {
            this.Command = cmd;
            this.Args = args;
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets the command.
        /// </summary>
        public Delegate Command { get; }

        /// <summary>
        /// Gets the declaring type.
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        /// Gets the view model instance.
        /// </summary>
        public object Args { get; }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            if (this.action == null)
            {
                if (parameter != null)
                {
                    var args = GetDefaultParameterValues(this.Args ?? parameter);
                    this.Command.Method.Invoke(this.Command.Target ?? this.Command.Method.DeclaringType, args);
                    return;
                }

                //Type type = this.DeclaringType;
                //object[] parameters = this.GetDefaultParameterValues(parameter);

                //if (type == null && parameter == null)
                //{
                //    this.Command.Method.Invoke(this.Args, parameters);
                //    return;
                //}

                //var constructor = type.GetConstructor(Type.EmptyTypes);
                //var instance = constructor?.Invoke(new object[] { });

                //if (parameter != null)
                //{
                //    this.Command.Method.Invoke(instance, parameter as object[]);
                //    return;
                //}

                //this.Command.Method.Invoke(instance, parameters);
            }

            //if (this.action != null)
            //{
            //    this.action.Method.Invoke(this.action.Target, new object[] { null });
            //}
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
