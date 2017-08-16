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
            for (int i = 1; i <= 24; i++)
            {
                PracticalItem item = new PracticalItem();
                item.ID = i.ToString();
                item.Name = "实操项目" + i;
                item.IsDone = i % 2 == 0;
                for (int k = 1; k < 10; k++)
                {
                    PracticalItemProject project = new PracticalItemProject();
                    project.ID = i.ToString() + k.ToString();
                    project.Name = "项目" + k;
                    project.IsDone = k % 2 == 0;
                    item.Projects.Add(project);
                }
                items.Add(item);
            }
            return items;
        }

        private PracticalItemProcesser()
        {

        }
    }
}
