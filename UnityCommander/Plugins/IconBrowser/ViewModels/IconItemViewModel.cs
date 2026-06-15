
using IconBrowser.Services;
using IconMaker.Core.Models;
using IconMaker.Core.Mvvm.Base;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IconBrowser.ViewModels
{
    public sealed class IconItemViewModel : PropertiesChanged
    {
        public IconDefinition Icon { get; }

        public Style Style { get; set; }

        public ControlTemplate Template { get; set; }

        private readonly Action<Guid> _remove;
        
        private readonly Action<Guid, string> _rename;

        private readonly Func<IconTheme> _themeProvider;

        public double Scale => _themeProvider().Scale;

        public Brush Brush => IconBrushFactory.CreateBrush(Icon, _themeProvider());

        public IconItemViewModel(
            IconDefinition icon,
            Func<IconTheme> themeProvider,
            Action<Guid> remove,
            Action<Guid, string> rename)
        {
            Icon = icon;
            Style = (Style)Application.Current.FindResource("IconStylesEditorColor");
            Template = (ControlTemplate)Application.Current.FindResource("IconTemplateEditorColor");
            //Brush = IconBrushFactory.Create(icon);
            
            _themeProvider = themeProvider;
            _remove = remove;
            _rename = rename;
        }

        public ICommand RemoveCommand =>
            new RelayCommand(() => _remove(Icon.Id));

        public ICommand RenameCommand =>
            new RelayCommand(() => _rename(Icon.Id, "new name"));

        public void Refresh()
        {
            OnPropertyChanged(nameof(Brush));
            OnPropertyChanged(nameof(Scale));
        }
    }
}
