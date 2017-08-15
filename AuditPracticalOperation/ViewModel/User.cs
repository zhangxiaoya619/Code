using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class User : ViewModelBase, IPower
    {
        public bool HasPower()
        {
            return true;
        }
    }
}
