﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    [Singleton]
    public class PracticalItemProcesser
    {
        private IPracticalManager practicalManager;

        public IList<PracticalItem> GetPracticalItems()
        {
            return practicalManager.GetAllPractical();
        }

        public IList<HasDonePracticalItem> GetHasDonePracticalItems()
        {
            return practicalManager.GetHasDonePractical();
        }

        private PracticalItemProcesser()
        {
            practicalManager = SingletonManager.Get<PracticalManager>();
        }
    }
}
