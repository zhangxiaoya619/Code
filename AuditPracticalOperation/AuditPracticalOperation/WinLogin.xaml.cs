using Business;
using Business.Processer;
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
using System.Windows.Shapes;

namespace AuditPracticalOperation
{
    /// <summary>
    /// WinLogin.xaml 的交互逻辑
    /// </summary>
    public partial class WinLogin : Window
    {
        public WinLogin()
        {
            InitializeComponent();
            this.Loaded += WinLogin_Loaded;
        }

        void WinLogin_Loaded(object sender, RoutedEventArgs e)
        {
            header.OnClose += header_OnClose;
        }

        void header_OnClose()
        {
            this.Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void TextBoxName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxName.Text == "请输入姓名+学号")
                TextBoxName.Clear();
        }

        private void TextBoxName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text.Trim()))
                TextBoxName.Text = "请输入姓名+学号";
        }

        public static RoutedUICommand Login = new RoutedUICommand("Login", "Login", typeof(WinLogin));

        private void LoginExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //TODO:存储用户信息
            MainWindow main = new MainWindow();
            Application.Current.MainWindow = main;
            main.Show();
            this.Close();
        }
    }
}
