using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using ViewModel;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalOperater.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalCenter : UserControl
    {
        public PracticalCenter(PracticalItem practicalItem)
        {
            InitializeComponent();
            container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = practicalItem });
        }
    }

    public class ProjectProcessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PracticalItem item = (PracticalItem)value;

            if (item.Projects.Count > 0)
                return (int)Math.Ceiling(item.Projects.Count(project => project.IsDone) * 1f / item.Projects.Count);
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
