using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public interface IHelper
    {
        string HelperText { get; set; }
        bool IsNeedShowHelper { get; }
        string Title { get; }
    }
}
