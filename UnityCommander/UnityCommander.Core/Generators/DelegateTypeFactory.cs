using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace UnityCommander.Core.Generators
{
    public class DelegateTypeFactory
    {
        private static ModuleBuilder module;

        public static Type Create(MethodInfo method)
        {
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DelegateTypeFactory"), AssemblyBuilderAccess.Run);
            module = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name ?? string.Empty);
            return CreateDelegateType(method);
        }

        public static Type CreateDelegateType(MethodInfo method)
        {
            string nameBase = $"{method.DeclaringType?.Name}{method.Name}";
            string name = GetUniqueName(nameBase);

            var typeBuilder = module.DefineType(
                name, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, 
                        new[] { typeof(object), typeof(IntPtr) });
            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            var parameters = method.GetParameters();

            var invokeMethod = typeBuilder.DefineMethod(
                "Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                            method.ReturnType,
                        parameters.Select(p => p.ParameterType).ToArray());
            
            invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
            }

            return typeBuilder.CreateType();
        }

        private static string GetUniqueName(string nameBase)
        {
            int number = 2;
            string name = nameBase;
            while (module.GetType(name) != null)
                name = nameBase + number++;
            return name;
        }
    }
}
