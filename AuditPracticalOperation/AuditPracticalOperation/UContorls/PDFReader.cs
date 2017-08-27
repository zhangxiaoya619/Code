using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuditPracticalOperation.UContorls
{
    public partial class PDFReader : UserControl
    {
        public PDFReader()
        {
            InitializeComponent();
        }
        public void AdobeReaderControl(string fileName)
        {
            this.axAcroPDF1.LoadFile(fileName);
        }
    }
}
