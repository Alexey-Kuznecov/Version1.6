
namespace UnityCommander.Core
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    using UnityCommander.Common.Commands;

    /// <summary>
    /// The global command.
    /// </summary>
    public class GlobalCommand : IGlobalCommand
    {
        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler ExecuteChanged;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the source full name.
        /// </summary>
        public string SourceFullName { get; set; }

        /// <summary>
        /// Gets or sets the source type.
        /// </summary>
        public Type SourceType { get; set; }

        /// <summary>
        /// Gets or sets the base type.
        /// </summary>
        public Type BaseType { get; set; }

        /// <summary>
        /// Gets or sets the source method.
        /// </summary>
        public MethodInfo SourceMethod { get; set; }

        /// <summary>
        /// Gets or sets the source method.
        /// </summary>
        public Delegate Delegate { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public InputGesture ShortcutKey { get; set; }

        public GlobalCommandSelection SelectionFlag { get; set; }

        public void OnExecuteChanged(object sender)
        {
            this.ExecuteChanged?.Invoke(sender, new EventArgs());
        }
    }
}
