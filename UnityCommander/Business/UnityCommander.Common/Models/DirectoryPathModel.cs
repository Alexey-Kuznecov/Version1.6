
namespace UnityCommander.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Metadata;
    using System.Windows.Controls;
    using System.Windows.Documents;

    /// <summary>
    /// The directory path model.
    /// </summary>
    public class History
    {
        /// <summary>
        /// The items.
        /// </summary>
        private readonly List<HistoryItem> items = new List<HistoryItem>();

        /// <summary>
        /// The path.
        /// </summary>
        private string path;

        /// <summary>
        /// The current index.
        /// </summary>
        private int currentIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryPathModel"/> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public History(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Gets a value indicating whether can undo.
        /// </summary>
        public bool CanUndo => this.items.Count > 0;

        /// <summary>
        /// The can redo.
        /// </summary>
        public bool CanRedo => this.items.Count > 0 && this.currentIndex < this.items.Count - 1;

        /// <summary>
        /// The undo.
        /// </summary>
        public void Undo()
        {
            if (!this.CanUndo)
            {
                return;
            }

            this.items[this.items.Count - 1]?.Undo(this.path);
            this.items.RemoveAt(this.items.Count - 1);
            this.currentIndex--;
        }

        /// <summary>
        /// The redo.
        /// </summary>
        public void Redo()
        {
            if (!this.CanRedo)
            {
                return;
            }

            this.currentIndex++;
            this.items[this.currentIndex]?.Redo(this.path);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(HistoryItem item)
        {
            this.CutOffHistory();
            this.items.Add(item);
            this.currentIndex++;
        }

        /// <summary>
        /// The cut off history.
        /// </summary>
        private void CutOffHistory()
        {
            int index = this.currentIndex++;
            if (index < this.items.Count)
            {
                this.items.RemoveRange(index, this.items.Count - index);
            }
        }

        /// <summary>
        /// The history item.
        /// </summary>
        public abstract class HistoryItem
        {
            /// <summary>
            /// The undo.
            /// </summary>
            /// <param name="path">
            /// The path.
            /// </param>
            public abstract void Undo(string path);

            /// <summary>
            /// The redo.
            /// </summary>
            /// <param name="path">
            /// The path.
            /// </param>
            public abstract void Redo(string path);
        }
    }
}
