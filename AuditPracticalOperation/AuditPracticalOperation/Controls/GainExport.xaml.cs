using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Common.Utils;
using Business;
using Business.Processer;
using ViewModel;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// GainExport.xaml 的交互逻辑
    /// </summary>
    public partial class GainExport : UserControl
    {
        private IList<HasDonePracticalItem> items;
        private bool isSelected;

        public GainExport()
        {
            isSelected = true;

            InitializeComponent();

            if (!this.IsInDesignMode())
            {
                items = SingletonManager.Get<PracticalItemProcesser>().GetHasDonePracticalItems();
                practicalList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = items });
            }
        }

        public static RoutedUICommand CheckAll = new RoutedUICommand("Check all the has done", "CheckAll", typeof(SubjectList));
        private void CheckAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            for (int i = 0; i < items.Count; i++)
                items[i].IsSelected = isSelected;

            isSelected = !isSelected;
        }

        public static RoutedUICommand Export = new RoutedUICommand("Export the selected has done", "Export", typeof(SubjectList));
        private void ExportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog sfd = new System.Windows.Forms.FolderBrowserDialog();

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = System.IO.Path.Combine(sfd.SelectedPath,
               string.Format("{0}-{1}.prac", SingletonManager.Get<UserProcesser>().GetUser().Name, SingletonManager.Get<UserProcesser>().GetUser().ID));
                SingletonManager.Get<PracticalItemProcesser>().ExportPractical(fileName, items.Where(item => item.IsSelected).ToArray());
            }
        }

        private void CheckAllCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = items != null ? items.Count > 0 : false;
        }

        private void ExportCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = items != null ? items.Count(item => item.IsSelected) > 0 : false;
        }
    }
}
