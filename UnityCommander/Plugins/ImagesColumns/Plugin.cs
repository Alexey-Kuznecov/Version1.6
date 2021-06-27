
namespace ImagesColumns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Windows;

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
        public static List<HostAppContext> Columns { get; set; }

        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        public List<Type> Register { get; set; }

        /// <summary>
        /// Gets or sets plugin name.
        /// </summary>
        public string DisplayName { get; set; } = "Image columns";
        
        /// <summary>
        /// Gets or sets plugin description.
        /// </summary>
        public string Description { get; set; } = "Plugin creates additional columns for image files";

        /// <summary>
        /// The register type.
        /// </summary>
        public void RegisterType()
        {
            this.Register.Add(typeof(ImageModel));
            this.Register.Add(typeof(ImageColumnModel));
        }

        public static string GetVersion()
        {
            return "Version 2.8";
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
        public void SortDpi()
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
            var dateTimeModel = new ImageModel();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            dateTimeModel.Dpi = "100dpi";
            dateTimeModel.Sized = "100x300";
            dateTimeModel.Colors = "Red";
            return dateTimeModel;
        }

        /// <summary>
        /// The initial data.
        /// </summary>
        private void InitialData()
        {
            var context = PluginScopes.Columns.Add(TargetPanel.Files, nameof(ImageModel.Dpi), 50)
                .AddBindingCommand(typeof(Plugin), nameof(this.SetColumnValue))
                .AddRender(OptionRender.TextBlock)
                .AddCommand(this.SortDpi)
                .AddContextItem("Sort by dpi", this.SortDpi);

            var context2 = PluginScopes.Columns.Add(TargetPanel.Folders | TargetPanel.Files, nameof(ImageModel.Sized), 60)
                .AddBindingCommand(typeof(Plugin), nameof(this.SetColumnValue))
                .AddRender(OptionRender.TextBlock)
                .AddCommand(this.SortDpi)
                .AddContextItem("Sort by dpi", this.SortDpi);

            var context3 = PluginScopes.Columns.Add(TargetPanel.Folders, nameof(ImageModel.Colors), 50)
                .AddBindingCommand(typeof(Plugin), nameof(this.SetColumnValue))
                .AddRender(OptionRender.TextBlock)
                .AddCommand(this.SortDpi)
                .AddContextItem("Sort by dpi", this.SortDpi);

            context.RegisterType(PluginScopes.Columns, typeof(ImageModel));
            context2.RegisterType(PluginScopes.Columns, typeof(ImageModel));
            context3.RegisterType(PluginScopes.Columns, typeof(ImageModel));

            Columns.Add(context);
            Columns.Add(context2);
            Columns.Add(context3);
        }
    }
}
