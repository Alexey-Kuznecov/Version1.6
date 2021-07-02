
namespace MultiColumns.DateTime
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Extentions.Helper;

    /// <summary>
    /// The home library book service.
    /// </summary>
    public class Plugin : IPluginDescriptor, IPluginImplement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        public Plugin()
        {
            this.Register = new List<Type>();
            Columns = new List<HostAppContext>();
            this.InitialData();
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        [AttachHandler(PluginScopes.Columns, typeof(PluginOptionHandler), nameof(IColumnService.GetColumns))]
        public static List<HostAppContext> Columns { get; set; }

        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        public List<Type> Register { get; set; }

        /// <summary>
        ///  Gets or sets plugin name.
        /// </summary>
        public string DisplayName { get; set; } = "DateTime Column";

        /// <summary>
        /// Gets or sets plugin description.
        /// </summary>
        public string Description { get; set; } = "Plugin provides a new column for date and time.";

        /// <summary>
        /// The register type.
        /// </summary>
        public void RegisterType()
        {
            this.Register.Add(typeof(DateTimeModel));
            this.Register.Add(typeof(DateTimeColumnModel));
        }

        /// <summary>
        /// The get unity context.
        /// </summary>
        /// <returns>
        /// The <see cref="HostAppContext"/>.
        /// </returns>
        public List<HostAppContext> SetHostAppContext()
        {
            return Columns;
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
        /// The get column value.
        /// </summary>
        /// <param name="path">
        /// The path
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object SetColumnValue(string path)
        {
            var dateTimeModel = new DateTimeModel();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            dateTimeModel.CreationTime = directoryInfo.CreationTime;
            dateTimeModel.LastAccessTime = directoryInfo.LastAccessTime;
            return dateTimeModel;
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            var unityContext = PluginScopes.Columns.Add(TargetPanel.Files, nameof(DateTimeModel.CreationTime), 150)
                    .AddBindingCommand(typeof(Plugin), nameof(this.SetColumnValue))
                    .AddRender(OptionRender.TextBlock)
                    .AddCommand(this.SortDateTime)
                    .AddContextItem("Date Filter", this.SetDateFilter)
                    .AddContextItem("Install Date and Time", this.SortDateTime);
            
            Columns.Add(unityContext);
        }
    }
}
