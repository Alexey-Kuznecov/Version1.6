
using System;
using System.Windows.Controls;

namespace UnityCommander.Services.Interfaces
{
    public interface IToolCatalogViewFactory
    {
        Control Create(
            Action<IToolDescriptor> onSelected,
            Action onCanceled);
    }
}
