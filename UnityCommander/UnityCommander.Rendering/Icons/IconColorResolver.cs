
using System.Windows.Media;
using UnityCommander.Abstractions;
using UnityCommander.Theme;
using UnityCommander.WPF;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconColorResolver : IIconColorResolver
    {
        public Brush Resolve(
            IconKind kind,
            IconRole role,
            IconTone tone,
            VisualState state)
        {
            var palette = ThemeManager.CurrentTheme!.Palette;
            var icons = palette.Icons;

            // ----------------------------------------------------
            // 1. Базовая кисть (ЧТО изображено)
            // ----------------------------------------------------

            Brush brush = kind switch
            {
                IconKind.Folder => ResourceManager.Get<Brush>(icons?.Folder!),
                IconKind.File => ResourceManager.Get<Brush>(icons?.File!),
                IconKind.Drive => ResourceManager.Get<Brush>(icons?.Drive!),
                IconKind.Archive => ResourceManager.Get<Brush>(icons?.Archive!),
                IconKind.Image => ResourceManager.Get<Brush>(icons?.Image!),

                _ => ResourceManager.Get<Brush>(icons?.Default!)
            };

            // ----------------------------------------------------
            // 2. Контекст использования (ГДЕ используется)
            // ----------------------------------------------------

            switch (role)
            {
                case IconRole.RibbonAction:
                    brush = ResourceManager.Get<Brush>(icons?.Accent!);
                    break;

                case IconRole.SidebarItem:
                    brush = ResourceManager.Get<Brush>(palette?.Foreground!);
                    break;

                case IconRole.Plugin:
                    brush = ResourceManager.Get<Brush>(icons?.Plugin!);
                    break;

                    // Generic и FilePanel ничего не переопределяют
            }

            // ----------------------------------------------------
            // 3. Тон (режим отображения)
            // ----------------------------------------------------

            switch (tone)
            {
                case IconTone.Accent:
                    brush = ResourceManager.Get<Brush>(icons?.Accent!);
                    break;

                case IconTone.Muted:
                    brush = ResourceManager.Get<Brush>(icons?.Muted!);
                    break;

                case IconTone.Static:
                    // Просто запрещает эффекты Hover/Selected
                    return brush;
            }

            // ----------------------------------------------------
            // 4. Состояние (самый высокий приоритет)
            // ----------------------------------------------------

            switch (state)
            {
                case VisualState.Disabled:
                    return ResourceManager.Get<Brush>(icons?.Disabled!);

                case VisualState.Hovered:
                    return ResourceManager.Get<Brush>(icons?.Hover!);

                case VisualState.Selected:
                    return ResourceManager.Get<Brush>(icons?.Selected!);

                case VisualState.Pressed:
                    return ResourceManager.Get<Brush>(icons?.Pressed!);

                default:
                    return brush;
            }
        }
    }
}
