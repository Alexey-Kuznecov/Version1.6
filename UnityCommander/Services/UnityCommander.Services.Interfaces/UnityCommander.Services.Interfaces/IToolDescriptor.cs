
using System.Windows.Controls;

namespace UnityCommander.Services.Interfaces
{
    public interface IToolDescriptor
    {
        string Id { get; }

        string Title { get; }

        bool CanCreateMultiple { get; }

        Control Create();
    }
}
