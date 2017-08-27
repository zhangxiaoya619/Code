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
    /// UCHeader.xaml 的交互逻辑
    /// </summary>
    public partial class UCHeader : UserControl
    {
        public UCHeader()
        {
            InitializeComponent();
        }
        public void SetUserName(string name)
        {
            this.UserName.Text = name;
        }
    }
}
