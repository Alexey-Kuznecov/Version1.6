
using CommandSystem.Abstractions;
using CommandSystem.Gui.Integraion;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Helper;

namespace UnityCommander.Core.Commands
{
    public static class CommandFactory
    {
        public static CommandDefinition Create(
            string id,
            Func<CommandContext, Task> execute,
            Func<CommandContext, Task<UndoToken>> undo = null,
            params Type[] contextTypes)
        {
            var presentation = CommandPresentationProvider.Get(id);

            return new CommandDefinition
            {
                Metadata = new CommandMetadata(
                    id,
                    presentation.Description)
                {
                    Category = CommandCategoryHelper.GetCategory(id),
                    ContextTypes = contextTypes.ToList()
                },

                Execute = execute,
                UndoExecute = undo
            };
        }
    }
}
