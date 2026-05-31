
using Prism.Commands;
using System;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Navigation;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class NavigationCommandFactory // : INavigationCommandFactory
    {
        private readonly NavigationManager _navigation;
        private readonly ICommandUIService _ui;
        private readonly ISelectionManager _selection;

        public NavigationCommandFactory(
            NavigationManager navigation,
            ISelectionManager selectionManager,
            ICommandUIService ui)
        {
            _navigation = navigation;
            _ui = ui;
            _selection = selectionManager;
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
    }
}
