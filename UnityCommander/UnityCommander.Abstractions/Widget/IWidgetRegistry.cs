
namespace UnityCommander.Abstractions.Widget
{
    public interface IWidgetRegistry
    {
        void Register<TViewModel, TView>(
            string id,
            string name);

        WidgetDefinition Get(string id);

        IReadOnlyList<WidgetDefinition> GetAll();
    }
}
