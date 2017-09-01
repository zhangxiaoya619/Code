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
            imageView.OnClose += imageView_OnClose;
        }

        void imageView_OnClose()
        {
            GridImageShow.Visibility = System.Windows.Visibility.Collapsed;
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
            xunhuanList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = datasource });
            dirList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = queueDir });
            proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
        }

        private void xunhuan_Checked(object sender, RoutedEventArgs e)
        {
            this.GridImageShow.Visibility = System.Windows.Visibility.Collapsed;
            this.GridPdfShow.Visibility = System.Windows.Visibility.Collapsed;
            ViewModel.ProofItem rootItem = (sender as RadioButton).Tag as ViewModel.ProofItem;
            int index = datasource.IndexOf(rootItem);
            if (SingletonManager.Get<UserProcesser>().GetUser().HasProofPower(index))
            {
                string root = rootItem.Path;
                if (!CurrentProofDir.Path.Contains(root))
                {
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
            this.GridImageShow.Visibility = System.Windows.Visibility.Collapsed;
            this.GridPdfShow.Visibility = System.Windows.Visibility.Collapsed;
            ViewModel.ProofItem choiseItem = (sender as RadioButton).Tag as ViewModel.ProofItem;
            if (choiseItem != CurrentProofDir)
            {
                CurrentProofDir = choiseItem;
                for (int i = queueDir.ToArray().Length - 1; i >= 0; i--)
                {
                    if (queueDir[i].Path.Contains(CurrentProofDir.Path) && queueDir[i] != CurrentProofDir)
                        queueDir.RemoveAt(i);
                }
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
                        GridPdfShow.Visibility = System.Windows.Visibility.Visible;
                        PDFReader pdfCtl = new PDFReader();
                        pdfCtl.AdobeReaderControl(choiseItem);
                        winFormHost.Child = pdfCtl;
                        break;
                    case ViewModel.FileTypeEnum.Img:
                        GridImageShow.Visibility = System.Windows.Visibility.Visible;
                        currentShowImgItem = choiseItem;
                        imageView.LoadImage(choiseItem.ImgSource);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ImageShow_Click(object sender, RoutedEventArgs e)
        {
            GridImageShow.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void pdfShow_Click(object sender, RoutedEventArgs e)
        {
            GridPdfShow.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (GridImageShow.Visibility == System.Windows.Visibility.Visible)
            {
                int index = CurrentProofDir.Proofs.IndexOf(currentShowImgItem);
                if (index < 0)
                    return;
                if (e.Key == Key.Left)
                {
                    if (index > 0)
                    {
                        index -= 1;
                        for (int i = index; i >= 0; i--)
                        {
                            if (CurrentProofDir.Proofs[i].Type == ViewModel.FileTypeEnum.Img)
                            {
                                GridImageShow.Visibility = System.Windows.Visibility.Collapsed;
                                currentShowImgItem = CurrentProofDir.Proofs[i];
                                imageView.LoadImage(CurrentProofDir.Proofs[i].ImgSource);
                                GridImageShow.Visibility = System.Windows.Visibility.Visible;
                                break;
                            }
                        }
                    }
                }
                else if (e.Key == Key.Right)
                {
                    if (index < CurrentProofDir.Proofs.Count - 1)
                    {
                        index += 1;
                        for (int i = index; i < CurrentProofDir.Proofs.Count; i++)
                        {
                            if (CurrentProofDir.Proofs[i].Type == ViewModel.FileTypeEnum.Img)
                            {
                                currentShowImgItem = CurrentProofDir.Proofs[i];
                                imageView.LoadImage(CurrentProofDir.Proofs[i].ImgSource);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
