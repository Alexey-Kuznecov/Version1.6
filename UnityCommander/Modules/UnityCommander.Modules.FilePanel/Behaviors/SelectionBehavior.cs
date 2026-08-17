
using Prism.Ioc;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Abstractions.Selection;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Selection;
using UnityCommander.Services.Interfaces;
using ILogger = UnityCommander.Logging.Contracts.ILogger;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class SelectionBehavior
    {
        private static ISelectionService _service;
        
        private static ISelectionService Service => _service ??= ContainerLocator.Container.Resolve<ISelectionService>();

        private static ITabContextAccessor _tabContextAccessor => ContainerLocator.Container.Resolve<ITabContextAccessor>();

        //private static LoggerCreator logCreat = ContainerLocator.Container.Resolve<LoggerCreator>();
        
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

                list.PreviewMouseLeftButtonDown += OnLeftMouseDown;
                list.PreviewMouseRightButtonDown += OnRightMouseDown;
                list.PreviewMouseMove += OnPreviewMouseMove;
                //logger = logCreat.Create(
                //    category: LogCategory.UserAction,
                //    scope: LogScope.UserAction
                //    );
            }
        }

        private static void OnLeftMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            var list = (ListView)sender;

            if (!TryGetSelectionTarget(
                    list,
                    e,
                    out var manager,
                    out var index))
            {
                manager?.ClearSelection();
                return;
            }

            var action = new SelectionAction
            {
                TargetIndex = index,
                Type = SelectionActionType.SingleClick
            };

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                action.Type = SelectionActionType.ShiftClick;
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                action.Type = SelectionActionType.CtrlClick;

            e.Handled = true;

            manager.Handle(action);
        }

        private static void OnRightMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            var list = (ListView)sender;


            if (!TryGetSelectionTarget(
                    list,
                    e,
                    out var manager,
                    out var index))
                return;


            var action = new SelectionAction
            {
                Type = SelectionActionType.ContextMenuClick,
                TargetIndex = index
            };

            manager.Handle(action);

            e.Handled = true;
        }

        private static bool TryGetSelectionTarget(
             ListView list,
             MouseButtonEventArgs e,
             out ISelectionManager manager,
             out int index)
        {
            manager = GetManager(list);
            index = -1;

            if (manager == null)
                return false;

            var container =
                list.ContainerFromElement(
                    (DependencyObject)e.OriginalSource)
                as ListViewItem;

            if (container == null)
                return false;

            index = list.ItemContainerGenerator.IndexFromContainer(container);

            manager.SetItems(
                list.Items.Cast<ISelectableItem>());

            return true;
        }

        private static void OnPreviewMouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var list = (ListView)sender;

            // Здесь пока просто диагностика
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
