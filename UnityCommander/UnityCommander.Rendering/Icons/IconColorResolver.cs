
using System.Windows.Media;
using UnityCommander.Theme;
using UnityCommander.WPF;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconColorResolver : IIconColorResolver
    {
        public Brush Resolve(IconRole role, IconTone tone, VisualState state)
        {
            var icons = ThemeManager.CurrentTheme?.Palette.Icons;
            var palette = ThemeManager.CurrentTheme?.Palette;

            if (icons == null)
                return null;

            var brush = role switch
            {
                IconRole.RibbonAction => ResourceManager.Get<Brush>(palette?.Accent!),
                IconRole.SidebarItem =>  ResourceManager.Get<Brush>(palette?.Foreground!),
                IconRole.FilePanel => ResourceManager.Get<Brush>(icons.Default!),

                _ => ResourceManager.Get<Brush>(icons.Default!)
            };

            return tone switch
            {
                IconTone.Static => ResourceManager.Get<Brush>(icons.Default!),
                IconTone.Muted => ResourceManager.Get<Brush>(icons.Muted!),
                IconTone.Disabled => ResourceManager.Get<Brush>(icons.Disabled!),
                IconTone.Accent => ResourceManager.Get<Brush>(icons.Accent!),
                IconTone.Hover => ResourceManager.Get<Brush>(icons.Hover!),
                //IconTone.Selected => icons.Selected!,
                //IconTone.Error => icons.Error!,
                //IconTone.Warning => icons.Warning!,
                //IconTone.Success => icons.Success!,
                _ => ResourceManager.Get<Brush>(icons.Default!)
            };
        }
    }
}
