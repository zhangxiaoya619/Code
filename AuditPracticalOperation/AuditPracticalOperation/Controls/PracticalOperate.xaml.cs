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
using Common;

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

        private IHelper f2Helper;

        private IHelper f1Helper;

        private KeyboardHook keyboardHook;

        private bool isOpenDialog;

        public PracticalOperate(IHelper f2Helper, int practicalID, PracticalItemProject project)
        {
            this.f1Helper = project;
            this.f2Helper = f2Helper;
            this.isOpenDialog = false;
            this.keyboardHook = new KeyboardHook();
            this.keyboardHook.SetHook();
            this.keyboardHook.OnKeyUp += KeyboardHook_OnKeyUp;

            InitializeComponent();

            if (!this.IsInDesignMode())
            {
                contentProcesser = new PracticalContentProcesser(practicalID, project.ID);
                container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = project });
                practicalFilePath = contentProcesser.LoadContent();
            }
        }

        private void KeyboardHook_OnKeyUp(System.Windows.Forms.Keys key, bool control)
        {
            if (control && key == System.Windows.Forms.Keys.F1)
                ShowHelper(f1Helper);
            else if (control && key == System.Windows.Forms.Keys.F2)
                ShowHelper(f2Helper);
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

            keyboardHook.UnHook();

            if (isDispose)
                framer.Dispose();

            if (OnBacked != null)
                OnBacked();
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            if (!isInit)
            {
                framer.Menubar = false;
                framer.Titlebar = false;
                framer.BorderStyle = DSOFramer.dsoBorderStyle.dsoBorderNone;
                masker.Height = editorContainer.ActualHeight;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object o)
                {
                    framer.Open(practicalFilePath, false, "Excel.Sheet", null, null);
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
            PracticalContentProcesser.KillAllProcess();
            contentProcesser.SaveContent();
            Close(false);
        }

        private void ShowHelper(IHelper helper)
        {
            if (isOpenDialog || !helper.IsNeedShowHelper)
                return;

            Helper helperWindow = new Helper(helper);
            helperWindow.Owner = Application.Current.MainWindow;
            helperWindow.Show();

            helperWindow.Closed += HelperWindowClosed;

            isOpenDialog = true;
        }

        private void HelperWindowClosed(object sender, EventArgs e)
        {
            isOpenDialog = false;
            ((Helper)sender).Closed -= HelperWindowClosed;
        }
    }
}
