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
    /// MyAchievement.xaml 的交互逻辑
    /// </summary>
    public partial class MyAchievement : UserControl
    {
        public MyAchievement()
        {
            InitializeComponent();
            this.Loaded += MyAchievement_Loaded;
        }

        void MyAchievement_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock_UserName.Text = SingletonManager.Get<UserProcesser>().GetUser().Name;
            TextBlock_UserID.Text = SingletonManager.Get<UserProcesser>().GetUser().ID;
            TextBlock_ranking.Text = SingletonManager.Get<UserProcesser>().GetUser().Name;
            TextBlock_DoneCount.Text = SingletonManager.Get<PracticalItemProcesser>().GetHasDonePracticalItems().Count.ToString();
            TextBlock_DonePercent.Text = ((int)SingletonManager.Get<PracticalItemProcesser>().GetHasDonePracticalItems().Count / SingletonManager.Get<PracticalItemProcesser>().GetPracticalItems().Count * 100).ToString();
        }
    }
}
