
using UnityCommander.Abstractions.Sidebar;
using UnityCommander.Common.Helper;

namespace UnityCommander.Common.Sidebar
{
    public class SidebarSectionFactory : ISidebarSectionFactory
    {
        private readonly IViewResolver _viewResolver;

        public SidebarSectionFactory(IViewResolver viewResolver)
        {
            _viewResolver = viewResolver;
        }

        public ISidebarSection Create(ISidebarDefinition def)
        {
            var name = def.ViewKey;

            var view = _viewResolver.Resolve(name.GetType());

            return new SidebarSection(def, view.GetType());
        }
    }
}
