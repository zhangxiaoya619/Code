using AuditPracticalOperation.Controls;
using Business;
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

namespace AuditPracticalOperation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Menu.OnUserLogout += Menu_OnUserLogout;
            Menu.OnUserClickMenu += Menu_OnUserClickMenu;
            SetUserName(SingletonManager.Get<UserProcesser>().GetUser().Name);
        }
        private void SetUserName(string name)
        {
            header.SetUserName(name);
        }
        void Menu_OnUserClickMenu(int index)
        {
            switch (index)
            {
                case 0:
                    myMainChild.Child = new SubjectList();
                    break;
                case 1:
                    myMainChild.Child = new ProofShow();
                    break;
                case 2:
                    myMainChild.Child = new GainExport();
                    break;
                case 3:
                    break;
                case 4:
                    myMainChild.Child = new UserCenter();
                    break;
                default:
                    break;
            }
        }

        void Menu_OnUserLogout()
        {
            this.Close();
        }
    }
}
