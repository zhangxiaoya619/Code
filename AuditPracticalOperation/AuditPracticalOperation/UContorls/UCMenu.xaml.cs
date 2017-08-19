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
    /// UCMenu.xaml 的交互逻辑
    /// </summary>
    public partial class UCMenu : UserControl
    {
        public UCMenu()
        {
            InitializeComponent();
            this.Loaded += UCMenu_Loaded;
        }

        void UCMenu_Loaded(object sender, RoutedEventArgs e)
        {
            DefaultMenu.IsChecked = true;
        }
        public event UserLogoutHandler OnUserLogout;
        public event UserClickMenuHandler OnUserClickMenu;

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            if (OnUserLogout != null)
                OnUserLogout();
        }

        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserChooseMenu(object sender, RoutedEventArgs e)
        {
            if (OnUserClickMenu != null)
            {
                OnUserClickMenu(int.Parse((sender as RadioButton).Tag.ToString()));
            }
        }
    }
    public delegate void UserLogoutHandler();
    public delegate void UserClickMenuHandler(int index);
}
