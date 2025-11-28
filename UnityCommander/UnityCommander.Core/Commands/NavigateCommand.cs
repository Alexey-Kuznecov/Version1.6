using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core.Commands
{
    public sealed class NavigateCommand : IAppCommand
    {
        private readonly INavigationService _nav;
        private readonly string? _newPath;
        private string? _oldPath;
        private bool _executed;

        public NavigateCommand(INavigationService navigation, string? newPath)
        {
            _nav = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _newPath = newPath;
        }

        public void Execute()
        {
            if (!_nav.IsValidPath(_newPath))
                throw new InvalidOperationException($"Invalid path: {_newPath ?? "<root>"}");

            // сохраняем предыдущее состояние единожды
            if (!_executed)
            {
                _oldPath = _nav.Current;
                _executed = true;
            }

            _nav.TryNavigateTo(_newPath);
        }

        public void Undo()
        {
            // Возвращаем предыдущее состояние
            _nav.TryNavigateTo(_oldPath);
        }
    }
}
