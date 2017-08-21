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

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// GainExport.xaml 的交互逻辑
    /// </summary>
    public partial class GainExport : UserControl
    {
        public GainExport()
        {
            InitializeComponent();
            gains = new ObservableCollection<ViewModel.GainItem>();
            gainList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = gains });
            this.Loaded += GainExport_Loaded;
        }

        void GainExport_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 20; i++)
            {
                gains.Add(new ViewModel.GainItem()
                    {
                        Name = "成果" + i,
                        Path = "",
                    });
            }
        }
        ObservableCollection<ViewModel.GainItem> gains;


        private void Button_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = true;
            foreach (var item in gains)
            {
                if (!item.IsChecked)
                    isChecked = false;
            }
            if (isChecked)
                foreach (var item in gains)
                {
                    item.IsChecked = false;
                }
            else
                foreach (var item in gains)
                {
                    item.IsChecked = true;
                }

        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
