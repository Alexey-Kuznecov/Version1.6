
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.Dialog
{
    public sealed class CreationDialogResult : IDialogResult
    {
        public CreationType Type { get; init; }

        public string Name { get; init; }

        public string Extension { get; init; }

        public string? TemplateId { get; init; }
    }
}
