
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
            var view = _viewResolver.Resolve(def.ViewKey);

            return new SidebarSection(def, view);
        }
    }
}
