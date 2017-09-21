using Common;
using Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AuditPracticalOperation
{
    /// <summary>
    /// App.xaml 的交互逻辑   
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            base.OnStartup(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogHelper.LogError(e.Exception);
            MessageBox.Show("未知程序错误。");
            Process.GetCurrentProcess().Kill();
        }
    }
}
