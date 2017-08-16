using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using ViewModel;
using System.Windows.Input;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalOperater.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalCenter : UserControl
    {
        public event Action OnBacked;

        public PracticalCenter(PracticalItem practicalItem)
        {
            InitializeComponent();
            container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = practicalItem });
        }

        public static RoutedUICommand Back = new RoutedUICommand("Back To SubjectList ", "BackToSubjectList", typeof(PracticalCenter));
        private void BackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnBacked != null)
                OnBacked();
        }
    }

    public class ProjectProcessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PracticalItem item = (PracticalItem)value;

            if (item.Projects.Count > 0)
                return (int)Math.Ceiling(item.Projects.Count(project => project.IsDone) * 1f / item.Projects.Count * 100);
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectHasDoneCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PracticalItem item = (PracticalItem)value;
            ProjectHasDoneCountEnumType type = (ProjectHasDoneCountEnumType)parameter;

            switch (type)
            {
                case ProjectHasDoneCountEnumType.HasDone:
                    return item.Projects.Count(project => project.IsDone);
                case ProjectHasDoneCountEnumType.DontHasDone:
                    return item.Projects.Count - item.Projects.Count(project => project.IsDone);
                default: throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum ProjectHasDoneCountEnumType
    {
        HasDone,
        DontHasDone,
    }
}
