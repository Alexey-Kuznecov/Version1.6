
// ReSharper disable All
namespace IconBrowser.Converters
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    class ConverterXamlToBitmap
    {
        /// <summary>
        /// Создает и сохраняет изображение из кисти.
        /// </summary>
        /// <param name="iconBrush">Кисть подразумевается что она содержит геометрию или фигуру.</param>
        /// <param name="resName">Имя кисти.</param>
        public static void ConvertXamlToBitmap(DrawingBrush iconBrush, string resName, int size)
        {
            BitmapSource bitmapSource = BitmapSourceFromBrush(iconBrush);
            Image myImage = new Image();
            myImage.Width = size;
            myImage.Source = bitmapSource;

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (FileStream stream = new FileStream("../../Resources/" + resName + ".png", FileMode.Create))
                encoder.Save(stream);
        }
        /// <summary>
        /// Конвертирует кисть в изображения. 
        /// </summary>
        /// <param name="drawingBrush">Кисть содержащея геометрию изображения</param>
        /// <param name="size">Размер изображения</param>
        /// <param name="dpi">Плотность изображения</param>
        /// <returns></returns>
        public static BitmapSource BitmapSourceFromBrush(DrawingBrush drawingBrush, int size = 32, int dpi = 96)
        {
            // RenderTargetBitmap = builds a bitmap rendering of a visual
            var pixelFormat = PixelFormats.Pbgra32;
            RenderTargetBitmap rtb = new RenderTargetBitmap(size, size, dpi, dpi, pixelFormat);

            // Drawing visual allows us to compose graphic drawing parts into a visual to render
            var drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                // Declaring drawing a rectangle using the input brush to fill up the visual
                context.DrawRectangle(drawingBrush, null, new Rect(0, 0, size, size));
            }
            // Actually rendering the bitmap
            rtb.Render(drawingVisual);
            return rtb;
        }
    }
    
}
