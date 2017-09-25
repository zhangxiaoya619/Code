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
using ViewModel;
using System.Globalization;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// ConfigWIndow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWIndow : Window
    {
        private bool isLoaded = false;

        public ConfigWIndow()
        {
            InitializeComponent();

            this.Loaded += ConfigWIndow_Loaded;
            this.Closed += ConfigWIndow_Closed;
        }

        private void ConfigWIndow_Closed(object sender, EventArgs e)
        {
            SingletonManager.Get<ConfigProcesser>().SetConfig((Config)GetValue(DataContextProperty));
        }

        private void ConfigWIndow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                SetBinding(DataContextProperty, new Binding(".") { Source = SingletonManager.Get<ConfigProcesser>().GetConfig() });
            }
        }
    }

    public class OfficeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((OfficeTypeAttribute)typeof(OfficeTypeEnum).GetField(value.ToString()).GetCustomAttributes(typeof(OfficeTypeAttribute), false)[0]).Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
