using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using ViewModel;
using System.Windows.Input;
using Common.Utils;
using System.Windows;
using System.Diagnostics;
using Business;
using Business.Processer;

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

            UpdateProcessBar();
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
            if (!CheckProcess())
                return;

            PracticalItemProject project = (PracticalItemProject)e.Parameter;
            PracticalOperate practicalOperate = new PracticalOperate(practicalItem, practicalItem.ID, project);
            operateContainer.Content = practicalOperate;
            practicalOperate.OnBacked += PracticalOperate_OnBacked;
            projectContainer.Visibility = Visibility.Collapsed;
            operateContainer.Visibility = Visibility.Visible;
        }

        private bool CheckProcess()
        {
            foreach (Process IsProcedding in Process.GetProcessesByName("EXCEL"))
            {
                if (IsProcedding.ProcessName == "EXCEL")
                {
                    if (MessageBox.Show("打开实操文档需要关闭所有的EXCEL进程，请确认是否关闭所有EXCEL进程并打开实操文档？", "打开实操文档", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                    else
                    {
                        PracticalContentProcesser.KillAllProcess();
                        return true;
                    }
                }
            }

            return true;
        }

        private void PracticalOperate_OnBacked()
        {
            ((PracticalOperate)operateContainer.Content).OnBacked -= PracticalOperate_OnBacked;
            operateContainer.Content = null;
            projectContainer.Visibility = Visibility.Visible;
            operateContainer.Visibility = Visibility.Collapsed;

            practicalItem.UpdateState();

            UpdateProcessBar();
        }

        private void UpdateProcessBar()
        {
            if (practicalItem.Projects.Count == 0)
                processBar.Value = 0;
            else
                processBar.Value = (int)Math.Ceiling(practicalItem.Projects.Count(project => project.IsDone) * 1f / practicalItem.Projects.Count * 100);
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
