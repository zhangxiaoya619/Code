using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    [Singleton]
    public class PracticalItemProcesser
    {
        public IList<PracticalItem> GetPracticalItems()
        {
            List<PracticalItem> items = new List<PracticalItem>();
            for(int i = 1; i <= 24; i++)
            {
                PracticalItem item = new PracticalItem();
                item.ID = i.ToString();
                item.Name = "实操项目" + i;
                item.IsDone = Convert.ToBoolean(i % 2);
                items.Add(item);
            }
            return items;
        }

        private PracticalItemProcesser()
        {

        }
    }
}
