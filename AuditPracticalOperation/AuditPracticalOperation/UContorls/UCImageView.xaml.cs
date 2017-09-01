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

namespace AuditPracticalOperation.UContorls
{
    /// <summary>
    /// UCImageView.xaml 的交互逻辑
    /// </summary>
    public partial class UCImageView : UserControl
    {
        public UCImageView()
        {
            InitializeComponent();
        }
        public event UserClickHandler OnClose;
        public void LoadImage(byte[] imageSource)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(imageSource);
            image.EndInit();

            img.Source = image;
        }
        Point dragStart;
        int imageAngle;
        // <summary>
        /// 图片鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(root);
        }
        /// <summary>
        /// 图片鼠标滚轮滚动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mosePos = e.GetPosition(img);


            var scale = transform.ScaleX * (e.Delta > 0 ? 1.2 : 1 / 1.2);
            scale = Math.Max(scale, 1);


            transform.ScaleX = scale;
            transform.ScaleY = scale;


            if (scale == 1)        //缩放率为1的时候，复位
            {
                translate.X = 0;
                translate.Y = 0;
            }
            else                //保持鼠标相对图片位置不变
            {
                var newPos = e.GetPosition(img);


                translate.X += (newPos.X - mosePos.X);
                translate.Y += (newPos.Y - mosePos.Y);
            }
        }
        /// <summary>
        /// 图片鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }


            var current = e.GetPosition(root);
            //将坐标都转换成image坐标系下的坐标
            Point point0 = root.TranslatePoint(current, (UIElement)img);
            Point point1 = root.TranslatePoint(dragStart, (UIElement)img);
            translate.X += (point0.X - point1.X);
            translate.Y += (point0.Y - point1.Y);


            dragStart = current;
        }
        /// <summary>
        /// 图片左转事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            imageAngle = (imageAngle + 90) % 360;
            rotate.Angle = imageAngle;
        }
        /// <summary>
        /// 图片右转事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            imageAngle = (imageAngle - 90) % 360;
            rotate.Angle = imageAngle;
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (OnClose != null)
                OnClose();
        }
    }
}
