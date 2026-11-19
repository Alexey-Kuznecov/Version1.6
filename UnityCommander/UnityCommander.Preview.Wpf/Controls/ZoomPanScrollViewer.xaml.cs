using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace UnityCommander.Preview.Wpf.Controls;

public partial class ZoomPanScrollViewer : UserControl
{
    private static readonly int[] ZoomLevels =
    {
        25, 50, 75, 100, 125, 150, 200, 300, 400
    };

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source),
            typeof(BitmapSource),
            typeof(ZoomPanScrollViewer),
            new PropertyMetadata(null, OnSourceChanged));

    public static readonly DependencyProperty SelectedZoomProperty =
        DependencyProperty.Register(
            nameof(SelectedZoom),
            typeof(string),
            typeof(ZoomPanScrollViewer),
            new FrameworkPropertyMetadata(
                "Fit",
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedZoomChanged));

    private readonly ScaleTransform _scaleTransform =
        new(1, 1);

    private double _currentScale = 1;

    private Point? _pendingAnchor;

    private bool _isDragging;
    private Point _dragStart;

    private double _dragStartHorizontalOffset;
    private double _dragStartVerticalOffset;

    public BitmapSource? Source
    {
        get => (BitmapSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string SelectedZoom
    {
        get => (string)GetValue(SelectedZoomProperty);
        set => SetValue(SelectedZoomProperty, value);
    }

    public ZoomPanScrollViewer()
    {
        InitializeComponent();

        PART_Image.LayoutTransform = _scaleTransform;

        PART_ScrollViewer.SizeChanged += OnViewportSizeChanged;
        PART_ScrollViewer.ScrollChanged += OnScrollChanged;

        PART_ScrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

        PART_ScrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
        PART_ScrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
        PART_ScrollViewer.PreviewMouseMove += OnMouseMove;

        PART_ScrollViewer.LostMouseCapture += OnLostMouseCapture;
        PART_ScrollViewer.MouseEnter += OnMouseEnter;

        Loaded += OnLoaded;
    }

    private static void OnSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ZoomPanScrollViewer)d;

        control.PART_Image.Source = (BitmapSource?)e.NewValue;

        if (e.NewValue is BitmapSource source)
        {
            control.PART_Image.Width = source.PixelWidth;
            control.PART_Image.Height = source.PixelHeight;

            control.SetCurrentValue(SelectedZoomProperty, "Fit");

            control.ApplySelectedZoom();
        }
        else
        {
            control.PART_Image.Width = double.NaN;
            control.PART_Image.Height = double.NaN;

            control._currentScale = 1;

            control._scaleTransform.ScaleX = 1;
            control._scaleTransform.ScaleY = 1;
        }
    }

    private static void OnSelectedZoomChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ZoomPanScrollViewer)d;

        control.ApplySelectedZoom(control._pendingAnchor);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplySelectedZoom();
    }

    private void OnViewportSizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        if (string.Equals(
            SelectedZoom,
            "Fit",
            StringComparison.OrdinalIgnoreCase))
        {
            ApplySelectedZoom();
        }

        UpdateCursor();
    }

    private void OnScrollChanged(
        object sender,
        ScrollChangedEventArgs e)
    {
        UpdateCursor();
    }

    private void ApplySelectedZoom(Point? anchor = null)
    {
        if (Source == null)
            return;

        var newScale = GetScale();

        if (newScale <= 0)
            return;

        var anchorPoint = anchor ?? new Point(
            PART_ScrollViewer.ViewportWidth / 2,
            PART_ScrollViewer.ViewportHeight / 2);

        var sourcePoint = GetSourcePoint(anchorPoint);

        _currentScale = newScale;

        _scaleTransform.ScaleX = newScale;
        _scaleTransform.ScaleY = newScale;

        Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            new Action(() =>
            {
                RestoreAnchor(sourcePoint, anchorPoint);
                UpdateCursor();
            }));
    }

    private double GetScale()
    {
        if (Source == null)
            return 1;

        if (string.Equals(
            SelectedZoom,
            "Fit",
            StringComparison.OrdinalIgnoreCase))
        {
            var viewportWidth = PART_ScrollViewer.ViewportWidth;
            var viewportHeight = PART_ScrollViewer.ViewportHeight;

            if (viewportWidth <= 0 || viewportHeight <= 0)
                return 1;

            var widthScale = viewportWidth / Source.PixelWidth;
            var heightScale = viewportHeight / Source.PixelHeight;

            return Math.Max(
                0.01,
                Math.Min(widthScale, heightScale));
        }

        var value = SelectedZoom.TrimEnd('%');

        if (!double.TryParse(
            value,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var zoom))
        {
            return 1;
        }

        return Math.Max(0.01, zoom / 100d);
    }

    private Point GetSourcePoint(Point viewportPoint)
    {
        if (Source == null || _currentScale <= 0)
            return new Point();

        var originX = GetCenteredOrigin(
            Source.PixelWidth * _currentScale,
            PART_ScrollViewer.ViewportWidth);

        var originY = GetCenteredOrigin(
            Source.PixelHeight * _currentScale,
            PART_ScrollViewer.ViewportHeight);

        return new Point(
            (PART_ScrollViewer.HorizontalOffset + viewportPoint.X - originX)
                / _currentScale,

            (PART_ScrollViewer.VerticalOffset + viewportPoint.Y - originY)
                / _currentScale);
    }

    private void RestoreAnchor(
        Point sourcePoint,
        Point viewportPoint)
    {
        if (Source == null)
            return;

        var originX = GetCenteredOrigin(
            Source.PixelWidth * _currentScale,
            PART_ScrollViewer.ViewportWidth);

        var originY = GetCenteredOrigin(
            Source.PixelHeight * _currentScale,
            PART_ScrollViewer.ViewportHeight);

        var horizontalOffset =
            sourcePoint.X * _currentScale + originX - viewportPoint.X;

        var verticalOffset =
            sourcePoint.Y * _currentScale + originY - viewportPoint.Y;

        PART_ScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
        PART_ScrollViewer.ScrollToVerticalOffset(verticalOffset);
    }

    private static double GetCenteredOrigin(
        double contentSize,
        double viewportSize)
    {
        return contentSize < viewportSize
            ? (viewportSize - contentSize) / 2
            : 0;
    }

    #region Mouse wheel zoom

    private void OnPreviewMouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        if (Source == null)
            return;

        var nextZoom = GetNextZoomLevel(e.Delta > 0 ? 1 : -1);

        if (nextZoom.HasValue)
        {
            _pendingAnchor = e.GetPosition(PART_ScrollViewer);

            try
            {
                SetCurrentValue(
                    SelectedZoomProperty,
                    $"{nextZoom.Value}%");
            }
            finally
            {
                _pendingAnchor = null;
            }
        }

        e.Handled = true;
    }

    private int? GetNextZoomLevel(int direction)
    {
        var currentZoom = _currentScale * 100;

        if (direction > 0)
        {
            foreach (var level in ZoomLevels)
            {
                if (level > currentZoom + 0.5)
                    return level;
            }
        }
        else
        {
            for (var i = ZoomLevels.Length - 1; i >= 0; i--)
            {
                if (ZoomLevels[i] < currentZoom - 0.5)
                    return ZoomLevels[i];
            }
        }

        return null;
    }

    #endregion

    #region Mouse pan

    private bool CanPan =>
        PART_ScrollViewer.ScrollableWidth > 1 ||
        PART_ScrollViewer.ScrollableHeight > 1;

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        UpdateCursor();
    }

    private void OnMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (Source == null || !CanPan)
            return;

        _isDragging = true;

        _dragStart = e.GetPosition(PART_ScrollViewer);

        _dragStartHorizontalOffset =
            PART_ScrollViewer.HorizontalOffset;

        _dragStartVerticalOffset =
            PART_ScrollViewer.VerticalOffset;

        if (!PART_ScrollViewer.CaptureMouse())
        {
            _isDragging = false;
            return;
        }

        UpdateCursor();

        e.Handled = true;
    }

    private void OnMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (!_isDragging)
            return;

        var current = e.GetPosition(PART_ScrollViewer);

        var deltaX = current.X - _dragStart.X;
        var deltaY = current.Y - _dragStart.Y;

        PART_ScrollViewer.ScrollToHorizontalOffset(
            _dragStartHorizontalOffset - deltaX);

        PART_ScrollViewer.ScrollToVerticalOffset(
            _dragStartVerticalOffset - deltaY);

        e.Handled = true;
    }

    private void OnMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (!_isDragging)
            return;

        _isDragging = false;

        if (PART_ScrollViewer.IsMouseCaptured)
            PART_ScrollViewer.ReleaseMouseCapture();

        UpdateCursor();

        e.Handled = true;
    }

    private void OnLostMouseCapture(
        object sender,
        MouseEventArgs e)
    {
        if (!_isDragging)
            return;

        _isDragging = false;

        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (_isDragging)
        {
            PART_ScrollViewer.Cursor = Cursors.SizeAll;
        }
        else
        {
            PART_ScrollViewer.Cursor =
                CanPan ? Cursors.Hand : Cursors.Arrow;
        }
    }

    #endregion
}