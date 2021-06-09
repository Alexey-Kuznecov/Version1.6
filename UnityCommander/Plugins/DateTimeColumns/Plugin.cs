
namespace DateTimeColumns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The home library book service.
    /// </summary>
    [Export(typeof(IPluginImplements))]
    public class Plugin : IPluginImplements
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        public Plugin()
        {
           this.InitialData();
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        [OptionHandler(typeof(PluginOptionHandler), nameof(IColumnService.GetColumns))]
        public static List<IColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            Columns = new List<IColumn>
            {
                new DateTimeColumnModel
                {
                    IsDisplayed = true,
                    Header = nameof(DateTimeModel.CreationTime),
                    Template = new GridViewColumn
                    {
                        Header = nameof(DateTimeModel.CreationTime),
                        Width = 150,
                        DisplayMemberBinding =
                            new Binding { Path = new PropertyPath(nameof(DateTimeModel.CreationTime)) }
                    }
                },
                new DateTimeColumnModel
                {
                    IsDisplayed = false,
                    Header = nameof(DateTimeModel.LastAccessTime),
                    Template = new GridViewColumn
                    {
                        Header = nameof(DateTimeModel.LastAccessTime),
                        Width = 100,
                        DisplayMemberBinding = new Binding(nameof(DateTimeModel.LastAccessTime))
                    }
                }
            };
        }
    }
}
