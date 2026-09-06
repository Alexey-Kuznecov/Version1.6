
namespace UnityCommander.Settings
{
    public interface ISettings
    {
        bool SidebarDisplayContent { get; }

        bool IsSessionSaved { get; }

        string SessionFiles { get; }

        bool RibbonVisibility { get; }
    }
}
