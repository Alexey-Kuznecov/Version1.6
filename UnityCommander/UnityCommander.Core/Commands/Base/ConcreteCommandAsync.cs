using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnityCommander.Core.Commands.Base
{
    public class ConcreteCommandAsync
    {
        private readonly ICommandExecutor _executor;

        public ConcreteCommandAsync(ICommandExecutor executor)
        {
            _executor = executor;
        }

        public async void Execute()
        {
            try
            {
                await _executor.ExecuteAsync();
            }
            catch (Exception ex)
            {
                // лог или обработка ошибки
            }
        }
    }
}
