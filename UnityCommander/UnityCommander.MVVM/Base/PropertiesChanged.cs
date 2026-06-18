
namespace UnityCommander.Mvvm.Base
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public class PropertiesChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> action)
        {
            if (Equals(storage, value))
                // ReSharper disable once StyleCop.SA1503
                return false;
            storage = value;
            this.RaisePropertyChanged(action);
            return true;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            this.RaisePropertyChanged(propertyName);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}