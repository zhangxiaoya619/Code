using Business;
using Business.Processer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ActivationCodeManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.header.OnClose += header_OnClose;
            this.Loaded += MainWindow_Loaded;
        }

        void header_OnClose()
        {
            this.Close();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Textbox_SerialNum.Focus();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Textbox_SerialNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnCreate_Click(this, null);
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            Textbox_ActivationCode.Text = SingletonManager.Get<ActivationProcessor>().getRNum(Textbox_SerialNum.Text);
        }





    }
}
