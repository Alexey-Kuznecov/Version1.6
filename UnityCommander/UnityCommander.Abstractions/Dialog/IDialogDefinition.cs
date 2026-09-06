
using System;

namespace UnityCommander.Abstractions.Dialog
{
    public interface IDialogDefinition
    {
        string Id { get; }

        Type ViewType { get; }

        Type ViewModelType { get; }

        DialogOptions Options { get; }

        string? OwnerId { get; }
    }
}
