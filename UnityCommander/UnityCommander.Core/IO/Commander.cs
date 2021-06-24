
namespace UnityCommander.Core.IO
{
    using System;

    /// <summary>
    /// Provides objects for command management.
    /// </summary>
    /// <typeparam name="T">
    /// Specify the type of command to control which the special <c>object</c> will be requested.
    /// </typeparam>
    public static class Commander<T> where T : new()
    {
        /// <summary>
        /// The get manager.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static ManagerBase GetManager()
        {
            Type type = typeof(T);

            var attributes = type.GetCustomAttributes(false);

            foreach (var attribute in attributes)
            {
                if (attribute is ManagerAttachAttribute commandAttribute)
                {
                    var handle = Activator.CreateInstance(commandAttribute.Function.Assembly.FullName, commandAttribute.Function.FullName ?? throw new InvalidOperationException());

                    if (handle != null)
                    {
                        ManagerBase manager = handle.Unwrap() as ManagerBase;
                        return manager;
                    }
                }
            }

            throw new ArgumentException();
        }
    }
}
