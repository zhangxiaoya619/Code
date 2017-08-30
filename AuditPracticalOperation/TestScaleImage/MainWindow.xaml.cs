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

namespace TestScaleImage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        Point dragStart;
        int imageAngle;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            imageView.LoadImage(@"E:\Projects\私单\AuditPracticalOperation\资料\审计证据列示\7.货币资金与业务循环\基金账户消户证明.jpg");
        }

    }
}
