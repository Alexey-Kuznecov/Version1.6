
namespace UnityCommander.Abstractions.Plugins
{
    public interface IInitializable<in T>
    {
        void Initialize(T parameter);
    }

    public interface IInitializable
    {
        void Initialize(object parameter);
    }
}
