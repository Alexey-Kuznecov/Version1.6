
namespace UnityCommander.Integration.Columns
{
    using System.Collections.Generic;

    /// <summary>
    /// The update column.
    /// </summary>
    public class ColumnManager
    {      
        /// <summary>
        /// Gets the update.
        /// </summary>
        private List<UpdateColumnValue> updateCommand = new ();
        
        /// <summary>
        /// The update column value.
        /// </summary>
        public delegate void UpdateColumnValue();

        /// <summary>
        /// The update.
        /// </summary>
        public void Update()
        {
            foreach (var command in this.updateCommand)
            {
                command.Invoke();
            }
        }

        /// <summary>
        /// The set update.
        /// </summary>
        /// <param name="updateMethod">
        /// The update command.
        /// </param>
        public void SetUpdate(UpdateColumnValue updateMethod)
        {
            this.updateCommand.Add(updateMethod);
        }
    }
}
