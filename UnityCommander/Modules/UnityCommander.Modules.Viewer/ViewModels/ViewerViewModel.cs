
namespace UnityCommander.Modules.Viewer.ViewModels
{
    using System;
    using Prism.Mvvm;

    using UnityCommander.Common.Module;

    public class ViewerViewModel : BindableBase, ITabPanelContent, IViewerPanel
    {
        public Guid Token { get; set; }

        public string CurrentPath { get; set; }
        public object viewerContent;

        public event Action<string> PathChanged;
        public event Action<string> TabTitleChanged;

        public object ViewerContent
        {
            get => this.viewerContent;
            set => this.SetProperty(ref this.viewerContent, value);
        }

        public bool IsActive => throw new NotImplementedException();

        public void SetViewerContent(object content)
        {
            if (content != null)
            {
                var pluginSettingsView = new Views.PluginSettingsView();
                var context = pluginSettingsView.DataContext as IPluginSettingsViewer;
                context.SetPluginSettingsContent(content);
                this.ViewerContent = pluginSettingsView;
            }
        }

        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            this.CurrentPath = path;
            this.Token = token;
            return this;
        }

        public string GetCurrentPath()
        {
            return this.CurrentPath;
        }

        public Guid GetPanelToken()
        {
            return this.Token;
        }

        public string GetCurrentFilePath()
        {
            return this.CurrentPath;
        }

        public void SetCurrentPath(string value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OnViewAttached(object view)
        {
            throw new NotImplementedException();
        }

        public void OnViewDetached()
        {
            throw new NotImplementedException();
        }
    }
}
