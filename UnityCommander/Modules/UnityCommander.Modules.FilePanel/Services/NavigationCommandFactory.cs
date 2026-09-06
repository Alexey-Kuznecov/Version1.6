
using Prism.Commands;
using System;
using System.Windows;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Panels;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Dialog;
using UnityCommander.Modules.FilePanel.Models;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;
using UnityCommander.WPF;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class NavigationCommandFactory // : INavigationCommandFactory
    {
        private readonly ActiveTab _activeTab;
        private readonly NavigationManager _navigation;
        private readonly ICommandUIService _ui;
        private readonly ISelectionManager _selection;
        private readonly ICreationService _creationService;
        private readonly IWindowManager _windowManager;
        private readonly IPopupService _popupService;

        public NavigationCommandFactory(
            ActiveTab activeTab,
            IPopupService popupService,
            ICreationService creationService,
            NavigationManager navigation,
            ISelectionManager selectionManager,
            ICommandUIService ui,
            IWindowManager windowManager)
        {
            _activeTab = activeTab;
            _popupService = popupService;
            _navigation = navigation;
            _ui = ui;
            _selection = selectionManager;
            _creationService = creationService;
            _windowManager = windowManager;
        }

        public UICommand CreateGoBackCommand<T>(
           string id,
           Func<bool> canExecute)
        {
            var command = _ui.Create<object>(
                id,
                new DelegateCommand<object>(
                    _ => _navigation.GoBack(),
                    _ => canExecute()),
                canExecute);

            _navigation.CurrentChanged += _ =>
                command.RefreshCanExecute();

            return command;
        }

        public UICommand CreateGoForwardCommand<T>(
            string id,
            Func<bool> canExecute)
        {
            var command = _ui.Create<object>(
                 id,
                 new DelegateCommand<object>(
                     _ => _navigation.GoForward(),
                     _ => canExecute()), 
                 canExecute);

            _navigation.CurrentChanged += _ =>
                  command.RefreshCanExecute();

            return command;
        }

        public UICommand CreateShowDrivesCommand(
            string id,
            Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<object>(x =>
                {
                    _navigation.TryNavigateTo(VirtualPaths.MyComputer, true);
                }),
                canExecute);
        }

        public UICommand CreateRefreshCommand(
        string id,
        Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<object>(x =>
                {
                    _navigation.TryNavigateTo(_activeTab.CurrentPath, true);
                }),
                canExecute);
        }

        public UICommand CreateCreationCommand(
           string id,
           Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<object>(parameter =>
                {
                    var request = _windowManager.ShowModalDialog<CreationDialogResult>(
                        "core.creation-item-dialog");

                    if (request is null)
                        return;

                    var path = _activeTab.CurrentPath;

                    var creation = 
                    new CreationContext(
                        request.Name,
                        path, 
                        request.Extension,
                        request.Type, 
                        null);

                    _creationService.CreateAsync(creation);
                }),
                canExecute);
        }

        public UICommand CreateCreationFileCommand(
           string id,
           Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<object>(obj =>
                {
                    if (obj is not FrameworkElement element)
                        return;

                    _popupService.Show<CreateFileViewModel>(
                        element, 
                        PopupPlacement.Bottom);
                }),
                canExecute);
        }

        public UICommand CreateCreationFolderCommand(
           string id,
           Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<object>(obj =>
                {
                    if (obj is not FrameworkElement element)
                        return;

                    _popupService.Show<CreateFolderViewModel>(
                       element,
                       PopupPlacement.Bottom);
                }),
                canExecute);
        }
    }
}
