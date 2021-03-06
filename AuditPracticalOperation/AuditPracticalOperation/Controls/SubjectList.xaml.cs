﻿using Business;
using Business.Processer;
using Common.Utils;
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
    [Singleton]
    public partial class SubjectList : UserControl
    {
        private SubjectList()
        {
            InitializeComponent();

            if (!this.IsInDesignMode())
                practicalList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = SingletonManager.Get<PracticalItemProcesser>().GetPracticalItems() });
        }

        public static RoutedUICommand OpenPracticalOperate = new RoutedUICommand("Open Practical Operate", "OpenPracticalOperate", typeof(SubjectList));
        private void OpenPracticalOperateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PracticalItem practicalItem = (PracticalItem)e.Parameter;
            if (SingletonManager.Get<UserProcesser>().GetPower().HasPracticalPower(practicalItem.ID))
            {
                PracticalCenter practicalCenter = new PracticalCenter(practicalItem);
                practicalCenterContainer.Content = practicalCenter;
                practicalCenter.OnBacked += PracticalCenter_OnBacked;
                subjectListContainer.Visibility = Visibility.Collapsed;
                practicalCenterContainer.Visibility = Visibility.Visible;
            }
            else
            {
                WinBuyActivationCode winCode = new WinBuyActivationCode();
                winCode.ShowDialog();
            }
        }

        private void PracticalCenter_OnBacked()
        {
            ((PracticalCenter)practicalCenterContainer.Content).OnBacked -= PracticalCenter_OnBacked;
            practicalCenterContainer.Content = null;
            subjectListContainer.Visibility = Visibility.Visible;
            practicalCenterContainer.Visibility = Visibility.Collapsed;
        }
    }
}
