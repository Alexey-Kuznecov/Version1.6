namespace UnityCommander.Common.Commands
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    /// <summary>
    /// The global command execute.
    /// </summary>
    [Obsolete]
    public class GlobalCommandExecute : ICommand
    {
        /// <summary>
        /// The action.
        /// </summary>
        private readonly Action<object> action;
        
        private static bool raiseEventGlobalCommandExecuteChanged;

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
        /// The can execute changed.
        /// </summary>
        public static event EventHandler GlobalCommandExecuteChanged;

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

        public static void OnGlobalCommandExecuteChanged(object sender, bool raiseEvent = true)
        {
            raiseEventGlobalCommandExecuteChanged = raiseEvent;
            GlobalCommandExecuteChanged?.Invoke(sender, new EventArgs());
        }

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
            if (this.action != null || parameter == null)
            {
                this.Command.Method.Invoke(this.Command.Target ?? this.Command.Method.DeclaringType, null);
                return; 
            }

            if (parameter is object[] arr)
                this.Command.Method.Invoke(this.Command.Target ?? this.Command.Method.DeclaringType, arr);
            else
            {
                var args = this.GetDefaultParameterValues(parameter);
                this.Command.Method.Invoke(this.Command.Target ?? this.Command.Method.DeclaringType, args);
            }

            if (raiseEventGlobalCommandExecuteChanged)
            {
                OnGlobalCommandExecuteChanged(this);
            }
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
