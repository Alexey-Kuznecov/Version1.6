
namespace UnityCommander.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using UnityCommander.Common;
    using UnityCommander.Common.Models;
    using UnityCommander.Controls.Ribbon;

    /// <summary>
    /// The command register.
    /// </summary>
    public class CommandRegister
    {
        /// <summary>
        /// The commands.
        /// </summary>
        private List<GlobalCommand> commands;

        /// <summary>
        /// The register command.
        /// </summary>
        public void RegisterCommand()
        {
            Type type = Type.GetType("UnityCommander.Core.IO.Operations.FileManager");
            ConstructorInfo magicConstructor = type?.GetConstructor(Type.EmptyTypes);
            object magicClassObject = magicConstructor?.Invoke(new object[] { });
            MethodInfo[] methods = type?.GetMethods();

            for (int i = 0; i < methods?.Length; i++)
            {
                var att = Attribute.GetCustomAttribute(methods[i], typeof(GlobalCommandAttribute));
                var m = methods[i] as MethodInfo;
                m.Invoke(magicClassObject, new object[] { "C:\\", "D:\\" });
            }

            //InputGesture inputGesture = new KeyGesture(Key.D, ModifierKeys.Control);
            //this.commands.Add(
            //    new GlobalCommand("DelSelFile",
            //        new DelegateCommand<string>(FileManager.Delete), inputGesture));
        }
    }
}
