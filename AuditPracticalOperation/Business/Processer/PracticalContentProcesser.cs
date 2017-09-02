using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    public class PracticalContentProcesser
    {
        private IPracticalManager practicalManager;
        private int practicalID;
        private string filePath;
        private PracticalItemProject project;

        public string LoadContent()
        {
            filePath = practicalManager.LoadContentByPractialID(practicalID);
            return filePath;
        }

        public void SaveContent()
        {
            practicalManager.SaveContent(practicalID, filePath);
            practicalManager.SetPracticalProjectDone(practicalID, project.ID);
            project.IsDone = true;
        }

        public void DeleteContent()
        {
            practicalManager.DeleteContent(filePath);
        }

        public PracticalContentProcesser(int practicalID, PracticalItemProject project)
        {
            this.practicalID = practicalID;
            this.project = project;
            this.practicalManager = SingletonManager.Get<PracticalManager>();
        }

        public static void KillAllProcess()
        {
            while (Process.GetProcessesByName("EXCEL").Count(process => process.ProcessName == "EXCEL") > 0)
            {
                foreach (Process IsProcedding in Process.GetProcessesByName("EXCEL"))
                    if (IsProcedding.ProcessName == "EXCEL")
                        if (!IsProcedding.HasExited)
                            IsProcedding.Kill();
            }
        }
    }
}
