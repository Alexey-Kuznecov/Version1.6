
using System;

namespace UnityCommander.Abstractions
{
    public interface IViewResolver
    {
        public object Resolve(Type type);

        public T Resolve<T>();
    }
}