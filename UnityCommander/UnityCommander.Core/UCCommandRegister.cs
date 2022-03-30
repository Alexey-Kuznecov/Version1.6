using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Common.Models;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Core
{
    public class UCCommandRegister
    {
        private List<UCCommand> commands;

        public void RegisterCommand()
        {
            Type type = Type.GetType("UnityCommander.Core.IO.Operations.FileManager");
            ConstructorInfo magicConstructor = type.GetConstructor(Type.EmptyTypes);
            object magicClassObject = magicConstructor.Invoke(new object[] { });
            MemberInfo[] methods = type.GetMethods();

            for (int i = 0; i < methods.Length; i++)
            {
                var att = Attribute.GetCustomAttribute(methods[i], typeof(UCCommandAttribute));
                var m = methods[i] as MethodInfo;
                m.Invoke(magicClassObject, new object[] { "C:\\", "D:\\" });
            }

            //InputGesture inputGesture = new KeyGesture(Key.D, ModifierKeys.Control);
            //this.commands.Add(
            //    new UCCommand("DelSelFile",
            //        new DelegateCommand<string>(FileManager.Delete), inputGesture));
        }
    }
}
