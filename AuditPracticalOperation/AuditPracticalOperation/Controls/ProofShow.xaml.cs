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
    public partial class ProofShow : UserControl
    {
        private bool isFirstLoad = true;
        private IList<ViewModel.ProofItem> datasource;
        private ViewModel.ProofItem CurrentProofDir = null;
        private ObservableCollection<ViewModel.ProofItem> queueDir = null;
        public ProofShow()
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
            xunhuanList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = datasource });
            dirList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = queueDir });
            proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
        }

        private void xunhuan_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ProofItem rootItem = (sender as RadioButton).Tag as ViewModel.ProofItem;
            string root = rootItem.Path;
            if (!CurrentProofDir.Path.Contains(root))
            {
                queueDir.Clear();
                queueDir.Add(rootItem);
                CurrentProofDir = rootItem;
                proofList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = CurrentProofDir.Proofs });
            }
        }

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
                        string pdffileName = choiseItem.Path;
                        GridPdfShow.Visibility = System.Windows.Visibility.Visible;
                        PDFReader pdfCtl = new PDFReader();
                        pdfCtl.AdobeReaderControl(pdffileName);
                        winFormHost.Child = pdfCtl;
                        break;
                    case ViewModel.FileTypeEnum.Img:
                        string fileName = choiseItem.Path;
                        BitmapImage image = new BitmapImage(new Uri(fileName));
                        CurrentImage.Source = image;
                        GridImageShow.Visibility = System.Windows.Visibility.Visible;
                        imageTitle.Text = choiseItem.Name;
                        break;
                    default:
                        break;
                }
            }
            originalX = tlt.X;
            originalY = tlt.Y;
        }

        private void ChoiseDir_Checked(object sender, RoutedEventArgs e)
        {
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
        private void ImageShow_Click(object sender, RoutedEventArgs e)
        {
            GridImageShow.Visibility = System.Windows.Visibility.Collapsed;
        }





        private bool isMouseLeftButtonDown = false;
        Point previousMousePoint = new Point(0, 0);
        double originalX = 0, originalY = 0;
        private void CurrentImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //image缩放
            Point centerPoint = e.GetPosition(CurrentImage);
            Image image = sender as Image;
            Point point = e.GetPosition(image);
            double scale = e.Delta * 0.001;
            ZoomImage(point, scale);
        }
        private void ZoomImage(Point point, double scale)
        {
            TransformGroup group = ImageTransformGroup;

            Point pointToContent = group.Inverse.Transform(point);

            ScaleTransform transform = group.Children[0] as ScaleTransform;

            if (transform.ScaleX + scale <= 1)
            {
                transform.ScaleX = 1;
            }
            if (transform.ScaleY + scale <= 1)
            {
                transform.ScaleY = 1;
            }
            if (sfr.ScaleX <= 1 && sfr.ScaleY <= 1 && scale < 0)
            {
                tlt.X = originalX;
                tlt.Y = originalY;
                return;
            }
            transform.ScaleX += scale;

            transform.ScaleY += scale;

            TranslateTransform transform1 = group.Children[1] as TranslateTransform;

            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);

            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);

        }
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = true;
            previousMousePoint = e.GetPosition(CurrentImage);
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = false;
            if (sfr.ScaleX <= 1)
            {
                tlt.X = originalX;
                tlt.Y = originalY;
            }
        }

        private void img_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseLeftButtonDown = false;
            if (sfr.ScaleX <= 1)
            {
                tlt.X = originalX;
                tlt.Y = originalY;
            }
        }

        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown == true)
            {
                Point position = e.GetPosition(CurrentImage);
                tlt.X += position.X - this.previousMousePoint.X;
                tlt.Y += position.Y - this.previousMousePoint.Y;
            }
        }

        private void RotateTransform_Click(object sender, RoutedEventArgs e)
        {
            rotate.Angle += 90;
            sfr.ScaleX = 1;
            sfr.ScaleY = 1;
            tlt.X = originalX;
            tlt.Y = originalY;
        }

        private void pdfShow_Click(object sender, RoutedEventArgs e)
        {
            GridPdfShow.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
