using AlexeyKuznecov.Library.Mvvm.Base;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Common.Models;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class GlobalCommandService : IGlobalCommandService
    {
        Assembly assembly;

        DelegateTypeFactory delegateTypeFactory;

        List<UCCommand> globalCommands;

        public GlobalCommandService()
        {
            assembly = Assembly.Load("UnityCommander.Core");
            delegateTypeFactory = new DelegateTypeFactory(assembly);
            globalCommands = new List<UCCommand>();
        }

        public void SetCommand(GlobalCommand global)
        {
            using var xParam = new XParam(global.ControlItem);

            if (global.ControlItem is MenuItem menuItem)
            {
                menuItem.Command = global.Command;
                var header = menuItem.Header;
                    xParam.AddParam((string)header, menuItem, global.XParamModel);
                xParam.ParamFinal(menuItem);
            }
        }

        public void SetCommand<T>(string commandName, GlobalCommand global)
        {
            var command = this.globalCommands.Single(c => c.Name == commandName);
            var uCommandExecute = (UCommandExecute<T>)command.Command;
            var paramInfo = uCommandExecute.GetCommand().Method.GetParameters();
            using var xParam = new XParam(global.ControlItem);

            if (global.ControlItem is MenuItem menuItem)
            {
                menuItem.Command = command.Command;
                var header = menuItem.Header;
                for (int i = 0; i < paramInfo.Length; i++)
                    xParam.AddParam((string)header, menuItem, global.XParamModelList[i]);
                xParam.ParamFinal(menuItem);
            }       
        }

        public void SetCommand<T>()
        {
            if (this.globalCommands.Count != 0) return;

            var instance = assembly.CreateInstance("UnityCommander.Core.IO.Operations.FileManager");
            
            if (instance is T)
            {
                Type type = instance.GetType();
                MemberInfo[] methods = type.GetMethods();

                for (int i = 0; i < methods.Length; i++)
                {
                    var att = Attribute.GetCustomAttribute(methods[i], typeof(UCCommandAttribute));

                    if (att != null)
                    {
                        var m = methods[i] as MethodInfo;
                        var action = Delegate.CreateDelegate(delegateTypeFactory.CreateDelegateType(m), instance, m);
                        var cmd = new UCCommand(((UCCommandAttribute)att).Name, new UCommandExecute<T>(action), new KeyGesture(Key.D, ModifierKeys.Control));
                        var input = new InputBinding(cmd.Command, cmd.ShortcutKey);
                        var inputBindingCollection = new InputBindingCollection();
                        inputBindingCollection.Add(input);
                        this.globalCommands.Add(cmd);
                    }
                }
            }
        }

        public UCCommand GetCommand<T>(string commandName)
        {
            var cmd = this.globalCommands.Single(c => c.Name == commandName);
            return cmd;
        }
    }

    public class DelegateTypeFactory
    {
        private readonly ModuleBuilder m_module;

        public DelegateTypeFactory(Assembly assembly)
        {
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DelegateTypeFactory"), AssemblyBuilderAccess.Run);
            m_module = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name);
        }

        public Type CreateDelegateType(MethodInfo method)
        {
            string nameBase = string.Format("{0}{1}", method.DeclaringType.Name, method.Name);
            string name = GetUniqueName(nameBase);

            var typeBuilder = m_module.DefineType(
                name, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            var parameters = method.GetParameters();

            var invokeMethod = typeBuilder.DefineMethod(
                "Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
            invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
            }

            return typeBuilder.CreateType();
        }

        private string GetUniqueName(string nameBase)
        {
            int number = 2;
            string name = nameBase;
            while (m_module.GetType(name) != null)
                name = nameBase + number++;
            return name;
        }
    }
}
