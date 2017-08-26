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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// UserCenter.xaml 的交互逻辑
    /// </summary>
    public partial class UserCenter : UserControl
    {
        public UserCenter()
        {
            InitializeComponent();
            this.Loaded += UserCenter_Loaded;
        }

        void UserCenter_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextBlock_xuliehao.Text = "序列号：" + SingletonManager.Get<UserProcesser>().GetUser().CPUID;
        }
    }
}
