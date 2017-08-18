using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Processer
{
    [Singleton]
    public class PracticalContentManager : IPracticalContentManager
    {
        public string LoadContentByPractialID(string id)
        {
            return null;
        }

        public void SaveContent(string filePath)
        {

        }

        private PracticalContentManager()
        {

        }
    }
}
