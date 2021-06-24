
namespace UnityCommander.Core.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Xml.Serialization;

    /// <summary>
    /// The types builder.
    /// </summary>
   /// [DebuggerStepThrough]
    public class GeneratorType
    {
        /// <summary>
        /// The mod builder.
        /// </summary>
        private static ModuleBuilder modBuilder;

        /// <summary>
        /// The generate assembly and module.
        /// </summary>
        /// <param name="name">
        /// The name assembly.
        /// </param>
        public static void GenerateAssemblyAndModule(string name)
        {
            AssemblyName assemblyName = new AssemblyName { Name = name };
            AppDomain thisDomain = Thread.GetDomain();
#if NETCOREAPP

            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            modBuilder = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name);
#elif NET472
            var asmBuilder = thisDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            modBuilder = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name, false);
#endif
        }

        public static void CanAddAttribute()
        {
            var type = typeof(SerializableAttribute);

            var attrCtorParams = new Type[] { };
            var attrCtorInfo = typeof(SerializableAttribute).GetConstructor(attrCtorParams);
            var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { });
        }

        /// <summary>
        /// The create type.
        /// </summary>
        /// <param name="typeName"> The type name. </param>
        /// <returns> The <see cref="TypeBuilder"/>. </returns>
        public static TypeBuilder CreateType(string typeName)
        {
            // ReSharper disable once ComplexConditionExpression
            TypeBuilder typeBuilder = modBuilder.DefineType(
                typeName,
                  TypeAttributes.Public
                    | TypeAttributes.Class
                    | TypeAttributes.AutoClass
                    | TypeAttributes.AnsiClass
                    | TypeAttributes.BeforeFieldInit
                    | TypeAttributes.AutoLayout
                    | TypeAttributes.Serializable,
                typeof(object));

            return typeBuilder;
        }

        /// <summary>
        /// The create <c>type</c>.
        /// </summary>
        /// <param name="typeBuilder"> The mod builder. </param>
        /// <param name="name"> The <c>name</c> property. </param>
        /// <param name="type"> The <c>type</c> property. </param>
        public static void CreateProperty(TypeBuilder typeBuilder, string name, Type type)
        {
            // Define the object field.
            string field = "_" + name.ToLower();
            FieldBuilder defineField = typeBuilder.DefineField(field, type, FieldAttributes.Private);

            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameter less property,
            // use an array with no elements: new Type[] {})
            PropertyBuilder defineProperty = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

            // The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // Define the "get" accessor method for object
            MethodBuilder getPropBldr = typeBuilder.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);
            ILGenerator getIl = getPropBldr.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, defineField);
            getIl.Emit(OpCodes.Ret);

            // Define the "set" mutator method for object.
            MethodBuilder setPropBldr = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });
            ILGenerator setIl = setPropBldr.GetILGenerator();
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, defineField);
            setIl.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            defineProperty.SetGetMethod(getPropBldr);
            defineProperty.SetSetMethod(setPropBldr);
        }
    }
}
