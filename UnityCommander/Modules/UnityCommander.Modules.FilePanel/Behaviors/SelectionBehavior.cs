
using Prism.Ioc;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Selection;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;
using ILogger = UnityCommander.Logging.Contracts.ILogger;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class SelectionBehavior
    {
        private static ISelectionService _service;
        
        private static ISelectionService Service => _service ??= ContainerLocator.Container.Resolve<ISelectionService>();

        private static ITabContextAccessor _tabContextAccessor => ContainerLocator.Container.Resolve<ITabContextAccessor>();

        private static LoggerCreator logCreat = ContainerLocator.Container.Resolve<LoggerCreator>();
        
        private static ILogger logger;
           
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

        private static void OnManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView list && e.NewValue is ISelectionManager manager)
            {
                var tabId = _tabContextAccessor.ActiveTabId;

                Service.Register(tabId, manager);

                manager.SelectionChanged += () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SyncFromManager(list, manager);
                    });
                };
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
                list.SelectionMode = SelectionMode.Multiple; // отключаем стандартное выделение
                list.SelectedItem = null;

                list.PreviewMouseLeftButtonDown += OnMouseDown;

                logger = logCreat.Create(
                    category: LogCategory.UserAction,
                    scope: LogScope.UserAction
                    );
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var list = (ListView)sender;
            var container = list.ContainerFromElement((DependencyObject)e.OriginalSource) as ListViewItem;
            if (container == null)
            {
                GetManager(list).Clear();
                list.SelectedItems.Clear();
                return;
            }

            int index = list.ItemContainerGenerator.IndexFromContainer(container);
            var ctx = new SelectionContext(list.Items.Cast<ISelectableItem>());
            var manager = GetManager(list); // через AttachedProperty
            if (ctx == null || manager == null)
                return;

            if (list.SelectedItem is ISelectableItem select)
            {
                manager.FocusedItem = select;
            }

            bool ctrl =
                Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

            bool shift =
                Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            var action = new SelectionAction
            {
                Type = SelectionActionType.CtrlClick,
                TargetIndex = index
            };

            if (shift)
                action.Type = SelectionActionType.ShiftClick;
            else if (ctrl)
                action.Type = SelectionActionType.CtrlClick;
            else
                action.Type = SelectionActionType.SingleClick;

            //logger.Debug($" Action: {action.Type}");

            e.Handled = true;
            manager.Handle(ctx, action);
        }

        private static void SyncFromManager(ListView list, ISelectionManager manager)
        {
            list.SelectedItems.Clear();

            foreach (var item in list.Items)
            {
                if (item is BaseDirectory dir)
                {
                    //logger.Debug(dir.Path + $" is selected {dir.IsSelected}");
                }

                if (item is ISelectableItem select && select.IsSelected)
                {
                    list.SelectedItems.Add(select);
                }
            }
        }
    }
}
