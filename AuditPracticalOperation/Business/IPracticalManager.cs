using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business
{
    internal interface IPracticalManager
    {
        string LoadContentByPractialID(int practicalID, IList<Autograph> autographs);

        void SaveContent(int practicalID, string filePath, IList<Autograph> autographs);

        void SetPracticalProjectDone(int practicalID, int projectID);

        void ExportPractical(string fileName, HasDonePracticalItem[] hasDonePracticalItems);

        PracticalItem[] GetAllPractical();

        HasDonePracticalItem[] GetHasDonePractical();

        void DeleteContent(string filePath);
    }
}
