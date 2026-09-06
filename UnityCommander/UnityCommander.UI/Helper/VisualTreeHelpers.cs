
using System.Windows;
using System.Windows.Media;

namespace UnityCommander.UI.Helper
{
    public static class VisualTreeHelpers
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        public static T? FindParent<T>(this DependencyObject? element)
            where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T result)
                    return result;

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
