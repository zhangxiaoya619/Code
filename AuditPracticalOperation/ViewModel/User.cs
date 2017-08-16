using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class User : ViewModelBase, IPower
    {
        public bool HasPower(string practicalItemID)
        {
            if (Convert.ToInt32(practicalItemID) % 2 == 0)
                return true;
            else
                return false;
        }
    }
}
