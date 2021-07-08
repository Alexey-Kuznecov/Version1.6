
namespace UnityCommander.Core.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Models;

    /// <summary>
    /// The extension method.
    /// </summary>
    public static class ExtensionMethod
    {
        /// <summary>
        /// The stored.
        /// </summary>
        private static Dictionary<string, object> stored;

        /// <summary>
        /// The type builder.
        /// </summary>
        private static TypeBuilder typeBuilder;

        /// <summary>
        /// The stored reset.
        /// </summary>
        public static void StoredReset()
        {
            stored = null;
        }

        /// <summary>
        /// This method will create a new <see langword="object"/> based on two
        /// objects. Note, that this method copies only the properties of
        /// objects. The method does just half the work to complete it, call the
        /// method on the <see cref="TypeBuilder.CreateType"/>object and
        /// create an instance using <see cref="Activator.CreateInstance"/>.
        /// </summary>
        /// <param name="mergeObjectL"> The object in the left. </param>
        /// <param name="mergeObjectR"> The object in the right. </param>
        /// <returns>
        /// Returns an object of type TypeBuilder. </returns>
        /// [DebuggerStepperBoundary]
        public static TypeBuilder MergeObjectProperties(this object mergeObjectL, object mergeObjectR)
        {
            stored = new Dictionary<string, object>();

            GeneratorType.GenerateAssemblyAndModule(mergeObjectL.GetType().Name);
            typeBuilder = GeneratorType.CreateType(mergeObjectL.GetType().Name);

            foreach (var propertyInfo in mergeObjectR.GetType().GetProperties())
            {
                stored.Add(propertyInfo.Name, propertyInfo.GetValue(mergeObjectR));
                GeneratorType.CreateProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
            }

            foreach (var propertyInfo in mergeObjectL.GetType().GetProperties())
            {
                if (!stored.ContainsKey(propertyInfo.Name))
                {
                    stored.Add(propertyInfo.Name, propertyInfo.GetValue(mergeObjectL));
                    GeneratorType.CreateProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
                }
            }

            return typeBuilder;
        }

        /// <summary>
        /// This method will create a new <see langword="object"/> based on two objects.
        /// Note, that this method copies only the properties of objects.  
        /// </summary>
        /// <param name="mergeObjectL"> The merge object in the left. </param>
        /// <param name="mergeObjectR"> The merge object in the right. </param>
        /// <param name="implInterface"> Adds an interface implemented by the object. </param>
        /// <typeparam name="T"> The type that needs to be made basic for the object. </typeparam>
        /// <returns> Returns an object of type. </returns>
        /// [DebuggerStepperBoundary]
        [Obsolete]
        public static T MergeObjectProperties<T>(this object mergeObjectL, object mergeObjectR, Type[] implInterface = null)
        {
            typeBuilder = MergeObjectProperties(mergeObjectL, mergeObjectR);

            if (implInterface != null)
            {
                foreach (var type in implInterface)
                {
                    typeBuilder.AddInterfaceImplementation(type);
                }
            }

            typeBuilder.SetParent(typeof(T));
            typeBuilder.CreateType();
            
            return RestoreData<T>(typeBuilder);
        }

        /// <summary>
        /// The restore data.
        /// </summary>
        /// <typeparam name="T">
        /// The type that needs to be made basic for the object.
        /// </typeparam>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <returns>
        /// The <see cref="TypeBuilder"/>.
        /// </returns>
        [Obsolete]
        private static T RestoreData<T>(TypeBuilder builder)
        {
            var instance = (T)Activator.CreateInstance(typeBuilder);

            foreach (var property in builder.GetProperties())
            {
                if (stored[property.Name] is string)
                {
                    property.SetValue(instance, (string)stored[property.Name]);
                }

                if (stored[property.Name] is IconModel)
                {
                    property.SetValue(instance, (IconModel)stored[property.Name]);
                }

                if (stored[property.Name] is DateTime)
                {
                    property.SetValue(instance, (DateTime)stored[property.Name]);
                }

                if (stored[property.Name] is TargetPanel)
                {
                    property.SetValue(instance, (TargetPanel)stored[property.Name]);
                }
            }

            return instance;
        }
    }
}
