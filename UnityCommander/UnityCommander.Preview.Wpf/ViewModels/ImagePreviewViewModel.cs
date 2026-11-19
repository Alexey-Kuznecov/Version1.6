
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UnityCommander.Preview.Wpf.ViewModels;

public sealed class ImagePreviewViewModel : INotifyPropertyChanged
{
    private static readonly int[] ZoomLevels =
    {
        25, 50, 75, 100, 125, 150, 200, 300, 400
    };

    private BitmapSource? _source;
    private string _selectedZoom = "Fit";

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<string> ZoomOptions { get; } =
        new[]
        {
            "Fit",
            "25%",
            "50%",
            "75%",
            "100%",
            "125%",
            "150%",
            "200%",
            "300%",
            "400%"
        };

    public BitmapSource? Source
    {
        get => _source;
        private set => SetProperty(ref _source, value);
    }

    public string SelectedZoom
    {
        get => _selectedZoom;
        set => SetProperty(ref _selectedZoom, value);
    }

    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }
    public ICommand FitCommand { get; }
    public ICommand ActualSizeCommand { get; }

    public ImagePreviewViewModel()
    {
        ZoomInCommand = new RelayCommand(ZoomIn);
        ZoomOutCommand = new RelayCommand(ZoomOut);
        FitCommand = new RelayCommand(() => SelectedZoom = "Fit");
        ActualSizeCommand = new RelayCommand(() => SelectedZoom = "100%");
    }

    public void SetImage(BitmapSource source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        SelectedZoom = "Fit";
    }

    public void Clear()
    {
        Source = null;
        SelectedZoom = "Fit";
    }

    private void ZoomIn()
    {
        if (SelectedZoom == "Fit")
        {
            SelectedZoom = "100%";
            return;
        }

        var current = GetZoomPercent();

        foreach (var level in ZoomLevels)
        {
            if (level > current)
            {
                SelectedZoom = $"{level}%";
                return;
            }
        }
    }

    private void ZoomOut()
    {
        if (SelectedZoom == "Fit")
        {
            SelectedZoom = "50%";
            return;
        }

        var current = GetZoomPercent();

        for (var i = ZoomLevels.Length - 1; i >= 0; i--)
        {
            if (ZoomLevels[i] < current)
            {
                SelectedZoom = $"{ZoomLevels[i]}%";
                return;
            }
        }
    }

    private int GetZoomPercent()
    {
        var value = SelectedZoom.TrimEnd('%');

        return int.TryParse(value, out var zoom)
            ? zoom
            : 100;
    }

    private bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));

        return true;
    }

    private sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}