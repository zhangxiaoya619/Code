using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// SlidingImages.xaml 的交互逻辑
    /// </summary>
    public partial class SlidingImages : UserControl
    {
        private Storyboard firstStoryboard;

        private Storyboard lastStoryboard;

        private DispatcherTimer timer;

        private bool isFirst;

        private const int WAIT_SECOND = 3;

        private bool isTimeRunning;

        public SlidingImages()
        {
            InitializeComponent();

            isFirst = true;
            firstStoryboard = (Storyboard)FindResource("first");
            lastStoryboard = (Storyboard)FindResource("last");
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, WAIT_SECOND);
            timer.Tick += Timer_Tick;
            timer.Start();
            isTimeRunning = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ChangeImage();
        }

        private void ChangeImage()
        {
            if (isFirst)
            {
                firstStoryboard.Begin();
                last.SetValue(Panel.ZIndexProperty, 500);
                first.SetValue(Panel.ZIndexProperty, 9999);
                lastRb.IsChecked = true;
            }
            else
            {
                lastStoryboard.Begin();
                first.SetValue(Panel.ZIndexProperty, 500);
                last.SetValue(Panel.ZIndexProperty, 9999);
                firstRb.IsChecked = true;
            }

            isFirst = !isFirst;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isTimeRunning)
            {
                timer.Stop();
                isTimeRunning = false;
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isTimeRunning)
            {
                timer.Start();
                isTimeRunning = true;
            }
        }

        private void lastRb_Click(object sender, RoutedEventArgs e)
        {
            if (!isFirst)
                return;

            firstStoryboard.Begin();
            last.SetValue(Panel.ZIndexProperty, 500);
            first.SetValue(Panel.ZIndexProperty, 9999);
            lastRb.IsChecked = true;
            isFirst = false;
        }

        private void firstRb_Click(object sender, RoutedEventArgs e)
        {
            if (isFirst)
                return;

            lastStoryboard.Begin();
            first.SetValue(Panel.ZIndexProperty, 500);
            last.SetValue(Panel.ZIndexProperty, 9999);
            firstRb.IsChecked = true;
            isFirst = true;
        }
    }
}
