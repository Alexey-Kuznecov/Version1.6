
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
    using UnityCommander.Integration.Extentions.Helper;

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
            this.Register = new List<Type>();
            this.InitialData();
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        [AttachHandler(PluginScopes.Columns, typeof(PluginOptionHandler), nameof(IColumnService.GetColumns))]
        public static List<IColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        public List<Type> Register { get; set; }

        /// <summary>
        /// The register type.
        /// </summary>
        public void RegisterType()
        {
            this.Register.Add(typeof(DateTimeModel));
            this.Register.Add(typeof(DateTimeColumnModel));
        }

        /// <summary>
        /// The set date filter.
        /// </summary>
        public void SetDateFilter()
        {
            MessageBox.Show("Filter date and time have been here");
        }

        /// <summary>
        /// The set date filter.
        /// </summary>
        public void SortDateTime()
        {
            MessageBox.Show("Sorting date and time have been here");
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            var column = PluginScopes.Columns
                    .Add(nameof(DateTimeModel.CreationTime), 150)
                    .AddRender(OptionRender.TextBlock)
                    .AddCommand(this.SortDateTime)
                    .AddContextItem("Date Filter", this.SetDateFilter)
                    .AddContextItem("Install Date and Time", this.SortDateTime);
        }
    }
}
