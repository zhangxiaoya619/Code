using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class RankingItem : ViewModelBase
    {
        private string imageSource;
        private string user_name;
        private int score;
        public string ImgSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                if (value != imageSource)
                {
                    imageSource = value;
                    RaisePropertyChanged("ImgSource");
                }
            }
        }
        public string User_Name
        {
            get
            {
                return user_name;
            }
            set
            {
                if (user_name != value)
                {
                    user_name = value;
                    RaisePropertyChanged("User_Name");
                }
            }
        }
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if (score != value)
                {
                    score = value;
                    RaisePropertyChanged("Score");
                }
            }
        }
    }

}
