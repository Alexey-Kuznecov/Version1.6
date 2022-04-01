using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace UnityCommander.Services.Interfaces
{
    public class UCommandExecute<T> : ICommand
    {
        private Delegate command;

        public UCommandExecute(Delegate cmd)
        {
            this.command = cmd;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Type type = typeof(T);
            ConstructorInfo magicConstructor = type.GetConstructor(Type.EmptyTypes);
            object magicClassObject = magicConstructor.Invoke(new object[] { });
            //command.Method.Invoke(magicClassObject, parameter as object[]);
        }

        public Delegate GetCommand()
        {
            return command;
        }
    }
}
