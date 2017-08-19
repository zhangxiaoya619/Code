using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;
using System.IO;

namespace Business.Processer
{
    [Singleton]
    public class PracticalManager : IPracticalManager
    {
        private const string TEMPLATE_FILE_NAME = "resource.sub";
        private readonly string templateFileName;
        private const string PRACTICAL_FILE_FOLDER = "prac";
        private readonly string practicalFileFolder;

        public string LoadContentByPractialID(string id)
        {
            return null;
        }

        public void SaveContent(string filePath)
        {

        }

        public PracticalItem[] GetAllPractical()
        {
            if (!File.Exists(templateFileName))
                throw new Exception("题库文件丢失。");

            //PracticalItem[] items = new PracticalItem[];
            return null;
        }

        private PracticalManager()
        {
            templateFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMPLATE_FILE_NAME);
            practicalFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PRACTICAL_FILE_FOLDER);
        }
    }
}
