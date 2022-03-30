using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace UnityCommander.Core
{
    public class GeneratorDelegateType
    {
        private readonly ModuleBuilder m_module;

        public GeneratorDelegateType()
        {
            AssemblyName assemblyName = new AssemblyName("DelegateTypeFactory");

            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);

            // Generate a persistable single-module assembly.
            m_module = asmBuilder.DefineDynamicModule("DelegateTypeFactory");
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
