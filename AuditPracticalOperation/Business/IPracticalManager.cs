using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business
{
    internal interface IPracticalManager
    {
        string LoadContentByPractialID(int practicalId);

        void SaveContent(string filePath);

        void SetPracticalProjectDone(int practicalID, int projectID);

        PracticalItem[] GetAllPractical();
    }
}
