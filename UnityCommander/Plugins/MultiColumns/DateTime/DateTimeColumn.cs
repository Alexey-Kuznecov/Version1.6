
namespace MultiColumns.DateTime
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    //public class DateTimeColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor, IPluginSettings
    //{
    //    private string dateTimeFormat;
        
    //    private bool includeTime;

    //    private DateTimeSettings settings;

    //    private ColumnManager manager;

    //    private OptionRender optionRender;

    //    public DateTimeColumn()
    //    {
    //        this.dateTimeFormat = "15/3/2008";
    //    }

    //    public List<object> DateTimeFormat { get; set; }

    //    public string DisplayName { get; set; } = "Date creation column";

    //    public string Description { get; set; } = "Date creation columns";
        
    //    public void ColumnInitial(ColumnBuilder builder)
    //    {
    //        builder.Add("Creation Date", 100);
    //        builder.AddContextItem("Select date format", this.InstallMod);
    //        builder.AddContextItem("Edit date format", this.InstallMod);
    //    }

    //    public object ColumnValueHandler(string columnName, string path, DirectoryItemType directoryItem)
    //    {
    //        DirectoryInfo directoryInfo = new DirectoryInfo(path);
    //        var nt = directoryInfo.CreationTime.ToLongTimeString();
    //        var nd = default(string);

    //        switch (this.dateTimeFormat)
    //        {
    //            case "15/3/2008":
    //            {
    //                var d = directoryInfo.CreationTime.Date;
    //                CultureInfo culture = new CultureInfo("pt-BR");
    //                nd = d.ToString("d", culture);
    //                break;
    //            }
    //            case "15.3.2008":
    //            {
    //                var d = directoryInfo.CreationTime.Date;
    //                nd = d.ToString("d");
    //                break;
    //            }
    //        }

    //        return this.includeTime ? nd + " " + nt : nd;
    //    }

    //    public void OnSettingsChanged(SettingsBase settings)
    //    {
    //        if (!(settings is DateTimeSettings myBase)) return;
            
    //        this.settings = myBase;

    //        if (this.settings.GetDateTimeFormat() != null)
    //        {
    //            this.dateTimeFormat = this.settings.GetDateTimeFormat() == "15/3/2008" ? "15/3/2008" : "15.3.2008";
    //        }

    //        this.manager.Update();
    //    }

    //    public void UpdateColumnValue(ColumnManager columnManager)
    //    {
    //        this.manager = columnManager;
    //    }

    //    public OptionRender ColumnValueRender()
    //    {
    //        this.optionRender = OptionRender.TextBlock;
    //        return this.optionRender;
    //    }

    //    public void OptionBuild(OptionBuilder optionBuilder)
    //    {
    //        optionBuilder.Add(
    //            "Select date format:", 
    //            this.DateTimeFormat, 
    //            dateTimeFormat, 
    //            this.DateTimeFormatHandler,
    //            OptionRender.DropBox);

    //        optionBuilder.Add("Shown date and time:", this.includeTime, this.IncludeTimeHandler, OptionRender.Checkbox);
    //    }

    //    private void IncludeTimeHandler(bool value)
    //    {
    //        this.includeTime = value;      
    //    }

    //    private void DateTimeFormatHandler(object selected)
    //    {
    //        dateTimeFormat = selected as string;
    //    }

    //    private void InstallMod(string path)
    //    {
    //        MessageBox.Show("Date Columns: " + path);
    //    }

    //    public object ColumnValueValidate(IPluginContext context)
    //    {
    //        return context;
    //    }
    //}
}
