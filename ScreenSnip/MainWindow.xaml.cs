using System;
using System.Windows;
using ScreenSnip.Services;

namespace ScreenSnip
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            try
            {
                var overlay = new SelectionOverlayWindow();
                bool? ok = overlay.ShowDialog();
                if (ok != true) return;

                var rect = overlay.SelectedRegion;
                if (rect.Width <= 0 || rect.Height <= 0) return;

                await ScreenCaptureService.CaptureCopyAndSavePngAsync(rect);

                MessageBox.Show("Copiado al portapapeles y guardado como PNG ✅",
                                "Listo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }
    }
}
