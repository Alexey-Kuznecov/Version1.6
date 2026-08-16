
namespace UnityCommander.Core.Registrar
{
    public interface IViewRegistry
    {
        void Register<TViewModel, TView>();

        Type? GetView(Type viewModelType);
    }
}
