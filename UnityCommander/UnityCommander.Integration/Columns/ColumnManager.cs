
namespace UnityCommander.Integration.Columns
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The update column.
    /// </summary>
    public class ColumnManager
    {      
        /// <summary>
        /// Gets the update.
        /// </summary>
        private readonly List<UpdateColumnValue> updateCommand = new ();
        
        private readonly List<DependencyObject> pluginColumns = new ();

        private readonly List<HideColumnValue> hideColumnCommands = new ();
        
        private readonly List<AddColumn> addColumnCommands = new ();
        
        private readonly List<GetDirectoryContext> directoryContextCommands = new ();

        /// <summary>
        /// The update column value.
        /// </summary>
        public delegate void UpdateColumnValue();
        
        public delegate void HideColumnValue(DependencyObject column);
        
        public delegate void AddColumn(DependencyObject column);
        
        public delegate void GetDirectoryContext();

        /// <summary>
        /// The update.
        /// </summary>
        public void Update()
        {
            foreach (var command in this.updateCommand)
            {
                command();
            }
        }

        /// <summary>
        /// Удаляет колонку по заданному имени.
        /// </summary>
        public void Hide(string columnName)
        {
            for (int i = 0; i < hideColumnCommands.Count; i++)
            {
                if ((pluginColumns[i] as GridViewColumn)?.Header.ToString() == columnName)
                {
                    this.hideColumnCommands[i](pluginColumns[i]);
                }
            }
        }

        /// <summary>
        /// Добавляет колонку по заданному имени.
        /// </summary>
        public void Add(string columnName)
        {
            for (int i = 0; i < addColumnCommands.Count; i++)
            {
                if ((pluginColumns[i] as GridViewColumn)?.Header.ToString() == columnName)
                {
                    this.addColumnCommands[i](pluginColumns[i]);
                }
            }
          
        }

        public void GetCurrentDirectoryContext()
        {
            for (int i = 0; i < directoryContextCommands.Count; i++)
            {
                this.directoryContextCommands[i]();
            }
        }
        
        /// <summary>
        /// The set update.
        /// </summary>
        /// <param name="updateMethod">
        /// The update command.
        /// </param>
        public void SetUpdateCommand(UpdateColumnValue updateMethod)
        {
            this.updateCommand.Add(updateMethod);
        }

        /// <summary>
        /// The set update.
        /// </summary>
        /// <param name="hideColumnMethod"></param>
        /// <param name="addColumnMethod"></param>
        /// <param name="column"></param>
        public void SetHideCommand(HideColumnValue hideColumnMethod, AddColumn addColumnMethod, DependencyObject column)
        {
            this.hideColumnCommands.Add(hideColumnMethod);
            this.addColumnCommands.Add(addColumnMethod);
            this.pluginColumns.Add(column);
        }

        /// <summary>
        /// The set update.
        /// </summary>
        /// <param name="updateMethod">
        /// The update command.
        /// </param>
        public void SetDirectoryContextCommand(UpdateColumnValue updateMethod)
        {
            this.updateCommand.Add(updateMethod);
        }
    }
}
