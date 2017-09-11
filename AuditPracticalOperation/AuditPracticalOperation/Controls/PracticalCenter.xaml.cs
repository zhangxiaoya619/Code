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
using System.Collections.Generic;
using Log;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalOperater.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalCenter : UserControl
    {
        public event Action OnBacked;

        private PracticalItem practicalItem;

        private bool isOpenDialog;

        public PracticalCenter(PracticalItem practicalItem)
        {
            isOpenDialog = false;
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

        public static RoutedUICommand F1HelpDialog = new RoutedUICommand("Show F1 Help", "ShowF1Help", typeof(PracticalCenter));
        private void F1HelpDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!practicalItem.IsNeedShowHelper)
                return;

            Helper helperWindow = new Helper(practicalItem);
            helperWindow.Owner = Application.Current.MainWindow;
            helperWindow.Closed += HelperWindowClosed;
            isOpenDialog = true;
            helperWindow.ShowDialog();
        }

        public static RoutedUICommand F2HelpDialog = new RoutedUICommand("Show F2 Help", "ShowF2Help", typeof(PracticalCenter));
        private void F2HelpDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!((IHelper)e.Parameter).IsNeedShowHelper)
                return;

            Helper helperWindow = new Helper((IHelper)e.Parameter);
            helperWindow.Owner = Application.Current.MainWindow;
            helperWindow.Closed += HelperWindowClosed;
            isOpenDialog = true;
            helperWindow.ShowDialog();
        }

        private void HelperWindowClosed(object sender, EventArgs e)
        {
            isOpenDialog = false;
            ((Helper)sender).Closed -= HelperWindowClosed;
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

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isOpenDialog && (e.Parameter == null ? true : ((IHelper)e.Parameter).IsNeedShowHelper);
        }
    }

    public class ProjectHasDoneCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Utils.IsInDesignMode())
                return 0;

            IList<PracticalItemProject> projects = (IList<PracticalItemProject>)value;
            ProjectHasDoneCountEnumType type = (ProjectHasDoneCountEnumType)parameter;

            switch (type)
            {
                case ProjectHasDoneCountEnumType.HasDone:
                    return projects.Count(project => project.IsDone);
                case ProjectHasDoneCountEnumType.DontHasDone:
                    return projects.Count - projects.Count(project => project.IsDone);
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
