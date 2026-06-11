using System;

namespace UnityCommander.Common.Dialog
{
    public interface IDialogDefinition
    {
        string Id { get; }

        Type ViewType { get; }

        Type ViewModelType { get; }

        DialogOptions Options { get; }
    }
}
