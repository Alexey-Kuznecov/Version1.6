using System.Collections.Generic;
using System.Diagnostics;

namespace UnityCommander.Core.Helper
{
    [DebuggerStepThrough]
    public struct Singleton  
    {
        public static object Back = null;
        public static int Count = 0;
        public static bool Status = false;
        /// <summary>
        /// This field stores all instances, it is not reasonable to get links from there,
        /// since you can get any instance through the SingleInstance method, it is enough
        /// to specify the type of the desired class as a parameter of the generic method,
        /// Therefore, I leave these fields closed and read-only, for security reasons, of course.
        /// </summary>
        private static readonly List<object> Instance = new List<object>();
        /// <summary>
        /// The method creates one single instance of a simple class that cannot be pre-defined in other classes.
        /// The only instance will be the first instance that was created in the class or structure. However,
        /// there is no difference in which class the instance was declared. This is especially convenient
        /// to communicate between different ViewModel windows, share resources and objects. To create a single
        /// instance of a class, write only one line of code<example> YourType instance = Singleton.SingleInstance{YourType}(); </example>
        /// </summary>
        /// <typeparam name="T"> Any reference object. </typeparam>
        /// <returns> The only copy. </returns>
        public static T SingleInstance<T>() where T : new ()
        {
            T singleInstance = new T();
            byte multi = 0, single = 0;
            // If method was call one time then needed instantiated
            if (Instance.Count == 0)  
                Instance.Add(new T());
            // Collection traversals and if type already exist returns it.
            for (var i = 0; i < Instance.Count; i++)
                if (Instance[i] is T)
                {
                    single++;
                    singleInstance = (T)Instance[i];
                }
                else multi++;
            // If iterator didn't found same type then adds new type in collection. 
            if (multi <= (multi - single))
                Instance.Add(new T());
            return singleInstance;
        }
        /// <summary>
        /// Object returns that was queried.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <returns>Reference into object.</returns>
        public static T GetSingleInstance<T>() where T : new ()
        {
            foreach (var obj in Instance)
                if (obj is T)
                    return (T)obj;
            return new T();
        }
    }
}