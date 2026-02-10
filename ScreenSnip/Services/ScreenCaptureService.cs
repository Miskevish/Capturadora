using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ScreenSnip.Services
{
    public static class ScreenCaptureService
    {
        public static Task CaptureCopyAndSavePngAsync(Rectangle region)
        {
            return Task.Run(() =>
            {
                using var bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
                using var g = Graphics.FromImage(bmp);

                g.CopyFromScreen(region.Left, region.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

                // Guardar PNG
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    "Capturas"
                );

                Directory.CreateDirectory(folder);

                string filePath = Path.Combine(
                    folder,
                    $"Captura_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png"
                );

                bmp.Save(filePath, ImageFormat.Png);

                // Copiar al portapapeles
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var hBitmap = bmp.GetHbitmap();
                    try
                    {
                        var source = Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        Clipboard.SetImage(source);
                    }
                    finally
                    {
                        DeleteObject(hBitmap);
                    }
                });
            });
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
