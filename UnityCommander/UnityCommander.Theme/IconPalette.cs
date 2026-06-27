
using System.Windows.Media;

namespace UnityCommander.Theme
{
    public sealed class IconPalette
    {
        // Обычная иконка
        public string? Default { get; init; }

        // Менее важная
        public string? Muted { get; init; }

        // Недоступная
        public string? Disabled { get; init; }

        // Акцентная (Ribbon, выделенные действия)
        public string? Accent { get; init; }

        // При наведении
        public string? Hover { get; init; }

        // Выбранный элемент
        public string? Selected { get; init; }

        // Ошибка
        public string? Error { get; init; }

        // Предупреждение
        public string? Warning { get; init; }

        // Успешное действие
        public string? Success { get; init; }
    }
}
