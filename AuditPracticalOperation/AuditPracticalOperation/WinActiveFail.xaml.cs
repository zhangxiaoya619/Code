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
using System.Windows.Shapes;

namespace AuditPracticalOperation
{
    /// <summary>
    /// WinActiveFail.xaml 的交互逻辑
    /// </summary>
    public partial class WinActiveFail : Window
    {
        public WinActiveFail()
        {
            InitializeComponent();
            this.Loaded += WinActiveSuccess_Loaded;
        }

        void WinActiveSuccess_Loaded(object sender, RoutedEventArgs e)
        {
            thclose = new System.Threading.Thread(time);
            thclose.Start();
        }
        System.Threading.Thread thclose;
        void CloseWindow()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.Close();
            });
        }
        void time()
        {
            System.Threading.Thread.Sleep(1000);
            CloseWindow();
        }
    }
}
