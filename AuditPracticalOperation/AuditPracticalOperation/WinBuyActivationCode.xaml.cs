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
    /// WinBuyActivationCode.xaml 的交互逻辑
    /// </summary>
    public partial class WinBuyActivationCode : Window
    {
        public WinBuyActivationCode()
        {
            InitializeComponent();
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox_Activation.Text.Trim()))
            {
                if (SingletonManager.Get<UserProcesser>().UserRegister(TextBox_Activation.Text.Trim()))
                {
                    //激活成功！
                    this.Visibility = Visibility.Collapsed;
                    WinActiveSuccess winSuccess = new WinActiveSuccess();
                    winSuccess.ShowDialog();
                    this.Close();
                }
                else
                {
                    //激活失败！
                    this.Visibility = Visibility.Collapsed;
                    WinActiveFail winFail = new WinActiveFail();
                    winFail.ShowDialog();
                    this.Close();
                }
            }
            else
            {
                TextBox_Activation.Text = "输入激活码";
                return;
            }
        }

        private void TextBox_Activation_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox_Activation.Text == "输入激活码")
                TextBox_Activation.Clear();
        }

        private void TextBox_Activation_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox_Activation.Text == "输入激活码" || string.IsNullOrEmpty(TextBox_Activation.Text))
                TextBox_Activation.Text = "输入激活码";
        }
    }
}
