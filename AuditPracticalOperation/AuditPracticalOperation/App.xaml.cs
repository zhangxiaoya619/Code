using Common;
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

            base.OnStartup(e);
        }
    }
}
