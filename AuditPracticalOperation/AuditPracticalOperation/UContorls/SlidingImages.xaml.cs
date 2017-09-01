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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuditPracticalOperation.UContorls
{
    /// <summary>
    /// SlidingImages.xaml 的交互逻辑
    /// </summary>
    public partial class SlidingImages : UserControl
    {
        public SlidingImages()
        {
            InitializeComponent();
            // this.SizeChanged += SlidingImages_SizeChanged;
            this.Loaded += SlidingImages_Loaded;
        }

        //void SlidingImages_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    PhotoCanvas.OnSizeChanged(this.ActualWidth, this.ActualHeight);
        //    //PhotoCanvas.SizeChanged()
        //}

        System.Threading.Thread thread;
        void SlidingImages_Loaded(object sender, RoutedEventArgs e)
        {

            int counts = this.PhotoCanvas.Children.Count;

            for (int i = 0; i < counts; i++)
            {
                RadioButton btn = new RadioButton();
                btn.Style = this.FindResource("RbtnStyle") as Style;
                btn.GroupName = "Index";
                RbtnPanel.Children.Add(btn);
            }

            ((RadioButton)RbtnPanel.Children[0]).IsChecked = true;


            thread = new System.Threading.Thread(SliderImg);
            thread.Start();
        }


        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            PhotoCanvas.OnSizeChanged(size.Width, size.Height);//this.ActualWidth, this.ActualHeight);
            return size;
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            if (this.ActualWidth>0) 
                PhotoCanvas.OnSizeChanged(this.ActualWidth, this.ActualHeight);//size.Width, size.Height);//
            return size;
        }
        private void CanvasWithPhoto_IndexChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            CanvasWithPhoto c = (CanvasWithPhoto)sender;
            UpdataRadioBtn(c.Index);
        }

        private void UpdataRadioBtn(int index)
        {
            if (RbtnPanel.Children[index - 1] is RadioButton)
                ((RadioButton)RbtnPanel.Children[index - 1]).IsChecked = true;
        }

        private void RbtnPanel_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)e.OriginalSource;
            for (int i = 0; i < RbtnPanel.Children.Count; i++)
            {
                if (btn == RbtnPanel.Children[i] && i + 1 != this.PhotoCanvas.Index)
                    this.PhotoCanvas.Index = i + 1;
            }
        }


        void SliderImg()
        {
            //int count = 0;
            //this.Dispatcher.BeginInvoke((Action)delegate
            //{
            //    count = RbtnPanel.Children.Count;
            //});
            //for (int i = 0; i < count; i++)
            //{
            //    this.Dispatcher.BeginInvoke((Action)delegate
            //    {
            //        ((RadioButton)RbtnPanel.Children[i]).IsChecked = true;
            //    });
            //    if (i == count - 1)
            //        i = 0;
            //    System.Threading.Thread.Sleep(1000);
            //}
        }
    }
}
