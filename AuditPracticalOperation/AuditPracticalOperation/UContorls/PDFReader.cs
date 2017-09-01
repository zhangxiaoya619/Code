using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ViewModel;
using Business.Processer;
using Business;

namespace AuditPracticalOperation.UContorls
{
    public partial class PDFReader : UserControl
    {
        private string tempFilePath;

        public PDFReader()
        {
            InitializeComponent();
            this.axAcroPDF1.Disposed += AxAcroPDF1_Disposed;
        }

        private void AxAcroPDF1_Disposed(object sender, EventArgs e)
        {
            SingletonManager.Get<ProofShowProcessor>().DeleteProofTempFile(tempFilePath);
        }

        public void AdobeReaderControl(ProofItem choiseItem)
        {
            tempFilePath = SingletonManager.Get<ProofShowProcessor>().GetProofTempFilePath(choiseItem);
            this.axAcroPDF1.LoadFile(tempFilePath);
        }
    }
}
