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
using AxDSOFramer;
using System.Windows.Threading;
using System.Diagnostics;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// PracticalOperate.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalOperate : UserControl
    {
        public event Action OnBacked;

        private PracticalContentProcesser contentProcesser;

        private string practicalFilePath;

        private bool isInit;

        private bool _isFramerDirty;

        public PracticalOperate(int practicalID, PracticalItemProject project)
        {
            InitializeComponent();

            if (!this.IsInDesignMode())
            {
                if (!CheckProcess())
                {
                    Close(true);
                }
                else
                {
                    contentProcesser = new PracticalContentProcesser(practicalID, project.ID);
                    container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = project });
                    practicalFilePath = contentProcesser.LoadContent();
                }
            }
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
                        KillAllProcess();
                        return true;
                    }
                }
            }

            return true;
        }

        private void KillAllProcess()
        {
            while (Process.GetProcessesByName("EXCEL").Count(process => process.ProcessName == "EXCEL") > 0)
            {
                foreach (Process IsProcedding in Process.GetProcessesByName("EXCEL"))
                    if (IsProcedding.ProcessName == "EXCEL")
                        if (!IsProcedding.HasExited)
                            IsProcedding.Kill();
            }
        }

        public static RoutedUICommand Back = new RoutedUICommand("Back To PracticalCenter", "BackToPracticalCenter", typeof(PracticalOperate));
        private void BackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close(true);
        }

        public static RoutedUICommand Save = new RoutedUICommand("Save", "Save", typeof(PracticalOperate));
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            framer.Save();
        }

        private void Close(bool isDispose)
        {
            framer.Close();

            if (isDispose)
                framer.Dispose();

            if (OnBacked != null)
                OnBacked();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInit)
            {
                framer.Menubar = false;
                framer.Titlebar = false;
                framer.BorderStyle = DSOFramer.dsoBorderStyle.dsoBorderNone;
                masker.Height = editorContainer.ActualHeight;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object o)
                {
                    framer.Open(practicalFilePath);
                    return null;
                }, null);

                isInit = true;
            }
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _isFramerDirty = true;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object o)
            {
                if (framer != null && _isFramerDirty)
                {
                    framer.Activate();
                }
                return null;
            }, null);
            framer.Activate();
        }

        private void DocumentOpened(object sender, _DFramerCtlEvents_OnDocumentOpenedEvent e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                System.Threading.Thread.Sleep(500);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    masker.Height = 0;
                    framer.Activate();
                }));
            });
        }

        private void DucomentSaveCompleted(object sender, _DFramerCtlEvents_OnSaveCompletedEvent e)
        {
            KillAllProcess();
            contentProcesser.SaveContent();
            Close(false);
        }
    }
}
