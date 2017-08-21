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
                contentProcesser = new PracticalContentProcesser(practicalID, project.ID);
                container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = project });
                practicalFilePath = contentProcesser.LoadContent();
            }
        }

        public static RoutedUICommand Back = new RoutedUICommand("Back To PracticalCenter", "BackToPracticalCenter", typeof(PracticalOperate));
        private void BackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            contentProcesser.SaveContent();

            framer.Close();

            if (OnBacked != null)
                OnBacked();
        }

        public static RoutedUICommand Save = new RoutedUICommand("Save", "Save", typeof(PracticalOperate));
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            framer.Close();

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
                    masker.Height = editorContainer.ActualHeight;
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
    }
}
