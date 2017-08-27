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
            header.Text = string.Format("{0}提示", helperType);
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
