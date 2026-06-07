
using Prism.Dialogs;
using System.Windows;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Core.Mvvm;

namespace UnityCommander.Operation
{
    public enum CopyConflictAction
    {
        Skip,
        Replace,
        Cancel,
        ReplaceAll,
        SkipAll
    }

    public class CopyConflictResolver
    {
        private bool replaceAll = false;
        private bool skipAll = false;

        private readonly IDialogService dialogService;

        public CopyConflictResolver(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public CopyConflictAction ResolveConflict(CopyInfo info)
        {
            if (replaceAll) return CopyConflictAction.Replace;
            if (skipAll) return CopyConflictAction.Skip;

            // Показываем диалог пользователю и ждём результата
            var parameters = new OverrideDialogParameters(info);
            CopyConflictAction result = CopyConflictAction.Skip; // по умолчанию
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialogService.ShowDialog("CopyDialogSkipReplace", parameters, r =>
                {
                    // r может содержать результат Replace/Skip
                    //result = parameters.Result;
                });
            });
            return result;
        }

        public void SetReplaceAll() => replaceAll = true;
        public void SetSkipAll() => skipAll = true;
    }
}
