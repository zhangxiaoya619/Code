using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class PracticalItemProject : ViewModelBase, IHelper
    {
        private int id;
        private string name;
        private bool isDone;

        public int ID
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

        public string HelperText { get; set; }

        public bool IsNeedShowHelper { get { return !string.IsNullOrEmpty(HelperText.Trim()); } }

        public string Title { get { return "差异说明"; } }
    }
}
