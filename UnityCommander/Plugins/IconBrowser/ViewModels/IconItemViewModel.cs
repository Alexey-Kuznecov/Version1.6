
using IconBrowser.Mvvm.Base;
using IconBrowser.Services;
using IconMaker.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IconBrowser.ViewModels
{
    public sealed class IconItemViewModel
    {
        public IconDefinition Icon { get; }
        public DrawingBrush Brush { get; }
        public Style Style { get; set; }
        public ControlTemplate Template { get; set; }

        private readonly Action<Guid> _remove;
        
        private readonly Action<Guid, string> _rename;

        public IconItemViewModel(
            IconDefinition icon,
            Action<Guid> remove,
            Action<Guid, string> rename)
        {
            Icon = icon;
            Style = (Style)Application.Current.FindResource("IconStylesEditorColor");
            Template = (ControlTemplate)Application.Current.FindResource("IconTemplateEditorColor");
            Brush = IconBrushFactory.Create(icon);

            _remove = remove;
            _rename = rename;
        }

        public ICommand RemoveCommand =>
            new RelayCommand(() => _remove(Icon.Id));

        public ICommand RenameCommand =>
            new RelayCommand(() => _rename(Icon.Id, "new name"));
    }
}
