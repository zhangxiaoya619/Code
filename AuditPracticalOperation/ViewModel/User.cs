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

        private string name;
        private string id;
        private string cpuId;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }

        public string CPUID
        {
            get
            {
                return cpuId;
            }

            set
            {
                cpuId = value;
                RaisePropertyChanged("CPUID");
            }
        }
    }
}
