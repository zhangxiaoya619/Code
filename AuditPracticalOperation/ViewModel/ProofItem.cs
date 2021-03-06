﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class ProofItem : ViewModelBase
    {
        private string name;
        private byte[] _imageSource = null;
        private FileTypeEnum type;
        private bool isChecked;
        private ObservableCollection<ProofItem> proofs;

        public ProofItem Parent { get; set; }

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
        public byte[] ImgSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                if (value != _imageSource)
                {
                    _imageSource = value;
                    RaisePropertyChanged("ImgSource");
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

        public long StartIndex { get; set; }
        public long Length { get; set; }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    RaisePropertyChanged("IsChecked");
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

        public bool Contains(ProofItem child)
        {
            if (child.Parent == null)
                return false;
            else if (child.Parent == this)
                return true;
            else
                return Contains(child.Parent);
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
