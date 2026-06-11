
using System;
using System.Windows.Controls;

namespace UnityCommander.Common.Helper
{
    public interface IViewFactory
    {
        public UserControl Create(Type viewType);
    }
}