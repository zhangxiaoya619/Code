using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business
{
    internal interface IPracticalManager
    {
        string LoadContentByPractialID(int id);

        void SaveContent(string filePath);

        PracticalItem[] GetAllPractical();
    }
}
