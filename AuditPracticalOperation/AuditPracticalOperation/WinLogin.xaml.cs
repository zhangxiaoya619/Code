using Business;
using Business.Processer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string currentName = string.Empty;
        private string currentID = string.Empty;
        public WinLogin()
        {
            InitializeComponent();
            this.Loaded += WinLogin_Loaded;
        }

        void WinLogin_Loaded(object sender, RoutedEventArgs e)
        {
            currentName = ConfigurationManager.AppSettings["UserName"];//zxy
            currentID = ConfigurationManager.AppSettings["UserID"];//yzxy
            string text = currentName + currentID;
            if (!string.IsNullOrEmpty(text))
            {
                TextBoxName.Text = text;
            }
            header.OnClose += header_OnClose;
        }
        private void AccessAppSettings(string key, string value)
        {
            //return;//zxy

            //获取Configuration对象
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            string name = config.AppSettings.Settings[key].Value;
            //写入<add>元素的Value
            config.AppSettings.Settings[key].Value = value;
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
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
            if (string.IsNullOrEmpty(TextBoxName.Text.Trim()) || TextBoxName.Text.Trim() == "请输入姓名+学号")
            {
                TextBoxName.Text = "请输入姓名+学号";
                return;
            }
            string id = RemoveNotNumber(TextBoxName.Text.Trim());
            string name = RemoveNumber(TextBoxName.Text.Trim());
            if (id != currentID)
            {
                currentID = id;
                AccessAppSettings("UserID", id);
            }
            if (name != currentName)
            {
                currentName = name;
                AccessAppSettings("UserName", name);
            }
            SingletonManager.Get<UserProcesser>().UserLogin(name, id);
            MainWindow main = new MainWindow();
            Application.Current.MainWindow = main;
            main.Show();
            this.Close();
        }

        /// 去掉字符串中的数字  
        public string RemoveNumber(string key)
        {
            return Regex.Replace(key, @"\d", "");
        }


        //去掉字符串中的非数字
        public string RemoveNotNumber(string key)
        {
            return Regex.Replace(key, @"[^\d]*", "");
        }

        private void TextBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginExecuted(this, null);
            }
        }

        private void TextBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((!string.IsNullOrEmpty(TextBoxName.Text.Trim())) && TextBoxName.Text.Trim() != "请输入姓名+学号")
            {
                TextBoxName.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
