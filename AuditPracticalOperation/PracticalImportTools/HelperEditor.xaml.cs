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

namespace PracticalImportTools
{
    /// <summary>
    /// HelperEditor.xaml 的交互逻辑
    /// </summary>
    public partial class HelperEditor : Window
    {
        private HelperTypeEnum helperType;
        private IHelper helper;

        public HelperEditor(IHelper helper, HelperTypeEnum helperType)
        {
            this.helperType = helperType;
            this.helper = helper;

            InitializeComponent();

            editor.AppendText(helper.HelperText);

            title.Text = helper.Title;

            switch (helperType)
            {
                case HelperTypeEnum.F1: header.Text = "差异说明"; break;
                case HelperTypeEnum.F2: header.Text = "实操指引"; break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TextRange textRange = new TextRange(editor.Document.ContentStart, editor.Document.ContentEnd);
            helper.HelperText = textRange.Text;
        }
    }

    public enum HelperTypeEnum
    {
        F1,
        F2,
    }
}
