
namespace UnityCommander.Core.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    /// <summary>
    /// The extension method.
    /// </summary>
    public static class ExtensionMethod
    {
        /// <summary>
        /// The stored.
        /// </summary>
        private static List<object> stored;

        /// <summary>
        /// The type builder.
        /// </summary>
        private static TypeBuilder typeBuilder;

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
            stored = new List<object>();
            GeneratorType.GenerateAssemblyAndModule("DirectoryModel");
            typeBuilder = GeneratorType.CreateType("DirectoryModel");

            foreach (var propertyInfo in mergeObjectL.GetType().GetProperties())
            {
                stored.Add(propertyInfo.GetValue(mergeObjectL));
                GeneratorType.CreateProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
            }

            foreach (var propertyInfo in mergeObjectR.GetType().GetProperties())
            {
                stored.Add(propertyInfo.GetValue(mergeObjectL));
                GeneratorType.CreateProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
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
        public static T MergeObjectProperties<T>(this object mergeObjectL, object mergeObjectR, Type[] implInterface = null)
        {
            typeBuilder = mergeObjectL.MergeObjectProperties(mergeObjectR);

            if (implInterface != null)
            {
                foreach (var type in implInterface)
                {
                    typeBuilder.AddInterfaceImplementation(type);
                } 
            }

            typeBuilder.SetParent(typeof(T));
            typeBuilder.CreateType();
            
            return (T)Activator.CreateInstance(RestoreData(typeBuilder));
        }

        /// <summary>
        /// The restore data.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <returns>
        /// The <see cref="TypeBuilder"/>.
        /// </returns>
        private static TypeBuilder RestoreData(TypeBuilder builder)
        {
            foreach (var property in builder.GetProperties())
            {
                property.SetValue(builder, (string)stored.ExtractEach());
            }

            return builder;

        }
    }
}
