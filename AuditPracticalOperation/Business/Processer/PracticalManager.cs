using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;
using System.IO;
using System.Xml;

namespace Business.Processer
{
    [Singleton]
    public class PracticalManager : IPracticalManager
    {
        private const string TEMPLATE_FILE_NAME = "resource.sub";
        private readonly string templateFileName;
        private const string PRACTICAL_FILE_FOLDER = "prac";
        private readonly string practicalFileName;
        private const string PRACTICAL_PROCESS_FILE_NAME = "process.xml";
        private readonly string practicalProcessFileName;
        private const int BUFFER_PACKAGE_LENGTH = 1000;
        private long startIndex;
        private int practicalCount;
        private long[] practicalContentBufferLength;

        public string LoadContentByPractialID(int practicalId)
        {
            FileStream fs = null;
            FileStream tempFs = null;
            byte[] buffer = null;
            string tempPracticalFilePath = string.Empty;

            try
            {
                long bufferOffset = 0;
                fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);

                for (int i = 0; i < practicalId; i++)
                    bufferOffset += practicalContentBufferLength[i];

                fs.Position = startIndex + bufferOffset;

                tempPracticalFilePath = Path.Combine(Path.GetTempPath(), string.Format("{0}.xls", Guid.NewGuid().ToString()));

                tempFs = new FileStream(tempPracticalFilePath, FileMode.Create, FileAccess.Write);

                while (tempFs.Length != practicalContentBufferLength[practicalId])
                {
                    buffer = new byte[practicalContentBufferLength[practicalId] - tempFs.Length > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : practicalContentBufferLength[practicalId] - tempFs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    tempFs.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                throw new Exception("实操文件加载失败。");
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();

                if (tempFs != null)
                    tempFs.Dispose();
            }

            return tempPracticalFilePath;
        }

        public void SaveContent(string filePath)
        {

        }

        public void SetPracticalProjectDone(int practicalID, int projectID)
        {
            if (!File.Exists(practicalProcessFileName))
                CreateProcessFile();

            StringWriter writer = null;
            FileStream fs = null;

            try
            {
                writer = new StringWriter();
                XmlDocument document = new XmlDocument();

                using (StringReader reader = new StringReader(UTF8Encoding.UTF8.GetString(File.ReadAllBytes(practicalProcessFileName))))
                    document.Load(reader);

                fs = new FileStream(practicalProcessFileName, FileMode.Open, FileAccess.Write);
                XmlNode rootNode = document.SelectSingleNode("process");
                XmlNode practicalNode = null;
                XmlNode projectNode = null;

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["id"].Value == practicalID.ToString())
                    {
                        practicalNode = node;
                        break;
                    }
                }

                if (practicalNode == null)
                {
                    practicalNode = document.CreateElement("practicalitem");
                    XmlAttribute attr = document.CreateAttribute("id");
                    attr.Value = practicalID.ToString();
                    practicalNode.Attributes.Append(attr);
                    rootNode.AppendChild(practicalNode);
                }

                foreach (XmlNode node in practicalNode.ChildNodes)
                {
                    if (node.Attributes["id"].Value == practicalID.ToString())
                    {
                        projectNode = node;
                        break;
                    }
                }

                if (projectNode == null)
                {
                    projectNode = document.CreateElement("project");
                    XmlAttribute idAttr = document.CreateAttribute("id");
                    idAttr.Value = projectID.ToString();
                    projectNode.Attributes.Append(idAttr);
                    XmlAttribute hasDoneAttr = document.CreateAttribute("hasdone");
                    hasDoneAttr.Value = false.ToString();
                    projectNode.Attributes.Append(hasDoneAttr);
                    practicalNode.AppendChild(projectNode);
                }

                projectNode.Attributes["hasdone"].Value = true.ToString();

                document.Save(writer);

                byte[] buffer = UTF8Encoding.UTF8.GetBytes(writer.GetStringBuilder().ToString());
                fs.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                throw new Exception("保存失败。");
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                if (fs != null)
                    fs.Dispose();
            }
        }

        private void CreateProcessFile()
        {
            FileStream fs = null;
            StringWriter writer = null;

            try
            {
                fs = File.Create(practicalProcessFileName);
                writer = new StringWriter();
                XmlDocument document = new XmlDocument();
                document.CreateXmlDeclaration("1.0", "utf-8", "yes");
                XmlNode rootNode = document.CreateElement("process");
                document.AppendChild(rootNode);
                document.Save(writer);
                byte[] buffer = UTF8Encoding.UTF8.GetBytes(writer.GetStringBuilder().ToString());
                fs.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                throw new Exception("保存失败。");
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
                if (writer != null)
                    writer.Dispose();
            }
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

                for (int i = 0; i < items.Length; i++)
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

                startIndex = fs.Position;
                practicalCount = items.Length;

                LoadProcess(items);
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

        private void LoadProcess(PracticalItem[] items)
        {
            if (!File.Exists(practicalProcessFileName))
                return;
            try
            {
                XmlDocument document = new XmlDocument();

                using (StringReader reader = new StringReader(UTF8Encoding.UTF8.GetString(File.ReadAllBytes(practicalProcessFileName))))
                    document.Load(reader);

                foreach (XmlNode practical in document.SelectSingleNode("process"))
                {
                    int practicalId = Convert.ToInt32(practical.Attributes["id"].Value);
                    PracticalItem practicalItem = items.SingleOrDefault(item => item.ID == practicalId);

                    if (practicalItem == null)
                        continue;

                    foreach (XmlNode project in practical.ChildNodes)
                    {
                        int projectId = Convert.ToInt32(project.Attributes["id"].Value);
                        PracticalItemProject projectItem = practicalItem.Projects.SingleOrDefault(item => item.ID == projectId);

                        if (practicalItem == null)
                            continue;

                        projectItem.IsDone = Convert.ToBoolean(project.Attributes["hasdone"].Value);
                    }

                    if (practicalItem.Projects.Count(item => item.IsDone) == 0)
                        practicalItem.State = PracticalStateEnum.Default;
                    else if (practicalItem.Projects.Count(item => item.IsDone) < practicalItem.Projects.Count)
                        practicalItem.State = PracticalStateEnum.OnWorking;
                    else
                        practicalItem.State = PracticalStateEnum.HasDone;
                }
            }
            catch
            {
                throw new Exception("完成进度读取失败。");
            }
        }

        private PracticalManager()
        {
            startIndex = 0;
            practicalCount = 0;
            practicalContentBufferLength = null;

            templateFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMPLATE_FILE_NAME);
            string practicalFileFolder = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PRACTICAL_FILE_FOLDER), SingletonManager.Get<UserProcesser>().GetUser().ID);
            practicalFileName = Path.Combine(practicalFileFolder, string.Format("{0}-{1}.prac", SingletonManager.Get<UserProcesser>().GetUser().ID, SingletonManager.Get<UserProcesser>().GetUser().Name));
            practicalProcessFileName = Path.Combine(practicalFileFolder, PRACTICAL_PROCESS_FILE_NAME);

            if (!Directory.Exists(practicalFileFolder))
                Directory.CreateDirectory(practicalFileFolder);
        }
    }
}
