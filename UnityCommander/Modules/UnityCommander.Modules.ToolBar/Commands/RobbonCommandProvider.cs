
using CommandSystem.Abstractions;
using System.Threading.Tasks;
using System.Windows;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class RobbonCommandProvider
    {
        public Task ShowDialogTest(CommandContext ctx)
        {
            MessageBox.Show("Работает!");

            return Task.CompletedTask;
        }

        public Task ShowParamTest(CommandContext ctx)
        {
            if (ctx.Parameter != null)
                MessageBox.Show(ctx.Parameter.ToString());
            else
                MessageBox.Show("Комманда без параметров");

            return Task.CompletedTask;
        }
    }
}
