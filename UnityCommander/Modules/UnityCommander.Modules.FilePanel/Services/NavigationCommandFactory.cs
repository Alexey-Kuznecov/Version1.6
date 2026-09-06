
using Prism.Commands;
using System;
using System.Reflection.Metadata;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Panels;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Dialog;
using UnityCommander.Modules.FilePanel.Models;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class NavigationCommandFactory // : INavigationCommandFactory
    {
        private readonly NavigationManager _navigation;
        private readonly ICommandUIService _ui;
        private readonly ISelectionManager _selection;
        private readonly ICreationService _creationService;
        private readonly IWindowManager _windowManager;

        public NavigationCommandFactory(
            ICreationService creationService,
            NavigationManager navigation,
            ISelectionManager selectionManager,
            ICommandUIService ui,
            IWindowManager windowManager)
        {
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
            return _ui.Create(
                id,
                new DelegateCommand<T>(x =>
                {
                    _navigation.GoBack();
                }),
                canExecute);
        }

        public UICommand CreateGoForwardCommand<T>(
         string id,
         Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<T>(x =>
                {
                    _navigation.GoForward();
                }),
                canExecute);
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
                    if (x != null)
                    {
                        _navigation.TryNavigateTo(x.ToString(), true);
                    }
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

                    if (parameter is not FilePanelContext context)
                        return;

                    var request = _windowManager.ShowModalDialog<CreationDialogResult>(
                        "core.creation-item-dialog");

                    if (request is null)
                        return;

                    var path = context.CurrentPath;

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
                new DelegateCommand<object>(parameter =>
                {

                    if (parameter is not FilePanelContext context)
                        return;

                    var path = context.CurrentPath;

                    var creation = new CreationContext("New File", path, ".txt", CreationType.File, null);

                    _creationService.CreateAsync(creation);
                }),
                canExecute);
        }

        public UICommand CreateCreationFolderCommand(
           string id,
           Func<bool> canExecute)
        {
            return _ui.Create(
                id,
                new DelegateCommand<object>(parameter =>
                {

                    if (parameter is not FilePanelContext context)
                        return;

                    var path = context.CurrentPath;

                    var creation = new CreationContext("New Folder", path, null, CreationType.Directory, null);

                    _creationService.CreateAsync(creation);
                }),
                canExecute);
        }
    }
}
