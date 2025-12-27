namespace UnityCommander.Logging.Configuration
{
    public enum LoggingMode
    {
        Debug,          // всё подряд
        Information,    // инфа + важное
        UserActions,    // только действия пользователя
        ErrorsOnly,     // только ошибки
        Silent          // почти ничего
    }
}
