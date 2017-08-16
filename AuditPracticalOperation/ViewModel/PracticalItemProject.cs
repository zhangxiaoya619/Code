using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class PracticalItemProject : ViewModelBase
    {
        private string id;
        private string name;
        private bool isDone;

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

        public bool IsDone
        {
            get
            {
                return isDone;
            }

            set
            {
                isDone = value;
                RaisePropertyChanged("IsDone");
            }
        }
    }
}
