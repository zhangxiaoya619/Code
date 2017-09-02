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
    public partial class ProofControl : UserControl
    {
        private int selectIndex;
        private ProofItem proof;

        public event Action<ProofControl> OnClose;

        public ProofControl(ProofItem proof, int selectIndex)
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (container.Content != null && container.Content is PDFView)
                ((PDFView)container.Content).PDFDispose();

            if (OnClose != null)
                OnClose(this);
        }

        public void BtnLast_Click(object sender, RoutedEventArgs e)
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

        public void BtnNext_Click(object sender, RoutedEventArgs e)
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
    }
}
