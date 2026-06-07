
using System.Windows.Controls;

namespace UnityCommander.Common.Sidebar
{
    public class SidebarSection : ISidebarSection
    {
        public string Id { get; }

        public ISidebarDefinition Definition { get; }

        public UserControl View { get; }

        public object ViewModel { get; }

        public string IconKey { get; }

        private bool _isActive;

        public SidebarSection(
           ISidebarDefinition def,
           UserControl view)
        {
            Id = def.Id;
            Definition = def;
            View = view;

            ViewModel = view.DataContext;
        }

        public SidebarSection(
            string id, 
            string iconKey,
            UserControl view, 
            object content)
        {
            Id = id;
            View = view;
            IconKey = iconKey;
            ViewModel = content;
        }

        public void Activate()
        {
            if (_isActive)
                return;

            _isActive = true;

            if (ViewModel is ISidebarActivatable activatable)
            {
                activatable.OnActivated();
            }
        }

        public void Capture(ISidebarSectionState state)
        {
            var s = (SidebarSectionState)state;

            s.SectionId = Id;
            s.IsActive = _isActive;

            if (ViewModel is ISidebarStateProvider provider)
            {
                s.Payload = provider.CaptureState();
            }
        }

        public void Deactivate()
        {
            if (!_isActive)
                return;

            _isActive = false;

            if (ViewModel is ISidebarActivatable activatable)
            {
                activatable.OnDeactivated();
            }
        }

        public void Restore(ISidebarSectionState state)
        {
            var s = (SidebarSectionState)state;

            if (s.IsActive)
            {
                Activate();
            }

            if (ViewModel is ISidebarStateProvider provider && s.Payload != null)
            {
                provider.RestoreState(s.Payload);
            }
        }
    }
}
