using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class Config : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OfficeTypeEnum officeType;

        public OfficeTypeEnum OfficeType { get { return officeType; } set { officeType = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("OfficeType")); } }
    }

    public enum OfficeTypeEnum
    {
        [OfficeType("兼容模式")]
        CompatibilityMode,
        [OfficeType("正常模式")]
        NormalMode,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OfficeTypeAttribute : Attribute
    {
        public string Text { get; private set; }

        public OfficeTypeAttribute(string text)
        {
            Text = text;
        }
    }
}
