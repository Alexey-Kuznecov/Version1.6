
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.IO;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Core.IO
{
    public interface ICopyOperationService
    {
        IReadOnlyCollection<CopyManager> Managers { get; }

        CopyManager Get(Guid operationId);

        bool TryGet(Guid operationId, out CopyManager manager);

        void Register(CopyManager manager, IOperationProgressService progressService);

        bool Unregister(Guid operationId);
    }
}
