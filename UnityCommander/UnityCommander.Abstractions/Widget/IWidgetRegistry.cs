
namespace UnityCommander.Abstractions.Widget
{
    public interface IWidgetRegistry
    {
        public void Register<TViewModel, TView>(
          string id,
          string name,
          WidgetActionDefinition primaryAction,
          IReadOnlyList<WidgetActionDefinition> actions);
        
        WidgetDefinition Get(string id);

        IReadOnlyList<WidgetDefinition> GetAll();
    }
}
