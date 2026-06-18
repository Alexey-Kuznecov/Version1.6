
using System;

namespace UnityCommander.Abstractions.Plugin
{
    public sealed class CompositionPart
    {
        public Type View { get; }
        public Type ViewModel { get; }
        public string Region { get; }

        public CompositionPart(Type view, Type viewModel, string region)
        {
            View = view;
            ViewModel = viewModel;
            Region = region;
        }
    }
}
