using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class User : ViewModelBase, IPower
    {
        public void ChangePower(bool ispower)
        {
            _isPower = ispower;
        }
        public bool HasPower(int practicalItemID)
        {
            if (practicalItemID == 0 || practicalItemID == 1 || practicalItemID == 2)
                return true;
            return _isPower;
        }
        private bool _isPower = false;
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
