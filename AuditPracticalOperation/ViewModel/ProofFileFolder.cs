using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class ProofFileFolder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;

        private int parentIndex;

        private string name;

        private ObservableCollection<ProofFile> files;

        private ObservableCollection<ProofFileFolder> folders;

        public string Name { get { return name; } set { name = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }

        public ObservableCollection<ProofFile> Files { get { return files; } set { files = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Files")); } }

        public ObservableCollection<ProofFileFolder> Folders { get { return folders; } set { folders = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Folders")); } }

        public int Index { get { return index; } set { index = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Index")); } }

        public int ParentIndex { get { return parentIndex; } set { parentIndex = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ParentIndex")); } }

        public ProofFileFolder()
        {
            files = new ObservableCollection<ProofFile>();
            folders = new ObservableCollection<ProofFileFolder>();
        }
    }

    public class ProofFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string path;

        private string name;

        private FileTypeEnum fileType;

        public string Path { get { return path; } set { path = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Path")); } }

        public string Name { get { return name; } set { name = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Path")); } }

        public FileTypeEnum FileType { get { return fileType; } set { fileType = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FileType")); } }
    }
}
