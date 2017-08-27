using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using ViewModel;
using System.Windows.Input;
using Common.Utils;
using System.Windows;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalOperater.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalCenter : UserControl
    {
        public event Action OnBacked;

        private PracticalItem practicalItem;

        public PracticalCenter(PracticalItem practicalItem)
        {
            this.practicalItem = practicalItem;
            InitializeComponent();

            if (!this.IsInDesignMode())
                projectContainer.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = practicalItem });
        }

        public static RoutedUICommand Back = new RoutedUICommand("Back To SubjectList", "BackToSubjectList", typeof(PracticalCenter));
        private void BackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnBacked != null)
                OnBacked();
        }

        public static RoutedCommand Operate = new RoutedUICommand("Open PracticalOperate", "OpenPracticalOperate", typeof(PracticalCenter));
        private void OperateExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            PracticalItemProject project = (PracticalItemProject)e.Parameter;
            PracticalOperate practicalOperate = new PracticalOperate(practicalItem, practicalItem.ID, project);
            operateContainer.Content = practicalOperate;
            practicalOperate.OnBacked += PracticalOperate_OnBacked;
            projectContainer.Visibility = Visibility.Collapsed;
            operateContainer.Visibility = Visibility.Visible;
        }

        private void PracticalOperate_OnBacked()
        {
            ((PracticalOperate)operateContainer.Content).OnBacked -= PracticalOperate_OnBacked;
            operateContainer.Content = null;
            projectContainer.Visibility = Visibility.Visible;
            operateContainer.Visibility = Visibility.Collapsed;
        }
    }

    public class ProjectProcessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Utils.IsInDesignMode())
                return 0;

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
            if (Utils.IsInDesignMode())
                return 0;

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

    public class ProjectNumNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (Utils.IsInDesignMode())
                return string.Empty;

            return string.Format("{0}、{1}", (int)values[0] + 1, values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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
