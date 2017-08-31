using Business;
using Business.Processer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;

namespace AuditPracticalOperation.Controls
{
    /// <summary>
    /// MyAchievement.xaml 的交互逻辑
    /// </summary>
    public partial class MyAchievement : UserControl
    {
        private ObservableCollection<RankingItem> obsRankings;
        public MyAchievement()
        {
            InitializeComponent();
            this.Loaded += MyAchievement_Loaded;
        }

        void MyAchievement_Loaded(object sender, RoutedEventArgs e)
        {
            obsRankings = new ObservableCollection<RankingItem>();
            TextBlock_UserName.Text = SingletonManager.Get<UserProcesser>().GetUser().Name;
            TextBlock_UserID.Text = SingletonManager.Get<UserProcesser>().GetUser().ID;
            TextBlock_DoneCount.Text = SingletonManager.Get<PracticalItemProcesser>().GetHasDonePracticalItems().Count.ToString();
            int this_score = (int)SingletonManager.Get<PracticalItemProcesser>().GetHasDonePracticalItems().Count / SingletonManager.Get<PracticalItemProcesser>().GetPracticalItems().Count * 100;
            TextBlock_DonePercent.Text = this_score.ToString();

            int index = 0;
            if (this_score == 100)
            {
                index = 0;
            }
            else if (this_score >= 90)
            {
                index = 1;
            }
            else if (this_score >= 80)
            {
                index = 2;
            }
            else if (this_score >= 70)
            {
                index = 3;
            }
            else if (this_score >= 50)
            {
                index = 4;
            }
            else
            {
                index = 5;
            }

            RankingItem[] items = new RankingItem[6];
            RankingItem[] fixedItems = new RankingItem[5];
            items[0] = new RankingItem()
            {
                Score = 100,
                User_Name = "陈昊",
                ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\touxiang3.png",
            };
            items[1] = new RankingItem()
            {
                Score = 90,
                User_Name = "冯轲",
                ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\touxiang2.png",
            };
            items[2] = new RankingItem()
            {
                Score = 80,
                User_Name = "邓润如",
                ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\touxiang3.png",
            };
            items[3] = new RankingItem()
            {
                Score = 70,
                User_Name = "赵旭",
                ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\touxiang2.png",
            };
            items[4] = new RankingItem()
            {
                Score = 50,
                User_Name = "甘仁",
                ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\touxiang2.png",
            };
            items[5] = new RankingItem()
            {
                Score = this_score,
                User_Name = SingletonManager.Get<UserProcesser>().GetUser().Name,
                ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\touxiang1.png",
            };
            switch (index)
            {
                case 0:
                    obsRankings.Add(items[5]);
                    obsRankings.Add(items[0]);
                    obsRankings.Add(items[1]);
                    obsRankings.Add(items[2]);
                    obsRankings.Add(items[3]);
                    break;
                case 1:
                    obsRankings.Add(items[0]);
                    obsRankings.Add(items[5]);
                    obsRankings.Add(items[1]);
                    obsRankings.Add(items[2]);
                    obsRankings.Add(items[3]);
                    break;
                case 2:
                    obsRankings.Add(items[0]);
                    obsRankings.Add(items[1]);
                    obsRankings.Add(items[5]);
                    obsRankings.Add(items[2]);
                    obsRankings.Add(items[3]);
                    break;
                case 3:
                    obsRankings.Add(items[0]);
                    obsRankings.Add(items[1]);
                    obsRankings.Add(items[2]);
                    obsRankings.Add(items[5]);
                    obsRankings.Add(items[3]);
                    break;
                case 4:
                    obsRankings.Add(items[0]);
                    obsRankings.Add(items[1]);
                    obsRankings.Add(items[2]);
                    obsRankings.Add(items[3]);
                    obsRankings.Add(items[5]);
                    break;
                default:
                    obsRankings.Add(items[0]);
                    obsRankings.Add(items[1]);
                    obsRankings.Add(items[2]);
                    obsRankings.Add(items[3]);
                    obsRankings.Add(items[4]);
                    break;
            }
            ranking1.DataContext = obsRankings[0];
            ranking2.DataContext = obsRankings[1];
            ranking3.DataContext = obsRankings[2];
            ranking4.DataContext = obsRankings[3];
            ranking5.DataContext = obsRankings[4];
        }
    }
}
