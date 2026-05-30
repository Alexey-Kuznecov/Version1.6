using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class DriveContextResolver
       : IContextResolver
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

            result.Set(new DrivePanelContext
            {
                Selected = drive.Selected
            });

            return result;
        }
    }
}
