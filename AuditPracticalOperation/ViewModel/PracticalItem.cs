using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class PracticalItem: ViewModelBase
    {
        private string name;
        private string id;
        private bool isDone;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
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
                RaisePropertyChanged(nameof(ID));
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
                RaisePropertyChanged(nameof(IsDone));
            }
        }
    }
}
