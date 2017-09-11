using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// ImageView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageView : UserControl
    {
        private const double SCROLL_SPEED = 50;
        private const double MAX_WIDTH = 6000;
        private const double MAX_HEIGHT = 6000;
        private const double SCALE = 1.3;

        private double MIN_HEIGHT;
        private double MIN_WIDTH;

        private byte[] imageSource;
        private bool isFirstLoad;

        private Point orign;
        private double xOffset;
        private double yOffset;

        private double xScale;
        private double yScale;
        private bool isScroll;

        public ImageView(byte[] imageSource)
        {
            this.imageSource = imageSource;
            this.isFirstLoad = true;
            InitializeComponent();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MIN_HEIGHT = this.ActualHeight / SCALE;
            MIN_WIDTH = this.ActualWidth / SCALE;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isFirstLoad)
            {
                BitmapImage bmp = null;
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(imageSource);

                bmp.EndInit();

                imgContainer.Width = MIN_WIDTH;
                imgContainer.Height = MIN_HEIGHT;
                img.Source = bmp;
                isFirstLoad = false;
            }
        }

        private void ImageScrollViewer_OnSetSize(ImageScrollViewer viewer, MouseWheelEventArgs e)
        {
            Point point = e.GetPosition(imgContainer);

            xScale = point.X / imgContainer.Width;
            yScale = point.Y / imgContainer.Height;

            if (e.Delta > 0 && (imgContainer.Width + SCROLL_SPEED <= MAX_WIDTH || imgContainer.Height + SCROLL_SPEED <= MAX_HEIGHT))
            {
                imgContainer.Width += SCROLL_SPEED;
                imgContainer.Height += SCROLL_SPEED;
                isScroll = true;
            }
            else if (e.Delta < 0 && (imgContainer.Width - SCROLL_SPEED >= MIN_WIDTH || imgContainer.Height - SCROLL_SPEED >= MIN_HEIGHT))
            {
                imgContainer.Width -= SCROLL_SPEED;
                imgContainer.Height -= SCROLL_SPEED;
                isScroll = true;
            }
        }

        private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!isScroll)
                return;

            isScroll = false;

            if (((ImageScrollViewer)sender).ScrollableWidth > 0)
                ((ImageScrollViewer)sender).ScrollToHorizontalOffset(((ImageScrollViewer)sender).ScrollableWidth * xScale);

            if (((ImageScrollViewer)sender).ScrollableHeight > 0)
                ((ImageScrollViewer)sender).ScrollToVerticalOffset(((ImageScrollViewer)sender).ScrollableHeight * yScale);
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            rotate.Angle = (rotate.Angle - 90) % 360;
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            rotate.Angle = (rotate.Angle + 90) % 360;
        }

        private void ImageScrollViewer_OnMouseDown(ImageScrollViewer viewer, MouseEventArgs e)
        {
            orign = e.GetPosition(this);
            xOffset = viewer.HorizontalOffset;
            yOffset = viewer.VerticalOffset;
        }

        private void ImageScrollViewer_OnMouseMove(ImageScrollViewer viewer, MouseEventArgs e)
        {
            viewer.ScrollToHorizontalOffset(xOffset + orign.X - e.GetPosition(this).X);
            viewer.ScrollToVerticalOffset(yOffset + orign.Y - e.GetPosition(this).Y);
        }
    }

    public class ImageScrollViewer : ScrollViewer
    {
        public event SetSizeHandler OnSetSize;

        public event ImageScroolViewerOnMouseDown OnImageScroolViewerOnMouseDown;

        public event ImageScroolViewerOnMouseMove OnImageScroolViewerOnMouseMove;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (OnSetSize != null)
                OnSetSize(this, e);

            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (OnImageScroolViewerOnMouseDown != null)
                OnImageScroolViewerOnMouseDown(this, e);

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (OnImageScroolViewerOnMouseMove != null && e.LeftButton == MouseButtonState.Pressed)
                OnImageScroolViewerOnMouseMove(this, e);

            e.Handled = true;
        }
    }

    public delegate void SetSizeHandler(ImageScrollViewer viewer, MouseWheelEventArgs e);
    public delegate void ImageScroolViewerOnMouseDown(ImageScrollViewer viewer, MouseEventArgs e);
    public delegate void ImageScroolViewerOnMouseMove(ImageScrollViewer viewer, MouseEventArgs e);
}
