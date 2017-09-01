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

        void ExportPractical(string fileName, HasDonePracticalItem[] hasDonePracticalItems);

        PracticalItem[] GetAllPractical();

        HasDonePracticalItem[] GetHasDonePractical();

        void DeleteContent(string filePath);
    }
}
