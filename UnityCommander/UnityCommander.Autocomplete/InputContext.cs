
namespace UnityCommander.CLI.Input
{
    public enum InputContext3
    {
        CommandName,        // ввод имени команды
        ArgumentName,       // ввод имени аргумента
        ArgumentValue,      // ввод значения аргумента
        Completed,          // команда полностью собрана
        Invalid             // ошибка (подсветка)
    }
}
