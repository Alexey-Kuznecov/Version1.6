
using System;

namespace UnityCommander.Abstractions.Dialog
{
    public interface IDialogAware<TResult>
      where TResult : IDialogResult
    {
        TResult? Result { get; }

        Action? RequestClose { get; set; }

        void OnDialogOpened(object? parameter);

        void OnDialogClosed();

        bool CanCloseDialog();
    }
}
