using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class ProofItem : ViewModelBase
    {
        private string name;
        private string path;
        private FileTypeEnum type;
        private ObservableCollection<ProofItem> proofs;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }
        public FileTypeEnum Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    RaisePropertyChanged("Type");
                }
            }
        }
        public ObservableCollection<ProofItem> Proofs
        {
            get
            {
                return proofs;
            }
            set
            {
                if (proofs != value)
                {
                    proofs = value;
                    RaisePropertyChanged("Proofs");
                }
            }
        }
        public ProofItem()
        {
            proofs = new ObservableCollection<ProofItem>();
        }
    }
    public enum FileTypeEnum
    {
        Directory,
        Word,
        Excel,
        Pdf,
        Img,
    }
}
