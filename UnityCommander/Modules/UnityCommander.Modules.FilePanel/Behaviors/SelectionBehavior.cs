
using Prism.Ioc;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Module;
using UnityCommander.Common.Selection;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class SelectionBehavior
    {
        private static ISelectionService _service;
        private static ISelectionService Service => _service ??= ContainerLocator.Container.Resolve<ISelectionService>();

        public static readonly DependencyProperty PanelIdProperty =
           DependencyProperty.RegisterAttached(
               "PanelId",
               typeof(string),
               typeof(SelectionBehavior),
               new PropertyMetadata(null));

        public static void SetPanelId(DependencyObject obj, string value) => obj.SetValue(PanelIdProperty, value);
        public static string GetPanelId(DependencyObject obj)=> (string)obj.GetValue(PanelIdProperty);

        public static readonly DependencyProperty ManagerProperty = DependencyProperty.RegisterAttached(
                "Manager",
                typeof(ISelectionManager),
                typeof(SelectionBehavior),
                new PropertyMetadata(null, OnManagerChanged));

        public static void SetManager(DependencyObject obj, ISelectionManager value) => obj.SetValue(ManagerProperty, value);
        public static ISelectionManager GetManager(DependencyObject obj) => (ISelectionManager)obj.GetValue(ManagerProperty);


        public static readonly DependencyProperty SelectionContextProperty =
                DependencyProperty.RegisterAttached(
                "SelectionContext",
                typeof(ISelectionContext),
                typeof(SelectionBehavior),
                new PropertyMetadata(null));

        public static void SetSelectionContext(DependencyObject obj, ISelectionContext value)
            => obj.SetValue(SelectionContextProperty, value);

        public static ISelectionContext GetSelectionContext(DependencyObject obj)
            => (ISelectionContext)obj.GetValue(SelectionContextProperty);

        private static void OnManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView list && e.NewValue is ISelectionManager manager)
            {
                var id = (string?)list.GetValue(PanelIdProperty);

                if (string.IsNullOrEmpty(id))
                {
                    // создаём GUID только если его нет
                    id = Guid.NewGuid().ToString();
                    list.SetValue(PanelIdProperty, id);
                }

                // регистрируем менеджер один раз
                var service = Service;
                service.Register(id, manager);
            }
        }

        public static readonly DependencyProperty EnableSelectionProperty =
            DependencyProperty.RegisterAttached(
                "EnableSelection",
                typeof(bool),
                typeof(SelectionBehavior),
                new PropertyMetadata(false, OnEnableChanged));
        public static void SetEnableSelection(DependencyObject obj, bool value)
    => obj.SetValue(EnableSelectionProperty, value);

        public static bool GetEnableSelection(DependencyObject obj)
            => (bool)obj.GetValue(EnableSelectionProperty);

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView list && e.NewValue is true)
            {
                list.SelectionMode = SelectionMode.Single; // отключаем стандартное выделение
                list.SelectedItem = null;

                list.PreviewMouseLeftButtonDown += OnMouseDown;
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var list = (ListView)sender;
            var container = list.ContainerFromElement((DependencyObject)e.OriginalSource) as ListViewItem;
            if (container == null)
                return;

            int index = list.ItemContainerGenerator.IndexFromContainer(container);

            // Получаем контекст и менеджер

            //if (list.DataContext is IDirectoryPanel panel) 
            //{
            //    panel.
            //}

            var ctx = list.DataContext as ISelectionContext;
            //var ctx = GetSelectionContext(list);
            var manager = GetManager(list); // через AttachedProperty
            if (ctx == null || manager == null)
                return;

            var action = new SelectionAction
            {
                Type = SelectionActionType.SingleClick,
                TargetIndex = index
            };

            manager.Handle(ctx, action);

            e.Handled = true;
        }
    }
}
