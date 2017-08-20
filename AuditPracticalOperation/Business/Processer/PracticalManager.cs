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

        public string LoadContentByPractialID(int id)
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

            FileStream fs = null;
            byte[] buffer = null;
            PracticalItem[] items = null;
            int[] practicalProjectCountArray = null;
            int[][] practicalProjectContentBufferLengthArray = null;
            int[] practicalNameBufferLength = null;
            long[] practicalContentBufferLength = null;

            try
            {
                fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                items = new PracticalItem[BitConverter.ToInt32(buffer, 0)];
                practicalProjectCountArray = new int[items.Length];
                practicalProjectContentBufferLengthArray = new int[items.Length][];
                practicalNameBufferLength = new int[items.Length];
                practicalContentBufferLength = new long[items.Length];

                for(int i = 0;i< items.Length;i++)
                    items[i] = new PracticalItem();

                for (int i = 0; i < items.Length; i++)
                {
                    buffer = new byte[sizeof(int)];
                    fs.Read(buffer, 0, buffer.Length);
                    practicalProjectCountArray[i] = BitConverter.ToInt32(buffer, 0);
                    practicalProjectContentBufferLengthArray[i] = new int[practicalProjectCountArray[i]];
                }
                for (int i = 0; i < items.Length; i++)
                {
                    for (int j = 0; j < practicalProjectCountArray[i]; j++)
                    {
                        buffer = new byte[sizeof(int)];
                        fs.Read(buffer, 0, buffer.Length);
                        practicalProjectContentBufferLengthArray[i][j] = BitConverter.ToInt32(buffer, 0);
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    buffer = new byte[sizeof(int)];
                    fs.Read(buffer, 0, buffer.Length);
                    practicalNameBufferLength[i] = BitConverter.ToInt32(buffer, 0);
                }
                for (int i = 0; i < items.Length; i++)
                {
                    buffer = new byte[sizeof(long)];
                    fs.Read(buffer, 0, buffer.Length);
                    practicalContentBufferLength[i] = BitConverter.ToInt64(buffer, 0);
                }
                for (int i = 0; i < items.Length; i++)
                {
                    for (int j = 0; j < practicalProjectCountArray[i]; j++)
                    {
                        buffer = new byte[practicalProjectContentBufferLengthArray[i][j]];
                        fs.Read(buffer, 0, buffer.Length);
                        PracticalItemProject project = new PracticalItemProject();
                        project.ID = j;
                        project.Name = UTF8Encoding.UTF8.GetString(buffer);
                        items[i].Projects.Add(project);
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    buffer = new byte[practicalNameBufferLength[i]];
                    fs.Read(buffer, 0, buffer.Length);
                    items[i].ID = i;
                    items[i].Name = UTF8Encoding.UTF8.GetString(buffer);
                }
            }
            catch
            {
                throw new Exception("题库加载错误。");
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
            return items;
        }

        private PracticalManager()
        {
            templateFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMPLATE_FILE_NAME);
            practicalFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PRACTICAL_FILE_FOLDER);
        }
    }
}
