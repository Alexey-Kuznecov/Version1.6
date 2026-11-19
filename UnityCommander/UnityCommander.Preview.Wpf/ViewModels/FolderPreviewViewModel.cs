
using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.Mvvm.Base;
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview.Wpf.ViewModels;

public sealed class FolderPreviewViewModel : PropertiesChanged
{
    private string? _name;
    private string? _path;

    private int _fileCount;
    private int _directoryCount;

    private long _totalSize;

    private DateTime _creationTime;
    private DateTime _lastModified;

    public string? Name
    {
        get => _name;
        private set
        {
            if (_name == value)
                return;

            _name = value;
            OnPropertyChanged();
        }
    }

    public string? Path
    {
        get => _path;
        private set
        {
            if (_path == value)
                return;

            _path = value;
            OnPropertyChanged();
        }
    }

    public int FileCount
    {
        get => _fileCount;
        private set
        {
            if (_fileCount == value)
                return;

            _fileCount = value;
            OnPropertyChanged();
        }
    }

    public int DirectoryCount
    {
        get => _directoryCount;
        private set
        {
            if (_directoryCount == value)
                return;

            _directoryCount = value;
            OnPropertyChanged();
        }
    }

    public long TotalSize
    {
        get => _totalSize;
        private set
        {
            if (_totalSize == value)
                return;

            _totalSize = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FormattedSize));
        }
    }

    public string FormattedSize => FormatSize(TotalSize);

    public DateTime CreationTime
    {
        get => _creationTime;
        private set
        {
            if (_creationTime == value)
                return;

            _creationTime = value;
            OnPropertyChanged();
        }
    }

    public DateTime LastModified
    {
        get => _lastModified;
        private set
        {
            if (_lastModified == value)
                return;

            _lastModified = value;
            OnPropertyChanged();
        }
    }

    private IReadOnlyList<ThumbnailResult> _thumbnails =
        Array.Empty<ThumbnailResult>();

    public IReadOnlyList<ThumbnailResult> Thumbnails
    {
        get => _thumbnails;
        private set => SetProperty(ref _thumbnails, value);
    }

    public void SetFolder(FolderPreviewContent content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        Name = System.IO.Path.GetFileName(
            content.Path.TrimEnd(
                System.IO.Path.DirectorySeparatorChar,
                System.IO.Path.AltDirectorySeparatorChar));

        if (string.IsNullOrEmpty(Name))
            Name = content.Path;

        Path = content.Path;

        FileCount = content.FileCount;
        DirectoryCount = content.DirectoryCount;

        TotalSize = content.TotalSize;

        CreationTime = content.CreationTime;
        LastModified = content.LastModified;
        Thumbnails = content.Thumbnails;
    }

    public void Clear()
    {
        Name = null;
        Path = null;

        FileCount = 0;
        DirectoryCount = 0;

        TotalSize = 0;

        CreationTime = default;
        LastModified = default;
    }

    private static string FormatSize(long size)
    {
        string[] units = { "Б", "КБ", "МБ", "ГБ", "ТБ" };

        double value = size;
        var unit = 0;

        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:N2} {units[unit]}";
    }
}
