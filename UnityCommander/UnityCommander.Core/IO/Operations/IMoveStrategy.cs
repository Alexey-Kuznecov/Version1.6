
using System.Threading.Tasks;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Core.IO.Operations
{
    public interface IMoveStrategy
    {
        bool DeletesSource { get; }

        bool CanHandle(string source, string destination);

        Task ExecuteAsync(
             OperationContext context,
             string source,
             string destination);
    }
}
