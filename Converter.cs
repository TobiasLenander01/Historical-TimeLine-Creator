using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// The System.Windows.Media.Brush class
    /// is not serializable. This class provides
    /// methods for converting a brush into a
    /// serializable System.Drawing.Color. And
    /// back to a System.Windows.Media.Brush.
    /// 
    /// Author:
    /// Tobias Lenander
    /// </summary>
    public static class Converter
    {
        public static System.Drawing.Color ConvertBrushToColor(System.Windows.Media.Brush brush)
        {
            var solidBrush = (System.Windows.Media.SolidColorBrush)brush;

            return System.Drawing.Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
        }

        public static System.Windows.Media.Brush ConvertColorToBrush(System.Drawing.Color color)
        {
            var mediaColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

            return new System.Windows.Media.SolidColorBrush(mediaColor);
        }

        public static System.Drawing.Bitmap? ConvertImageSourceToBitmap(System.Windows.Media.ImageSource? image)
        {
            if (image == null) 
                return null;

            BitmapSource bitmapSource = (BitmapSource)image;

            Bitmap bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            bitmapSource.CopyPixels(Int32Rect.Empty, bmpData.Scan0, bmpData.Height * bmpData.Stride, bmpData.Stride);

            bitmap.UnlockBits(bmpData);

            return bitmap;
        }

        public static System.Windows.Media.ImageSource? ConvertBitmapToImageSource(System.Drawing.Bitmap? bitmap)
        {
            if (bitmap == null)
                return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = memoryStream;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                imageSource.Freeze(); // Optional: Freeze the BitmapImage for improved performance

                return imageSource;
            }
        }
    }
}
