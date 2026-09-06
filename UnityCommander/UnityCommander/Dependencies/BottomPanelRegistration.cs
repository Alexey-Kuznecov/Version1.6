
using Prism.Ioc;
using UnityCommander.Modules.BottomPanel.Services;
using UnityCommander.Services;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Dependencies
{
    public static class BottomPanelRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<IToolRegistry, ToolRegistry>();
            registry.RegisterSingleton<IToolDescriptor, ConsoleToolDescriptor>();
            registry.RegisterSingleton<IToolDescriptor, LoggerToolDescriptor>();
            //registry.RegisterSingleton<IToolDockingService, ToolDockingService>();
            registry.RegisterSingleton<IToolDockingStore, ToolDockingStore>();
            registry.RegisterSingleton<IToolDockingManager, ToolDockingManager>();
            registry.RegisterSingleton<DockingContext>();
        }
    }
}
