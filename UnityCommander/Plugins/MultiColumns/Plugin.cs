
using MultiColumns.DateTime;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using UnityCommander.Common.Columns;

[assembly: PluginInfo(
    name: "Multi Column Plugin",
    developerId: "multi-column-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Плагин расширяет колонки файлов и папок"
)]
namespace MultiColumns
{
    public class Plugin : IPlugin
    {
        public string Name => "Multi Column Plugin";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            init.RegisterColumn<IColumnProvider, DateTimeColumnProvider>();
            //init.RegisterView<MultiColumn, MultiColumnViewModel>();
        }

        public void Start(IPluginContext context)
        {
            //throw new System.NotImplementedException();
        }

        public void Stop()
        {
            //throw new System.NotImplementedException();
        }
    }
}
