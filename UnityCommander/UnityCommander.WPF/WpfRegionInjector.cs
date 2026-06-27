
using System.Reflection;
using System.Windows.Controls;
using UnityCommander.Core.Plugin;

namespace UnityCommander.WPF
{
    public sealed class WpfRegionInjector : IRegionInjector
    {
        public void Inject(object window, string region, object view)
        {
            if (string.IsNullOrWhiteSpace(region))
                return;

            var field = window.GetType().GetField(
                region,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            if (field == null)
                throw new InvalidOperationException(
                    $"Region '{region}' not found in window '{window.GetType().Name}'.");

            if (field.GetValue(window) is ContentControl contentControl)
            {
                contentControl.Content = view;
                return;
            }

            throw new InvalidOperationException(
                $"Region '{region}' is not a ContentControl.");
        }
    }
}
