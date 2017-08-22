using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business
{
    internal interface IPracticalManager
    {
        string LoadContentByPractialID(int practicalID);

        void SaveContent(int practicalID, string filePath);

        void SetPracticalProjectDone(int practicalID, int projectID);

        PracticalItem[] GetAllPractical();

        HasDonePracticalItem[] GetHasDonePractical();
    }
}
