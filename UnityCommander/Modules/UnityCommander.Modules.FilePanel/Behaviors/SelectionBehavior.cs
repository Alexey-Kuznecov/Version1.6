
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Abstractions.Selection;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Selection;
using UnityCommander.Logging;
using UnityCommander.Logging.Core;
using UnityCommander.Services.Interfaces;
using UnityCommander.UI.Helper;
using UnityCommander.UI.Overlay;
using ILogger = UnityCommander.Logging.Contracts.ILogger;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class SelectionBehavior
    {
        private static ISelectionService _service;
        
        private static ISelectionService Service => _service ??= ContainerLocator.Container.Resolve<ISelectionService>();

        private static ITabContextAccessor _tabContextAccessor => ContainerLocator.Container.Resolve<ITabContextAccessor>();

        //private static LoggerCreator logCreat = ContainerLocator.Container.Resolve<LoggerCreator>();
        
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

                manager.SelectionChanged += () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    
                    {
                        SyncFromManager(list, manager);
                    });
                };
                list.ItemContainerGenerator.StatusChanged += (_, _) =>
                {
                    if (list.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                        return;

                    if (!manager.SelectFirstOnNextSync)
                        return;

                    manager.SetItems(
                        list.Items.Cast<ISelectableItem>());

                    manager.SelectFirst();

                    SyncFromManager(list, manager);
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

                list.PreviewMouseLeftButtonDown += OnPreviewLeftMouseDown;
                list.PreviewMouseRightButtonDown += OnRightMouseDown;
                list.PreviewKeyDown += OnPreviewKeyDown;
            }
        }

        private static void OnPreviewKeyDown(
          object sender,
          KeyEventArgs e)
        {
            var list = (ListView)sender;

            if (IsOverlayInput(e.OriginalSource))
                return;

            var manager = GetManager(list);

            if (manager is null || list.Items.Count == 0)
                return;

            if (e.Key is not (Key.Up or Key.Down))
                return;

            var current = manager.FocusedIndex;

            if (current < 0)
                current = 0;

            var offset = e.Key == Key.Up ? -1 : 1;
            var target = current + offset;

            var shift = Keyboard.Modifiers.HasFlag(
                ModifierKeys.Shift);

            // Для обычного движения разрешаем wrap-around.
            if (!shift)
            {
                if (target < 0)
                    target = list.Items.Count - 1;
                else if (target >= list.Items.Count)
                    target = 0;
            }
            else
            {
                // Для Shift не переходим через границу.
                if (target < 0 || target >= list.Items.Count)
                {
                    e.Handled = true;
                    return;
                }
            }

            manager.Handle(new SelectionAction
            {
                Type = shift
                    ? SelectionActionType.ShiftClick
                    : SelectionActionType.Move,

                TargetIndex = target,
                Offset = offset
            });

            e.Handled = true;

            ScrollToFocusedItem(list, manager, offset);
        }

        private static void OnPreviewLeftMouseDown(
             object sender,
             MouseButtonEventArgs e)
        {
            var list = (ListView)sender;

            if (IsOverlayInput(e.OriginalSource))
                return;

            if (!TryGetSelectionTarget(
                    list,
                    e,
                    out var manager,
                    out var index))
            {
                e.Handled = true;
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

        private static bool IsOverlayInput(object source)
        {
            if (source is not DependencyObject element)
                return false;

            return element.FindParent<OverlayHost>() != null;
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

        private static void ScrollToFocusedItem(
            ListView list,
            ISelectionManager manager,
            int direction)
        {
            var index = manager.FocusedIndex;

            if (direction == 1 && index > 0)
                index--;

            if (index < 0 || index >= list.Items.Count)
                return;

            list.ScrollIntoView(list.Items[index]);
        }

        private static void SyncFromManager(
          ListView list,
          ISelectionManager manager)
        {
            list.SelectedItems.Clear();

            foreach (var item in manager.SelectedItems)
            {
                if (list.Items.Contains(item))
                    list.SelectedItems.Add(item);
            }
        }
    }
}
