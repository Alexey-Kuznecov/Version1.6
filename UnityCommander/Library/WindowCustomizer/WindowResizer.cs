
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantTypeSpecificationInDefaultExpression
namespace WindowCustomizer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    /// <summary>
    /// The dock position of the window
    /// </summary>
    public enum WindowDockPosition
    {
        /// <summary>
        /// Not docked
        /// </summary>
        Undocked,

        /// <summary>
        /// Docked to the left of the screen
        /// </summary>
        Left,

        /// <summary>
        /// Docked to the right of the screen
        /// </summary>
        Right,
    }

    /// <summary>
    /// The monitor options.
    /// </summary>
    public enum MonitorOptions : uint
    {
        /// <summary>
        /// The monitor default to null.
        /// </summary>
        MONITOR_DEFAULT_TO_NULL = 0x00000000,

        /// <summary>
        /// The monitor default to primary.
        /// </summary>
        MONITOR_DEFAULT_TO_PRIMARY = 0x00000001,

        /// <summary>
        /// The monitor default to nearest.
        /// </summary>
        MONITOR_DEFAULT_TO_NEAREST = 0x00000002
    }

    #region Dll Helper Structures

    /// <summary>
    /// The rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        /// <summary>
        /// The left.
        /// </summary>
        public int Left, Top, Right, Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="top">
        /// The top.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="bottom">
        /// The bottom.
        /// </param>
        public Rectangle(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    /// <summary>
    /// The min max info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        /// <summary>
        /// The point reserved.
        /// </summary>
        public POINT PointReserved;

        /// <summary>
        /// The point max size.
        /// </summary>
        public POINT PointMaxSize;

        /// <summary>
        /// The point max position.
        /// </summary>
        public POINT PointMaxPosition;

        /// <summary>
        /// The point min track size.
        /// </summary>
        public POINT PointMinTrackSize;

        /// <summary>
        /// The point max track size.
        /// </summary>
        public POINT PointMaxTrackSize;
    }

    /// <summary>
    /// The point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// x coordinate of point.
        /// </summary>
        public int X;

        /// <summary>
        /// y coordinate of point.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="POINT"/> struct. 
        /// Construct a point of coordinates (x,y).
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    #endregion

    /// <summary>
    /// Fixes the issue with Windows of Style <see cref="WindowStyle.None"/> covering the task bar
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1614:ElementParameterDocumentationMustHaveText", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1616:ElementReturnValueDocumentationMustHaveText", Justification = "Reviewed. Suppression is OK here.")]
    public class WindowResizer
    {
        #region Private Members

        /// <summary>
        /// The window to handle the resizing for
        /// </summary>
        private Window window;

        /// <summary>
        /// The last calculated available screen size
        /// </summary>
        private Rect screenSize = new Rect();

        /// <summary>
        /// How close to the edge the window has to be to be detected as at the edge of the screen
        /// </summary>
        private int mEdgeTolerance = 2;

        /// <summary>
        /// The transform matrix used to convert WPF sizes to screen pixels
        /// </summary>
        private Matrix transformToDevice;

        /// <summary>
        /// The last screen the window was on
        /// </summary>
        private IntPtr lastScreen;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mLastDock = WindowDockPosition.Undocked;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowResizer"/> class. 
        /// Default constructor
        /// </summary>
        /// <param name="window">
        /// The window to monitor and correctly maximize
        /// </param>
        public WindowResizer(Window window)
        {
            this.window = window;

            // Create transform visual (for converting WPF size to pixel size)
            this.GetTransform();

            // Listen out for source initialized to setup
            this.window.SourceInitialized += this.Window_SourceInitialized;

            // Monitor for edge docking
            this.window.SizeChanged += this.Window_SizeChanged;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Called when the window dock position changes
        /// </summary>
        public event Action<WindowDockPosition> WindowDockChanged = (dock) => { };

        #endregion

        #region Dll Imports

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        #endregion

        #region Initialize

        /// <summary>
        /// Gets the transform object used to convert WPF sizes to screen pixels
        /// </summary>
        private void GetTransform()
        {
            // Get the visual source
            var source = PresentationSource.FromVisual(this.window);

            // Reset the transform to default
            this.transformToDevice = default(Matrix);

            // If we cannot get the source, ignore

            // Otherwise, get the new transform object
            if (source?.CompositionTarget != null)
            {
                this.transformToDevice = source.CompositionTarget.TransformToDevice;
            }
        }

        /// <summary>
        /// Initialize and hook into the windows message pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // Get the handle of this window
            var handle = (new WindowInteropHelper(this.window)).Handle;
            var handleSource = HwndSource.FromHwnd(handle);

            // Hook into it's Windows messages
            handleSource?.AddHook(this.WindowProc);
        }

        #endregion

        #region Edge Docking

        /// <summary>
        /// Monitors for size changes and detects if the window has been docked (Aero snap) to an edge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // We cannot find positioning until the window transform has been established
            if (this.transformToDevice == default(Matrix))
            {
                return;
            }

            // Get the WPF size
            var size = e.NewSize;

            // Get window rectangle
            var top = this.window.Top;
            var left = this.window.Left;
            var bottom = top + size.Height;
            var right = left + this.window.Width;

            // Get window position/size in device pixels
            var windowTopLeft = this.transformToDevice.Transform(new Point(left, top));
            var windowBottomRight = this.transformToDevice.Transform(new Point(right, bottom));

            // Check for edges docked
            var edgedTop = windowTopLeft.Y <= (this.screenSize.Top + this.mEdgeTolerance);
            var edgedLeft = windowTopLeft.X <= (this.screenSize.Left + this.mEdgeTolerance);
            var edgedBottom = windowBottomRight.Y >= (this.screenSize.Bottom - this.mEdgeTolerance);
            var edgedRight = windowBottomRight.X >= (this.screenSize.Right - this.mEdgeTolerance);

            // Get docked position
            var dock = WindowDockPosition.Undocked;

            // Left docking
            if (edgedTop && edgedBottom && edgedLeft)
            {
                dock = WindowDockPosition.Left;
            }
            else if (edgedTop && edgedBottom && edgedRight)
            {
                dock = WindowDockPosition.Right;
            }
            else
            {
                // None
                dock = WindowDockPosition.Undocked;
            }

            // If dock has changed
            if (dock != this.mLastDock)
            {
                // Inform listeners
                this.WindowDockChanged(dock);
            }

            // Save last dock position
            this.mLastDock = dock;
        }

        #endregion

        #region Windows Proc

        /// <summary>
        /// Listens out for all windows messages for this window
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // Handle the GetMinMaxInfo of the Window
                case 0x0024: /* WM_GET_MIN_MAX_INFO */
                    this.WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (IntPtr)0;
        }

        #endregion

        /// <summary>
        /// Get the min/max window size for this window
        /// Correctly accounting for the task bar size and position
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            // Get the point position to determine what screen we are on
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            // Get the primary monitor at cursor position 0,0
            var lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULT_TO_PRIMARY);

            // Try and get the primary screen information
            var lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            // Now get the current screen
            var lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULT_TO_NEAREST);

            // If this has changed from the last one, update the transform
            if (lCurrentScreen != this.lastScreen || this.transformToDevice == default(Matrix))
            {
                this.GetTransform();
            }

            // Store last know screen
            this.lastScreen = lCurrentScreen;

            // Get min/max structure to fill with information
            var lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // If it is the primary screen, use the rcWork variable
            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.PointMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.PointMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.PointMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.PointMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                // Otherwise it's the rcMonitor values
                lMmi.PointMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.PointMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.PointMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.PointMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            // Set min size
            var minSize = this.transformToDevice.Transform(new Point(this.window.MinWidth, this.window.MinHeight));

            lMmi.PointMinTrackSize.X = (int)minSize.X;
            lMmi.PointMinTrackSize.Y = (int)minSize.Y;

            // Store new size
            this.screenSize = new Rect(lMmi.PointMaxPosition.X, lMmi.PointMaxPosition.Y, lMmi.PointMaxSize.X, lMmi.PointMaxSize.Y);

            // Now we have the max size, allow the host to tweak as needed
            Marshal.StructureToPtr(lMmi, lParam, true);
        }
    }

    /// <summary>
    /// The monitor info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    [SuppressMessage("ReSharper", "StyleCop.SA1401")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "StyleCop.SA1305")]
    [SuppressMessage("ReSharper", "StyleCop.SA1307")]
    public class MONITORINFO
    {
        /// <summary>
        /// Set this member to sizeof (MONITORINFO) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rectangle rcMonitor = new Rectangle();

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates.
        /// </summary>
        public Rectangle rcWork = new Rectangle();

        /// <summary>
        /// A set of flags that represent attributes of the display monitor.
        /// </summary>
        public int dwFlags = 0;
    }
}