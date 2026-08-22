
using System.Windows;

namespace UnityCommander.WPF
{
    public interface IProgressIndicatorService
    {
        void Show(
           UIElement target,
           ProgressIndicatorMode mode);

        void Update(
            UIElement target,
            double progress);

        void Hide(UIElement target);
    }
}
