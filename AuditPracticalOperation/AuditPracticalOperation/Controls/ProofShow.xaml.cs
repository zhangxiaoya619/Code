using AuditPracticalOperation.UContorls;
using Business;
using Business.Processer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// ProofShow.xaml 的交互 逻辑
    /// </summary>
    [Singleton]
    public partial class ProofShow : UserControl
    {
        private bool isFirstLoad = true;
        private IList<ViewModel.ProofItem> datasource;
        private ViewModel.ProofItem CurrentProofDir = null;
        private ObservableCollection<ViewModel.ProofItem> queueDir = null;
        private ProofShow()
        {
            InitializeComponent();
            this.Loaded += ProofShow_Loaded;
        }

        private void ProofShow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isFirstLoad)
                return;
            isFirstLoad = false;
            queueDir = new ObservableCollection<ViewModel.ProofItem>();
            datasource = SingletonManager.Get<ProofShowProcessor>().GetProofItems();
            queueDir.Add(datasource[0]);
            CurrentProofDir = datasource[0];
            CurrentProofDir.IsChecked = true;
            xunhuanList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = datasource });
            dirList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = queueDir });
            proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
        }

        private void xunhuan_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ProofItem rootItem = (sender as RadioButton).Tag as ViewModel.ProofItem;
            int index = datasource.IndexOf(rootItem);
            if (SingletonManager.Get<UserProcesser>().GetUser().HasProofPower(index))
            {
                if (!CurrentProofDir.Contains(rootItem))
                {
                    if (container.Content != null && container.Content is ProofControl)
                        ((ProofControl)container.Content).Close();

                    queueDir.Clear();
                    queueDir.Add(rootItem);
                    CurrentProofDir = rootItem;
                    proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
                }
            }
            else
            {
                WinBuyActivationCode winCode = new WinBuyActivationCode();
                winCode.ShowDialog();
            }
        }

        private void ChoiseDir_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ProofItem choiseItem = (sender as RadioButton).Tag as ViewModel.ProofItem;
            if (choiseItem != CurrentProofDir)
            {
                CurrentProofDir = choiseItem;

                int index = queueDir.IndexOf(CurrentProofDir);

                while (index != queueDir.Count - 1)
                    queueDir.RemoveAt(queueDir.Count - 1);

                proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
            }
        }


        private ViewModel.ProofItem currentShowImgItem = null;
        private void ShowImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ProofItem choiseItem = (sender as Image).Tag as ViewModel.ProofItem;
            if (e.ClickCount == 2)
            {
                switch (choiseItem.Type)
                {
                    case ViewModel.FileTypeEnum.Directory:
                        CurrentProofDir = choiseItem;
                        queueDir.Add(choiseItem);
                        proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
                        break;
                    case ViewModel.FileTypeEnum.Word:
                        break;
                    case ViewModel.FileTypeEnum.Excel:
                        break;
                    case ViewModel.FileTypeEnum.Pdf:
                    case ViewModel.FileTypeEnum.Img:
                        currentShowImgItem = choiseItem;
                        ProofControl proofControl = new ProofControl(CurrentProofDir, CurrentProofDir.Proofs.IndexOf(choiseItem));
                        proofControl.OnClose += ProofControl_OnClose;
                        container.Content = proofControl;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProofControl_OnClose(ProofControl proofControl)
        {
            proofControl.OnClose -= ProofControl_OnClose;
            container.Content = null;
        }
    }
}
