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
using ViewModel;
using Common.Utils;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalOperate.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalOperate : UserControl
    {
        public event Action OnBacked;

        private PracticalContentProcesser contentProcesser;

        public PracticalOperate(PracticalItemProject project)
        {
            InitializeComponent();

            if (!this.IsInDesignMode())
            {
                contentProcesser = new PracticalContentProcesser(project);
                container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = project });
            }
        }

        public static RoutedUICommand Back = new RoutedUICommand("Back To PracticalCenter", "BackToPracticalCenter", typeof(PracticalOperate));
        private void BackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnBacked != null)
                OnBacked();
        }
    }
}
