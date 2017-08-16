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

namespace AuditPracticalOperation.UContorls
{
    /// <summary>
    /// UCLoginHeader.xaml 的交互逻辑
    /// </summary>
    public partial class UCLoginHeader : UserControl
    {
        public event UserClickHandler OnClose;
        public UCLoginHeader()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (OnClose != null)
                OnClose();
        }
    }
    public delegate void UserClickHandler();
}
