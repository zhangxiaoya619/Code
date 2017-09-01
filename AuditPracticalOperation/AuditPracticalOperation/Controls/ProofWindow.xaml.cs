using AuditPracticalOperation.UContorls;
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
using ViewModel;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// ProofWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProofWindow : Window
    {
        private int selectIndex;
        private ProofItem proof;

        public ProofWindow(ProofItem proof, int selectIndex)
        {
            InitializeComponent();

            this.selectIndex = selectIndex;
            this.proof = proof;
            LoadControl(proof.Proofs[selectIndex]);
        }

        private void LoadControl(ProofItem file)
        {
            switch (file.Type)
            {
                case FileTypeEnum.Pdf:
                    PDFView pdf = new PDFView(file);
                    container.Content = pdf;
                    break;
                case FileTypeEnum.Img:
                    ImageView image = new ImageView(file.ImgSource);
                    container.Content = image;
                    break;
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                BtnMax_Click(null, null);
                return;
            }

            this.DragMove();
        }

        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else if (this.WindowState == WindowState.Maximized || this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            int old = selectIndex;

            if (selectIndex == 0)
                selectIndex = proof.Proofs.Count - 1;

            else
                selectIndex--;

            if (old == selectIndex)
                return;

            LoadControl(proof.Proofs[selectIndex]);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            int old = selectIndex;

            if (selectIndex == proof.Proofs.Count - 1)
                selectIndex = 0;

            else
                selectIndex++;

            if (old == selectIndex)
                return;

            LoadControl(proof.Proofs[selectIndex]);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (container.Content != null && container.Content is PDFView)
                ((PDFView)container.Content).PDFDispose();
        }
    }
}
