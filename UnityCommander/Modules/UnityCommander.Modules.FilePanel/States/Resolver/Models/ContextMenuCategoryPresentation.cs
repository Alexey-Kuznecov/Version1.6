
using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver.Models
{
    public sealed class ContextMenuCategoryPresentation
      : ISurfaceCategoryPresentation
    {
        public SurfaceCategoryPresentation Get(string category)
        {
            return category switch
            {
                "file" => SurfaceCategoryPresentation.Separator,
                "folder" => SurfaceCategoryPresentation.Separator,
                "item" => SurfaceCategoryPresentation.Separator,
                "clipboard" => SurfaceCategoryPresentation.Separator,
                "panel" => SurfaceCategoryPresentation.Separator,
                "archive" => SurfaceCategoryPresentation.Submenu,

                _ => SurfaceCategoryPresentation.Submenu
            };
        }
    }
}
