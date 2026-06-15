
using System;
using UnityCommander.Common.Plugins;

namespace UnityCommander.Common.Dialog
{
    public class DialogDefinition : IDialogDefinition, IPluginOwned
    {
        public string Id { get; }
        public Type ViewType { get; }
        public Type ViewModelType { get; }
        public DialogOptions Options { get; }
        public string PluginId { get; set; }
        public string OwnerId => PluginId;

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
