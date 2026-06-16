
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Infrastructure;

[assembly: PluginInfo(
    name: "Advanced Copy",
    developerId: "advance-copy-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Продвинутый копировщик файлов"
)]
namespace IconBrowser
{
    public class Plugin : IPlugin, IDisposable
    {
        private ILogger _logger;

        public string Name => "Advanced Copy";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {

        }

        public void Start(IPluginContext context)
        {
            var loggerCreate = context.Services.Get<LoggerCreator>();
            _logger = loggerCreate.ForPlugin();
            
            _logger.Info($"{Name} is ready!!!");
        }

        public void Stop()
        {
            _logger = null;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
