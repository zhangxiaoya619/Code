using Common.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PracticalImportTools
{
    /// <summary>
    /// PracticalFileAutograph.xaml 的交互逻辑
    /// </summary>
    public partial class PracticalFileAutograph : Window
    {
        public PracticalFileAutograph(PracticalFile practicalFile)
        {
            InitializeComponent();

            if (!this.IsInDesignMode())
                container.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = practicalFile });
        }
    }

    public class AutographConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<Autograph> autographs = (IList<Autograph>)value;
            string text = string.Empty;

            for (int i = 0; i < autographs.Count; i++)
                text += string.Format("{0}.{1}.{2},", autographs[i].SheetName, autographs[i].RowIndex + 1, ToName(autographs[i].ColIndex));

            if (!string.IsNullOrEmpty(text))
                text = text.Substring(0, text.Length - 1);

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] texts = ((string)value).Trim().Split(',');
            IList<Autograph> autographs = new ObservableCollection<Autograph>();

            for (int i = 0; i < texts.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(texts[i]))
                    continue;

                string[] tmp = texts[i].Trim().Split('.');
                Autograph autograph = new Autograph();
                autograph.SheetName = tmp[0];
                autograph.RowIndex = System.Convert.ToInt32(tmp[1]) - 1;
                autograph.ColIndex = ToIndex(tmp[2]);
                autographs.Add(autograph);
            }

            return autographs;
        }

        private static int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+")) { throw new Exception("invalid parameter"); }

            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }

        private static string ToName(int index)
        {
            if (index < 0) { throw new Exception("invalid parameter"); }

            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return String.Join(string.Empty, chars.ToArray());
        }
    }

}
