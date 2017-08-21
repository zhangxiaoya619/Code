using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    public class PracticalContentProcesser
    {
        private IPracticalManager contentManager;
        private int practicalID;
        private int projectID;
        private string filePath;

        public string LoadContent()
        {
            filePath = contentManager.LoadContentByPractialID(practicalID);
            return filePath;
        }

        public void SaveContent()
        {
            contentManager.SaveContent(filePath);
            contentManager.SetPracticalProjectDone(practicalID, projectID);
        }

        public PracticalContentProcesser(int practicalID, int projectID)
        {
            this.practicalID = practicalID;
            this.projectID = projectID;
            this.contentManager = SingletonManager.Get<PracticalManager>();
        }
    }
}
