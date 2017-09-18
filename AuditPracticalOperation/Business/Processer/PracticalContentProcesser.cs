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

        public void KillAllProcess()
        {
            Process tool = new Process();
            tool.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "handle.exe");
            tool.StartInfo.Arguments = filePath;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.StartInfo.CreateNoWindow = true;
            tool.StartInfo.UseShellExecute = false;
            tool.Start();
            tool.WaitForExit();
            string outputTool = tool.StandardOutput.ReadToEnd();

            string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            foreach (Match match in Regex.Matches(outputTool, matchPattern))
            {
                Process process = Process.GetProcessById(int.Parse(match.Value));
                process.Kill();
                Thread.Sleep(1000);
                break;
            }
        }
    }
}
