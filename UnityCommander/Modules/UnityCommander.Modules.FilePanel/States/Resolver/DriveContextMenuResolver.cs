using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class DriveContextMenuResolver
       : IContextMenuResolver
    {
        public bool CanResolve(object context)
        {
            return context is DriveNodeContext;
        }

        public SurfaceContext Resolve(
            object context,
            object parameter)
        {
            var drive = (DriveNodeContext)context;

            var result = new SurfaceContext();

            result.Set(new DrivePanelContexMenu
            {
                Selected = drive.Selected
            });

            return result;
        }
    }
}
