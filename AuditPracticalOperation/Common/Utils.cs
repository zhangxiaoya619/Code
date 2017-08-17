using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Common.Utils
{
    public static class Utils
    {
        public static bool IsInDesignMode(this Control control)
        {
            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(control);
        }

        public static bool IsInDesignMode()
        {
            return Application.Current.MainWindow == null;
        }
    }
}
