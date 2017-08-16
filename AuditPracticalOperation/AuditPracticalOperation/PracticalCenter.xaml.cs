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

namespace AuditPracticalOperation
{
    /// <summary>
    /// PracticalCenter.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalCenter : UserControl
    {
        public PracticalCenter()
        {
            InitializeComponent();
            practicallist.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = SingletonManager.Get<PracticalItemProcesser>().GetPracticalItems() });
        }

        public static RoutedUICommand OpenPracticalOperate = new RoutedUICommand("Open Practical Operate", "OpenPracticalOperate", typeof(PracticalCenter));
        private void OpenPracticalOperateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(SingletonManager.Get<UserProcesser>().GetPower().HasPower().ToString());
        }
    }
}
