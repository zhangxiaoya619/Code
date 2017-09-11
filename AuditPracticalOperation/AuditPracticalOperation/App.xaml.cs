using Common;
using Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AuditPracticalOperation
{
    /// <summary>
    /// App.xaml 的交互逻辑   
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!OfficeOperator.IsInstall())
            {
                MessageBox.Show("本程序依赖于Microsoft Office，请先安装Microsoft Office。");
                Process.GetCurrentProcess().Kill();
            }

            DispatcherUnhandledException += OnDispatcherUnhandledException;

            base.OnStartup(e);
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogHelper.LogError(e.Exception);
            MessageBox.Show("未知程序错误。");
        }
    }
}
