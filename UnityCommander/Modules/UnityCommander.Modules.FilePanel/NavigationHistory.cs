
namespace UnityCommander.Modules.FilePanel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// Declares an <see langword="interface"/> for executing an operation.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Execute new command.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Undo the last command.
        /// </summary>
        public abstract void UnExecute();
    }

    /// <summary>
    /// Defines a binding between a Receiver <c>object</c> and an action
    /// implements Execute by invoking the corresponding operation(s) on Receiver <see cref="Navigator"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class NavigationCommand : Command
    {
        /// <summary>
        /// The receiver.
        /// </summary>
        private readonly INavigator navigator;

        /// <summary>
        /// The path.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationCommand"/> class.
        /// </summary>
        /// <param name="receiver"> The receiver. </param>
        /// <param name="path"> The path. </param>
        public NavigationCommand(ref INavigator receiver, string path)
        {
            this.navigator = receiver;
            this.path = path;
        }

        /// <summary>
        /// Execute new command.
        /// </summary>
        public override void Execute()
        {
            this.navigator?.Next(this.path);
        }

        /// <summary>
        /// Undo the last command.
        /// </summary>
        public override void UnExecute()
        {
            this.navigator?.Back(this.path);
        }
    }
    
    /// <summary>
    /// Knows how to perform the operations associated with carrying out the request.
    /// </summary>
    public class Navigator : Panel, INavigator
    {
        /// <summary>
        /// The bonded property.
        /// </summary>
        private static readonly DependencyProperty BondedProperty;

        /// <summary>
        /// The back path property.
        /// </summary>
        private static readonly DependencyProperty PreviousDirectoryProperty;

        /// <summary>
        /// The margin.
        /// </summary>
        private static double margin;

        #region Declaration constuctors

        /// <summary>
        /// Initializes static members of the <see cref="Navigator"/> class.
        /// </summary>
        static Navigator()
        {
            PreviousDirectoryProperty = DependencyProperty.Register(
                "PreviousDirectory",
                typeof(string),
                typeof(Navigator),
                new FrameworkPropertyMetadata("path",
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                    new PropertyChangedCallback(OnPreviousDirectoryChanged),
                    new CoerceValueCallback(CoercePreviousDirectory)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigator"/> class.
        /// </summary>
        public Navigator()
            : base()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Previous a directory path.
        /// </summary>
        public string PreviousDirectory
        {
            get => (string)GetValue(PreviousDirectoryProperty);
            set => this.SetValue(PreviousDirectoryProperty, value);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The back.
        /// </summary>
        /// <param name="path">
        /// The <c>path</c>.
        /// </param>
        public void Back(string path)
        {
            this.PreviousDirectory = path;
        }

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="path"> The <c>path</c>. </param>
        public void Next(string path)
        {
            InternalChildren.Clear();
            margin = 0;
            string[] slp = path.Split('\\');

            foreach (var item in slp)
            {
                Button button = new Button
                {
                    Height = 25, Content = item
                };

                InternalChildren.Add(button);
            }
        }

        #endregion

        #region Override methods

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required
        /// for child elements and determines a size for the FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements.
        /// Infinity can be specified as a value to indicate that the element will
        /// size to whatever content is available.
        /// </param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
          
            // In our example, we just have one child. 
            // Report that our panel requires just the size of its only child.
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(size);
            }

            return new Size();
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines
        /// a size for a FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this
        /// element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            margin = 0;

            for (var index = 0; index < InternalChildren.Count; index++)
            {
                UIElement child = this.InternalChildren[index];
                child.Arrange(new Rect(new Point(margin, 10), child.DesiredSize));
                margin += child.DesiredSize.Width + 2;

                if (margin - 10 > finalSize.Width)
                {
                    this.InternalChildren.RemoveAt(1);
                }
            }

            // Returns the final Arranged size
            return finalSize;
        }

        #endregion

        #region Declaration callback functions

        /// <summary>
        /// The coerce bonded.
        /// </summary>
        /// <param name="d"> The d. </param>
        /// <param name="basevalue"> The <c>base</c> value. </param>
        /// <returns> The <see cref="object"/>. </returns>
        private static object CoercePreviousDirectory(DependencyObject d, object basevalue)
        {
            return basevalue;
        }

        /// <summary>
        /// The on bonded changed.
        /// </summary>
        /// <param name="d"> The d. </param>
        /// <param name="e"> The e. </param>
        private static void OnPreviousDirectoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion
    }

    /// <summary>
    /// Asks the command to carry out the request.
    /// </summary>
    public class NavigationInvoker
    {
        /// <summary>
        /// The directory traversal history.
        /// </summary>
        private readonly List<Command> history = new List<Command>();

        /// <summary>
        /// The navigator.
        /// </summary>
        private INavigator navigator = new Navigator();

        /// <summary>
        /// The current index or path.
        /// </summary>
        private int currentIndex = -1;

        /// <summary>
        /// Gets a value indicating whether the command can be canceled.
        /// </summary>
        public bool CanUndo => this.history.Count > -1;

        /// <summary>
        /// Gets a value indicating whether the command can be repeated.
        /// </summary>
        public bool CanRedo => this.history.Count > -1 && this.currentIndex < this.history.Count - 1;

        /// <summary>
        /// Redo the command if possible.
        /// </summary>
        public void Next()
        {
            if (!this.CanRedo)
            {
                return;
            }

            this.history[++this.currentIndex]?.Execute();
        }

        /// <summary>
        /// Undo the command if possible.
        /// </summary>
        public void Previous()
        {
            if (this.CanUndo || --this.currentIndex > 0)
            {
                this.history[this.currentIndex]?.UnExecute();
            }
        }

        /// <summary>
        /// Add the command if possible.
        /// </summary>
        /// <param name="dirPath">
        /// The directory path. 
        /// </param>
        /// <returns>
        /// The <see cref="INavigator"/>.
        /// </returns>
        public INavigator Add(string dirPath)
        {
            Command command = new NavigationCommand(ref this.navigator, dirPath);
            this.currentIndex++;
            this.history.Add(command);
            this.history[this.currentIndex]?.Execute();
            if (this.currentIndex > 10)
            {
                this.currentIndex = 11;
                this.history.RemoveAt(0);
            }

            return this.navigator;
        }
    }
}
