
using System;
using System.Collections.Generic;
using UnityCommander.Diagnostics.Tracing;
using Prism.Ioc;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class ViewportMapper
    {
#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        private IDiagnosticTrace? _trace;
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".

        public ViewportMapper()
        {
            _trace = ContainerLocator.Container.Resolve<IDiagnosticTrace>();
        }

        public event Action<int, int>? RangeChanged;

        public void Update(double offset, double viewport)
        {
            int start = (int)offset;
            int end = start + (int)Math.Ceiling(viewport);

            //_trace?.Write(
            //      "ViewportMapper",
            //      "RangeUpdated",
            //      new Dictionary<string, object?>
            //      {
            //          ["Offset"] = offset,
            //          ["Viewport"] = viewport,
            //          ["Start"] = start,
            //          ["End"] = end + 1
            //      });


            RangeChanged?.Invoke(start, end + 1);
        }
    }
}
