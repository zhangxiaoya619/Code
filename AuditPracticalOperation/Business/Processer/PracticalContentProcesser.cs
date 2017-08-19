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
        private PracticalItemProject project;
        private string filePath;

        public string LoadContent()
        {
            filePath = contentManager.LoadContentByPractialID(project.ID);
            return filePath;
        }

        public void SaveContent()
        {
            contentManager.SaveContent(filePath);
        }

        public PracticalContentProcesser(PracticalItemProject project)
        {
            this.project = project;
            this.contentManager = SingletonManager.Get<PracticalManager>();
        }
    }
}
