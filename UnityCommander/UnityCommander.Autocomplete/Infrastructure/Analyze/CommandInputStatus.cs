
namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public enum CommandInputStatus
    {
        None,           // ничего не введено
        Typing,         // вводится (по префиксу)
        Completed,      // введена полностью
        Invalid         // не существует
    }
}
