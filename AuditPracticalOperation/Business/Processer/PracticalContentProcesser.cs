using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ViewModel;

namespace Business.Processer
{
    public class PracticalContentProcesser
    {
        private IPracticalManager practicalManager;
        private int practicalID;
        private string filePath;
        private PracticalItemProject project;
        private IList<Autograph> autographs;

        public string LoadContent()
        {
            filePath = practicalManager.LoadContentByPractialID(practicalID, autographs);
            return filePath;
        }

        public void SaveContent()
        {
            practicalManager.SaveContent(practicalID, filePath, autographs);
            practicalManager.SetPracticalProjectDone(practicalID, project.ID);
            project.IsDone = true;
        }

        public void DeleteContent()
        {
            practicalManager.DeleteContent(filePath);
        }

        public PracticalContentProcesser(int practicalID, IList<Autograph> autographs, PracticalItemProject project)
        {
            this.practicalID = practicalID;
            this.project = project;
            this.autographs = autographs;
            this.practicalManager = SingletonManager.Get<PracticalManager>();
        }

        public void KillAllProcess()
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
