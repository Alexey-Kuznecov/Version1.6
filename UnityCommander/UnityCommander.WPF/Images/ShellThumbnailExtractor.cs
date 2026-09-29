
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class ShellThumbnailExtractor : IShellThumbnailExtractor
    {
        public Task<ThumbnailResult?> ExtractAsync(
            string path,
            int size,
            CancellationToken token)
        {
            return Task.Run(
                () =>
                {
                    token.ThrowIfCancellationRequested();

                    return Extract(path, size);
                },
                token);
        }

        private static ThumbnailResult? Extract(
            string path,
            int size)
        {
            var initHr = NativeMethods.CoInitializeEx(
                IntPtr.Zero,
                NativeMethods.COINIT_MULTITHREADED);

            var shouldUninitialize =
                initHr == 0 ||
                initHr == 1;

            try
            {
                var iid =
                    typeof(IShellItemImageFactory).GUID;

                var hr =
                    NativeMethods.SHCreateItemFromParsingName(
                        path,
                        IntPtr.Zero,
                        ref iid,
                        out IShellItemImageFactory? factory);

                if (hr < 0 || factory is null)
                    return null;

                try
                {
                    var nativeSize = new NativeSize
                    {
                        Width = size,
                        Height = size
                    };

                    hr = factory.GetImage(
                       nativeSize,
                       SIIGBF.THUMBNAILONLY |
                       SIIGBF.RESIZETOFIT,
                       out var hBitmap);

                    if (hr < 0 || hBitmap == IntPtr.Zero)
                    {
                        hr = factory.GetImage(
                            nativeSize,
                            SIIGBF.ICON |
                            SIIGBF.RESIZETOFIT,
                            out hBitmap);
                    }

                    if (hr < 0 || hBitmap == IntPtr.Zero)
                        return null;

                    try
                    {
                        return ConvertBitmap(hBitmap);
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hBitmap);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(factory);
                }
            }
            finally
            {
                if (shouldUninitialize)
                    NativeMethods.CoUninitialize();
            }
        }

        private static ThumbnailResult ConvertBitmap(
            IntPtr hBitmap)
        {
            var source =
                Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            source.Freeze();

            using var stream = new MemoryStream();

            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(
                BitmapFrame.Create(source));

            encoder.Save(stream);

            return new ThumbnailResult(
                stream.ToArray(),
                source.PixelWidth,
                source.PixelHeight);
        }

        private static class NativeMethods
        {
            public const uint COINIT_MULTITHREADED = 0x0;

            [DllImport(
                "ole32.dll",
                ExactSpelling = true)]
            public static extern int CoInitializeEx(
                IntPtr pvReserved,
                uint dwCoInit);

            [DllImport(
                "ole32.dll",
                ExactSpelling = true)]
            public static extern void CoUninitialize();

            [DllImport(
                "shell32.dll",
                CharSet = CharSet.Unicode,
                ExactSpelling = true)]
            public static extern int SHCreateItemFromParsingName(
                string pszPath,
                IntPtr pbc,
                ref Guid riid,
                [MarshalAs(UnmanagedType.Interface)]
            out IShellItemImageFactory ppv);

            [DllImport(
                "gdi32.dll",
                ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject(
                IntPtr hObject);
        }

        [ComImport]
        [Guid("BCC18B79-BA16-442F-80C4-8A59C30C463B")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemImageFactory
        {
            [PreserveSig]
            int GetImage(
                NativeSize size,
                SIIGBF flags,
                out IntPtr phbm);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeSize
        {
            public int Width;
            public int Height;
        }

        [Flags]
        private enum SIIGBF
        {
            RESIZETOFIT = 0x00000000,
            BIGGERSIZEOK = 0x00000001,
            MEMORYONLY = 0x00000002,
            ICON = 0x00000004,
            THUMBNAILONLY = 0x00000008,
            INCACHEONLY = 0x00000010,
            CROPTOSQUARE = 0x00000020,
            WIDETHUMBNAILS = 0x00000040,
            ICONBACKGROUND = 0x00000080,
            SCALEUP = 0x00000100
        }
    }
}
