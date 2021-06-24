
using System.Diagnostics.CodeAnalysis;

namespace UnityCommander.Core.Helper
{
    using System;
    using System.Reflection.Emit;

    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class ObjectBuilder<T> : IDisposable where T : new()
    {
        public static TypeBuilder TypeBuilder { get; set; }

        public T Instance { get; set; }

        private T mergeObjectL = default;
        private object mergeObjectR = default;
        private WeakReference weakReference = default;

        public T ObjectInit(T objectL, object objectR)
        {
            Instance = (T)Activator.CreateInstance(TypeBuilder);
            weakReference = new WeakReference(Instance);

            var propertyInfos = Instance?.GetType().GetProperties();

            foreach (var objectLProperty in objectL.GetType().GetProperties())
            {
                if (propertyInfos != null)
                    
                    foreach (var singleProperty in propertyInfos)
                    {
                        if (objectLProperty.Name == singleProperty.Name)
                        {
                            singleProperty.SetValue(this.Instance, objectLProperty.GetValue(objectL));
                        }
                    }
            }

            foreach (var objectRProperty in objectR.GetType().GetProperties())
            {
                if (propertyInfos != null)

                    foreach (var singleProperty in propertyInfos)
                    {
                        if (objectRProperty.Name == singleProperty.Name)
                        {
                            singleProperty.SetValue(this.Instance, objectRProperty.GetValue(objectR));
                        }
                    }
            }

            return this.Instance;
        }

        public void MergeObjectProperties(Type objectR)
        {
            this.mergeObjectL = new T();
            this.mergeObjectR = Activator.CreateInstance(objectR);

            if (TypeBuilder is null)
            {
                TypeBuilder = this.mergeObjectL.MergeObjectProperties(this.mergeObjectR);
                TypeBuilder.SetParent(typeof(T));
                TypeBuilder.CreateType();
            }
        }

        public void Dispose()
        {
            //if (weakReference is null) return;

            //for (int i = 0; weakReference.IsAlive && (i < 10); i++)
            //{
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //}
        }
    }
}
