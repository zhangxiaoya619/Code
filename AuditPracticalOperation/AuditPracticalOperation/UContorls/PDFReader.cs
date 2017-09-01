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
            this.axAcroPDF1.setPageMode("thumbs");
            this.axAcroPDF1.setPageMode("none");
        }

        public void PDFDispose()
        {
            SingletonManager.Get<ProofShowProcessor>().DeleteProofTempFile(tempFilePath);
            this.axAcroPDF1.Dispose();
        }

        public void AdobeReaderControl(ProofItem choiseItem)
        {
            tempFilePath = SingletonManager.Get<ProofShowProcessor>().GetProofTempFilePath(choiseItem);
            this.axAcroPDF1.LoadFile(tempFilePath);
        }
    }
}
