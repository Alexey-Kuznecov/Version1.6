
using UnityCommander.Abstractions.Plugin;

namespace UnityCommander.Core.Plugin
{
    public class CompositionBuilder : ICompositionBuilder
    {
        private readonly CompositionDefinition _def;

        public CompositionBuilder(CompositionDefinition def)
        {
            _def = def;
        }

        public void Add<TView, TViewModel>(string region = null)
        {
            _def.Parts.Add(
                new CompositionPart(
                    typeof(TView),
                    typeof(TViewModel),
                    region));
        }
    }
}
