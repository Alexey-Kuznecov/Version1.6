
using System;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Dialog;

namespace UnityCommander.Common.Dialog
{
    public class DialogDefinition : IDialogDefinition, IOwned
    {
        public string Id { get; }
        public Type ViewType { get; }
        public Type ViewModelType { get; }
        public DialogOptions Options { get; }
        public string OwnerId { get; set; }

        public DialogDefinition(
            string id, 
            Type viewType, 
            Type viewModelType, 
            DialogOptions option = null)
        {
            Id = id;
            ViewType = viewType;
            ViewModelType = viewModelType;
            Options = option;
        }
    }
}
