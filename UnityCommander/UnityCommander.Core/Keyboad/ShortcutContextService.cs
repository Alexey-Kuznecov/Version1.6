
using System.Collections.Generic;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Core.Keyboad
{
    public sealed class ShortcutContextService : IShortcutContextService
    {
        private readonly List<ScopeEntry> _stack = [];

        public ShortcutScope Current =>
            _stack.Count > 0
                ? _stack[^1].Scope
                : ShortcutScope.Global;

        public void Push(object owner, ShortcutScope scope)
        {
            _stack.RemoveAll(x => ReferenceEquals(x.Owner, owner));

            _stack.Add(new ScopeEntry
            {
                Owner = owner,
                Scope = scope
            });
        }

        public void Pop(object owner)
        {
            _stack.RemoveAll(x => ReferenceEquals(x.Owner, owner));
        }
    }
}
