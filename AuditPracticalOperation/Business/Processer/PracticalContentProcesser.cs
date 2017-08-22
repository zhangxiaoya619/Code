using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    public class PracticalContentProcesser
    {
        private IPracticalManager practicalManager;
        private int practicalID;
        private int projectID;
        private string filePath;

        public string LoadContent()
        {
            filePath = practicalManager.LoadContentByPractialID(practicalID);
            return filePath;
        }

        public void SaveContent()
        {
            practicalManager.SaveContent(practicalID, filePath);
            practicalManager.SetPracticalProjectDone(practicalID, projectID);
        }

        public PracticalContentProcesser(int practicalID, int projectID)
        {
            this.practicalID = practicalID;
            this.projectID = projectID;
            this.practicalManager = SingletonManager.Get<PracticalManager>();
        }
    }
}
