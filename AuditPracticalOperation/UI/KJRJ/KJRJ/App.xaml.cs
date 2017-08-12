using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

//
// SketchFlow 需要知道哪个控件程序集包含其屏幕。这是在创建项目时
// 自动设置的，但是如果您手动更改控件程序集的名称，则也必须
// 在此处手动更新它。
//
[assembly: Microsoft.Expression.Prototyping.Services.SketchFlowLibraries("KJRJ.Screens")]

namespace KJRJ
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.App_Startup;
		}

		private void App_Startup(object sender, StartupEventArgs e)
		{
			this.StartupUri = new Uri(@"pack://application:,,,/Microsoft.Expression.Prototyping.Runtime;Component/WPF/Workspace/PlayerWindow.xaml");
		}
	}
}