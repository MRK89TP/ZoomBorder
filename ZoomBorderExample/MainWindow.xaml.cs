using System.Windows;

namespace ZoomBorderExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ZoomBorder.Uniform();
        }

        private void BuoundingButton_Click(object sender, RoutedEventArgs e)
        {
            var boundingBox = Rect.Empty;

            foreach (var point in P1.Points)
            {
                var p = new Point(point.X + 50, point.Y + 50);
                boundingBox.Union(p);
            }

            foreach (var point in P2.Points)
            {
                var p = new Point(point.X + 100, point.Y + 100);
                boundingBox.Union(p);
            }


            ZoomBorder.ZoomOnRectangle(boundingBox);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomBorder.Reset();
        }
    }
}
