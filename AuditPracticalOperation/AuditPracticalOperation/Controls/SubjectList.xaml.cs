using Business;
using Business.Processer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ViewModel;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalCenter.xaml 的交互逻辑
    /// </summary>
    public partial class SubjectList : UserControl
    {
        public SubjectList()
        {
            InitializeComponent();
            practicalList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = SingletonManager.Get<PracticalItemProcesser>().GetPracticalItems() });
        }

        public static RoutedUICommand OpenPracticalOperate = new RoutedUICommand("Open Practical Operate", "OpenPracticalOperate", typeof(SubjectList));
        private void OpenPracticalOperateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PracticalItem practicalItem = (PracticalItem)e.Parameter;
            if (SingletonManager.Get<UserProcesser>().GetPower().HasPower(practicalItem.ID))
            {
                practicalCenterContainer.Content = new PracticalCenter(practicalItem);
                subjectListContainer.Visibility = Visibility.Collapsed;
                practicalCenterContainer.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("你没有购买。");
            }
        }
    }
}
