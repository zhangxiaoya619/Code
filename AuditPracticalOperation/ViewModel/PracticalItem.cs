using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class PracticalItem : ViewModelBase
    {
        private string name;
        private int id;
        private bool isDone;
        private ObservableCollection<PracticalItemProject> projects;

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

        public ObservableCollection<PracticalItemProject> Projects
        {
            get
            {
                return projects;
            }

            set
            {
                projects = value;
                RaisePropertyChanged("Projects");
            }
        }

        public PracticalItem()
        {
            Projects = new ObservableCollection<PracticalItemProject>();
        }
    }
}
