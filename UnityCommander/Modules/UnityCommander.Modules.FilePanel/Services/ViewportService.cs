
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Diagnostics.Tracing;
using Prism.Ioc;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class ViewportService<T>
    {
        private readonly Func<IReadOnlyList<T>> _source;

        private int _start;
        private int _end;

#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        private IDiagnosticTrace? _trace = ContainerLocator.Container.Resolve<IDiagnosticTrace>();
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".


        public ViewportService(Func<IReadOnlyList<T>> source)
        {
            _source = source;
        }

        public void SetRange(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public IEnumerable<T> GetVisibleItems()
        {
            var items = _source();

            //_trace.Write(
            //  "ViewportService",
            //  "GetVisibleItems",
            //  new Dictionary<string, object?>
            //  {
            //      ["Start"] = _start,
            //      ["End"] = _end,
            //      ["Count"] = items.Count
            //  });

            for (int i = _start; i < _end && i < items.Count; i++)
            {
                //_trace.Write(
                //   "ViewportService",
                //   "VisibleItem",
                //   new Dictionary<string, object?>
                //   {
                //       ["Index"] = i,
                //       ["Type"] = items[i]?.GetType().Name,
                //       ["Path"] = (items[i] as IDirectoryItem)?.Path,
                //       ["Name"] = (items[i] as IDirectoryItem)?.Name
                //   });

                yield return items[i];
            }
        }
    }
}
