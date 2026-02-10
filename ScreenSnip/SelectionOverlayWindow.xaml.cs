using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

// Alias explícitos (clave)
using WpfPoint = System.Windows.Point;
using DrawingRect = System.Drawing.Rectangle;

namespace ScreenSnip
{
    public partial class SelectionOverlayWindow : Window
    {
        private WpfPoint _start;
        private WpfPoint _current;
        private bool _dragging;

        public DrawingRect SelectedRegion { get; private set; }

        public SelectionOverlayWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragging = true;
            _start = e.GetPosition(RootCanvas);
            SelectionRectangle.Visibility = Visibility.Visible;
            RootCanvas.CaptureMouse();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;

            _current = e.GetPosition(RootCanvas);
            UpdateRectangle();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_dragging) return;
            _dragging = false;

            RootCanvas.ReleaseMouseCapture();
            _current = e.GetPosition(RootCanvas);
            UpdateRectangle();

            var rect = Normalize(_start, _current);
            var dpi = VisualTreeHelper.GetDpi(this);

            int x = (int)((Left + rect.X) * dpi.DpiScaleX);
            int y = (int)((Top + rect.Y) * dpi.DpiScaleY);
            int w = (int)(rect.Width * dpi.DpiScaleX);
            int h = (int)(rect.Height * dpi.DpiScaleY);

            if (w < 5 || h < 5)
            {
                DialogResult = false;
                Close();
                return;
            }

            SelectedRegion = new DrawingRect(x, y, w, h);
            DialogResult = true;
            Close();
        }

        private void UpdateRectangle()
        {
            var r = Normalize(_start, _current);
            Canvas.SetLeft(SelectionRectangle, r.X);
            Canvas.SetTop(SelectionRectangle, r.Y);
            SelectionRectangle.Width = r.Width;
            SelectionRectangle.Height = r.Height;
        }

        private static Rect Normalize(WpfPoint a, WpfPoint b)
        {
            double x = Math.Min(a.X, b.X);
            double y = Math.Min(a.Y, b.Y);
            double w = Math.Abs(a.X - b.X);
            double h = Math.Abs(a.Y - b.Y);
            return new Rect(x, y, w, h);
        }
    }
}
