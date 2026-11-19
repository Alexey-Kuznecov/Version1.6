
using Prism.Ioc;
using System.Collections.Generic;
using UnityCommander.Preview;
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Providers;
using UnityCommander.Preview.Providers.Images;
using UnityCommander.Preview.Providers.Video;
using UnityCommander.Preview.Wpf;
using UnityCommander.Preview.Wpf.Presenters;

namespace UnityCommander.Dependencies
{
    public static class PreviewRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Preview infrastructure
            registry.RegisterSingleton<IPreviewRegistry, PreviewRegistry>();
            registry.RegisterSingleton<IPreviewService, PreviewService>();

            // Built-in providers
            registry.RegisterSingleton<IPreviewProvider, ImagePreviewProvider>();
            registry.RegisterSingleton<IPreviewProvider, VideoPreviewProvider>();
            registry.RegisterSingleton<IPreviewProvider, FolderPreviewProvider>();

            // Built-in presenters
            registry.RegisterSingleton<IPreviewPresenter, ImagePreviewPresenter>();
            registry.RegisterSingleton<IPreviewPresenter, VideoPreviewPresenter>();
            registry.RegisterSingleton<IPreviewPresenter, MediaElementVideoPreviewPresenter>();
            registry.RegisterSingleton<IPreviewPresenter, FolderPreviewPresenter>();

            registry.RegisterSingleton<VideoEngine>();
        }

        public static void Initialize(IPreviewRegistry previewRegistry, IContainerProvider container)
        {
            const string ownerId = "core.preview";

            foreach (var provider in container.Resolve<IEnumerable<IPreviewProvider>>())
                previewRegistry.RegisterProvider(ownerId, provider);

            foreach (var presenter in container.Resolve<IEnumerable<IPreviewPresenter>>())
                previewRegistry.RegisterPresenter(ownerId, presenter);
        }
    }
}
