
using System;
using System.Collections.Generic;

namespace UnityCommander.Core.Navigation
{
    public class CommandHistory
    {
        private readonly List<IAppCommand> _commands = new();
        private int _index = -1; // индекс последней выполненной команды в списке

        public bool CanUndo => _index >= 0;
        public bool CanRedo => _index + 1 < _commands.Count;

        /// <summary>Выполняет команду и добавляет в историю (сбрасывает Redo-ветку).</summary>
        public void Execute(IAppCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            // Удаляем ветку redo, если она есть
            if (_index + 1 < _commands.Count)
                _commands.RemoveRange(_index + 1, _commands.Count - _index - 1);

            command.Execute();
            _commands.Add(command);
            _index++;
        }

        public void Undo()
        {
            if (!CanUndo) return;
            _commands[_index].Undo();
            _index--;
        }

        public void Redo()
        {
            if (!CanRedo) return;
            _index++;
            _commands[_index].Execute();
        }

        public void Clear()
        {
            _commands.Clear();
            _index = -1;
        }
    }
}
