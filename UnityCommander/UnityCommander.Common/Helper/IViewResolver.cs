
using System;

namespace UnityCommander.Common.Helper
{
    public interface IViewResolver
    {
        public object Resolve(Type type);

        public T Resolve<T>();
    }
}