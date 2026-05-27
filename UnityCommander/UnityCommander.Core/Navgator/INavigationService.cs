using System;

namespace UnityCommander.Core.Navgator
{
    public interface INavigationService
    {
        /// <summary>Текущий путь. null = "Мой компьютер" (root).</summary>
        string Current { get; }

        /// <summary>Проверяет путь — валиден ли (например, диск/папка существует). Для виртуальных провайдеров можно адаптировать.</summary>
        bool IsValidPath(string path);

        /// <summary>Пытается перейти на указанный путь. Возвращает true если успешно.</summary>
        bool TryNavigateTo(string path);

        bool TryNavigateTo(string path, bool forceRecord);

        /// <summary>Событие при изменении Current.</summary>
        event Action<string> CurrentChanged;
    }
}
