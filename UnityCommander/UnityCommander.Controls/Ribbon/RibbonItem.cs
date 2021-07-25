using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace UnityCommander.Controls.Ribbon
{
    using UnityCommander.Controls.Ribbon.Control;

    public class RibbonItem : ContentControl
    {
        #region Dependency Injection Fields

        private static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("ButtonIcon", typeof(Path), typeof(RibbonItem), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                OnPropertyChanged));

        private static readonly DependencyProperty BindCommandProperty =
            DependencyProperty.Register("BindCommand", typeof(ICommand), typeof(RibbonItem), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                OnPropertyChanged));

        #endregion
        
        private readonly ContentControl contentControl = new ();

        public Path Icon
        {
            get => (Path)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public ICommand BindCommand
        {
            get => (ICommand)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public RibbonItem()
        {
            this.contentControl.DataContext = new RibbonButton.RibbonControlModel();
            this.contentControl.Template = (ControlTemplate)Application.Current.FindResource("ContentControlTemplate");
            this.contentControl.Style = (Style)Application.Current.FindResource("ContentControlStyles");
            this.Content = this.contentControl;
        }

        public RibbonItem(RibbonButton button)
        {
            this.contentControl.DataContext = button.DataContext;
            this.contentControl.Template = button.Template;
            this.contentControl.Style = button.Style;
            this.Content = this.contentControl;
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RibbonItem ribbon && ribbon.contentControl.DataContext is RibbonButton.RibbonControlModel ribbonModel)
            {
                ribbonModel.Icon ??= e.NewValue as Path;
                ribbonModel.Command ??= e.NewValue as ICommand;
            }
        }
    }
}
