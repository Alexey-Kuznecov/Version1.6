
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
        private List<UpdateColumnValue> updateCommand = new ();
        
        private List<DependencyObject> pluginColumns = new ();

        private List<HideColumnValue> hideColumnCommands = new ();
        
        private List<AddColumn> addColumnCommands = new ();

        /// <summary>
        /// The update column value.
        /// </summary>
        public delegate void UpdateColumnValue();
        
        public delegate void HideColumnValue(DependencyObject column);
        
        public delegate void AddColumn(DependencyObject column);

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
        /// The update.
        /// </summary>
        public void Hide(string columnName)
        {
            for (int i = 0; i < hideColumnCommands.Count; i++)
            {
                if ((pluginColumns[i] as GridViewColumn).Header.ToString() == columnName)
                {
                    this.hideColumnCommands[i](pluginColumns[i]);
                }
            }
        }

        public void Add(string columnName)
        {
            for (int i = 0; i < addColumnCommands.Count; i++)
            {
                if ((pluginColumns[i] as GridViewColumn).Header.ToString() == columnName)
                {
                    this.addColumnCommands[i](pluginColumns[i]);
                }
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
        /// <param name="updateMethod">
        /// The update command.
        /// </param>
        public void SetHideCommand(HideColumnValue hideColumnMethod, AddColumn addColumnMethod, DependencyObject column)
        {
            this.hideColumnCommands.Add(hideColumnMethod);
            this.addColumnCommands.Add(addColumnMethod);
            this.pluginColumns.Add(column);
        }
    }
}
