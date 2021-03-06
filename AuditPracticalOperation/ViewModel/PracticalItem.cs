﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ViewModel
{
    public class PracticalItem : ViewModelBase, IHelper
    {
        private string name;
        private int id;
        private PracticalStateEnum state;
        private ObservableCollection<PracticalItemProject> projects;

        public string Name
        {
            get
            {
                return Regex.Replace(name, @"\d", "").Replace(".", "");
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

        public PracticalStateEnum State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                RaisePropertyChanged("State");
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

        public ObservableCollection<Autograph> Autographs { get; set; }

        public string HelperText { get; set; }

        public bool IsNeedShowHelper { get { return !string.IsNullOrEmpty(HelperText.Trim()); } }

        public string Title { get { return "差异说明"; } }

        public void UpdateState()
        {
            if (Projects.Count(item => item.IsDone) == 0)
                State = PracticalStateEnum.Default;
            else if (Projects.Count(item => item.IsDone) < Projects.Count)
                State = PracticalStateEnum.OnWorking;
            else
                State = PracticalStateEnum.HasDone;

            RaisePropertyChanged("Projects");
        }

        public PracticalItem()
        {
            Projects = new ObservableCollection<PracticalItemProject>();
            Autographs = new ObservableCollection<Autograph>();
        }
    }

    public enum PracticalStateEnum
    {
        Default,
        OnWorking,
        HasDone,
    }

}
