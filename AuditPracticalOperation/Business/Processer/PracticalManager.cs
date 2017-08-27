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
        private readonly string practicalFileFolder;
        private const string PRACTICAL_PROCESS_FILE_NAME = "process.dat";
        private readonly string practicalProcessFileName;
        private const int BUFFER_PACKAGE_LENGTH = 1000;
        private long startIndex;
        private int practicalCount;
        private long[] practicalContentBufferLength;

        public string LoadContentByPractialID(int practicalID)
        {
            FileStream fs = null;
            FileStream tempFs = null;
            byte[] buffer = null;
            string tempPracticalFilePath = Path.Combine(Path.GetTempPath(), string.Format("{0}.xls", Guid.NewGuid().ToString()));

            try
            {
                if (ExistPract(practicalID))
                {
                    string fileName = Path.Combine(practicalFileFolder, practicalID + ".pracd");
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    tempFs = new FileStream(tempPracticalFilePath, FileMode.Create, FileAccess.Write);

                    fs.Position += sizeof(long);

                    while (fs.Position != fs.Length)
                    {
                        buffer = new byte[fs.Length - fs.Position > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : fs.Length - fs.Position];
                        fs.Read(buffer, 0, buffer.Length);
                        tempFs.Write(buffer, 0, buffer.Length);
                    }
                }
                else
                {
                    long bufferOffset = 0;
                    fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);

                    for (int i = 0; i < practicalID; i++)
                        bufferOffset += practicalContentBufferLength[i];

                    fs.Position = startIndex + bufferOffset;

                    tempFs = new FileStream(tempPracticalFilePath, FileMode.Create, FileAccess.Write);

                    while (tempFs.Length != practicalContentBufferLength[practicalID])
                    {
                        buffer = new byte[practicalContentBufferLength[practicalID] - tempFs.Length > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : practicalContentBufferLength[practicalID] - tempFs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        tempFs.Write(buffer, 0, buffer.Length);
                    }
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

        public void SaveContent(int practicalID, string filePath)
        {
            FileStream readFs = null;
            FileStream writeFs = null;
            byte[] buffer = null;
            string fileName = Path.Combine(practicalFileFolder, practicalID + ".pracd");

            try
            {
                readFs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                writeFs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                buffer = BitConverter.GetBytes(readFs.Length);
                writeFs.Write(buffer, 0, buffer.Length);
                while (readFs.Position != readFs.Length)
                {
                    buffer = new byte[readFs.Length - readFs.Position > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : readFs.Length - readFs.Position];
                    readFs.Read(buffer, 0, buffer.Length);
                    writeFs.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                throw new Exception("保存失败。");
            }
            finally
            {
                if (readFs != null)
                    readFs.Dispose();

                if (writeFs != null)
                    writeFs.Dispose();

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        public void SetPracticalProjectDone(int practicalID, int projectID)
        {
            if (!File.Exists(practicalProcessFileName))
                CreateProcessFile();

            StringWriter writer = null;

            try
            {
                writer = new StringWriter();
                XmlDocument document = new XmlDocument();

                using (StringReader reader = new StringReader(UTF8Encoding.UTF8.GetString(File.ReadAllText(practicalProcessFileName).Split(' ').Select(item => Convert.ToByte(item)).ToArray())))
                    document.Load(reader);

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

                projectNode.Attributes["hasdone"].Value = ExistPract(practicalID).ToString();

                document.Save(writer);

                byte[] buffer = UTF8Encoding.UTF8.GetBytes(writer.GetStringBuilder().ToString());
                File.WriteAllText(practicalProcessFileName, string.Join(" ", buffer.Select(item => item.ToString()).ToArray()));
            }
            catch
            {
                throw new Exception("保存失败。");
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        private bool ExistPract(int practicalID)
        {
            return File.Exists(Path.Combine(practicalFileFolder, practicalID + ".pracd"));
        }

        private void CreateProcessFile()
        {
            StringWriter writer = null;

            try
            {
                writer = new StringWriter();
                XmlDocument document = new XmlDocument();
                document.CreateXmlDeclaration("1.0", "utf-8", "yes");
                XmlNode rootNode = document.CreateElement("process");
                document.AppendChild(rootNode);
                document.Save(writer);
                byte[] buffer = UTF8Encoding.UTF8.GetBytes(writer.GetStringBuilder().ToString());
                File.WriteAllText(practicalProcessFileName, string.Join(" ", buffer.Select(item => item.ToString()).ToArray()));
            }
            catch
            {
                throw new Exception("保存失败。");
            }
            finally
            {
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
            int[][] practicalProjectF1BufferLengthArray = null;
            int[] practicalProjectF2BufferLengthArray = null;
            int[] practicalNameBufferLength = null;

            try
            {
                fs = new FileStream(templateFileName, FileMode.Open, FileAccess.Read);
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                items = new PracticalItem[BitConverter.ToInt32(buffer, 0)];
                practicalProjectCountArray = new int[items.Length];
                practicalProjectContentBufferLengthArray = new int[items.Length][];
                practicalProjectF1BufferLengthArray = new int[items.Length][];
                practicalProjectF2BufferLengthArray = new int[items.Length];
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
                    practicalProjectF1BufferLengthArray[i] = new int[practicalProjectCountArray[i]];
                }
                for (int i = 0; i < items.Length; i++)
                {
                    for (int j = 0; j < practicalProjectCountArray[i]; j++)
                    {
                        PracticalItemProject project = new PracticalItemProject();
                        project.ID = j;
                        items[i].Projects.Add(project);
                    }
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
                    for (int j = 0; j < practicalProjectCountArray[i]; j++)
                    {
                        buffer = new byte[sizeof(int)];
                        fs.Read(buffer, 0, buffer.Length);
                        practicalProjectF1BufferLengthArray[i][j] = BitConverter.ToInt32(buffer, 0);
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    buffer = new byte[sizeof(int)];
                    fs.Read(buffer, 0, buffer.Length);
                    practicalProjectF2BufferLengthArray[i] = BitConverter.ToInt32(buffer, 0);
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
                        items[i].Projects[j].Name = UTF8Encoding.UTF8.GetString(buffer);
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    for (int j = 0; j < practicalProjectCountArray[i]; j++)
                    {
                        buffer = new byte[practicalProjectF1BufferLengthArray[i][j]];
                        fs.Read(buffer, 0, buffer.Length);
                        items[i].Projects[j].HelperText = UTF8Encoding.UTF8.GetString(buffer);
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    buffer = new byte[practicalProjectF2BufferLengthArray[i]];
                    fs.Read(buffer, 0, buffer.Length);
                    items[i].HelperText = UTF8Encoding.UTF8.GetString(buffer);
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

                using (StringReader reader = new StringReader(UTF8Encoding.UTF8.GetString(File.ReadAllText(practicalProcessFileName).Split(' ').Select(item => Convert.ToByte(item)).ToArray())))
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

                        projectItem.IsDone = ExistPract(practicalItem.ID) && Convert.ToBoolean(project.Attributes["hasdone"].Value);
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

        public HasDonePracticalItem[] GetHasDonePractical()
        {
            try
            {
                string[] files = Directory.GetFiles(practicalFileFolder);
                PracticalItem[] items = GetAllPractical();
                IList<HasDonePracticalItem> hasDoneItems = new List<HasDonePracticalItem>();

                for (int i = 0; i < files.Length; i++)
                {
                    if (!files[i].Contains("pracd"))
                        continue;

                    int id = Convert.ToInt32(Path.GetFileNameWithoutExtension(files[i]));

                    if (items[id].State != PracticalStateEnum.HasDone)
                        continue;

                    HasDonePracticalItem hasDoneItem = new HasDonePracticalItem();

                    hasDoneItem.ID = id;
                    hasDoneItem.Name = items[id].Name;
                    hasDoneItem.IsSelected = false;

                    hasDoneItems.Add(hasDoneItem);
                }

                return hasDoneItems.OrderBy(item => item.ID).ToArray();
            }
            catch
            {
                throw new Exception("获取完成结果失败。");
            }
        }

        public void ExportPractical(string fileName, HasDonePracticalItem[] hasDonePracticalItems)
        {
            FileStream[] readFsArray = null;
            FileStream writeFs = null;
            byte[] buffer = null;
            byte[][] practicalFileNameBufferArray = null;

            try
            {
                writeFs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                PracticalItem[] items = GetAllPractical();
                readFsArray = new FileStream[hasDonePracticalItems.Length];
                practicalFileNameBufferArray = new byte[hasDonePracticalItems.Length][];

                for (int i = 0; i < hasDonePracticalItems.Length; i++)
                    readFsArray[i] = new FileStream(Path.Combine(practicalFileFolder, hasDonePracticalItems[i].ID + ".pracd"), FileMode.Open, FileAccess.Read);

                #region 写入学号
                buffer = UTF8Encoding.UTF8.GetBytes(SingletonManager.Get<UserProcesser>().GetUser().ID);
                writeFs.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
                writeFs.Write(buffer, 0, buffer.Length);
                #endregion

                #region 写入姓名
                buffer = UTF8Encoding.UTF8.GetBytes(SingletonManager.Get<UserProcesser>().GetUser().Name);
                writeFs.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
                writeFs.Write(buffer, 0, buffer.Length);
                #endregion

                #region 写入CPU序列号
                buffer = UTF8Encoding.UTF8.GetBytes(SingletonManager.Get<UserProcesser>().GetUser().CPUID);
                writeFs.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
                writeFs.Write(buffer, 0, buffer.Length);
                #endregion

                #region 写入文档个数
                writeFs.Write(BitConverter.GetBytes(hasDonePracticalItems.Length), 0, sizeof(int));
                #endregion

                #region 写入文档名长度
                for (int i = 0; i < hasDonePracticalItems.Length; i++)
                {
                    practicalFileNameBufferArray[i] = UTF8Encoding.UTF8.GetBytes(items[hasDonePracticalItems[i].ID].Name);
                    writeFs.Write(BitConverter.GetBytes(practicalFileNameBufferArray[i].Length), 0, sizeof(int));
                }
                #endregion

                #region 写入文档长度
                for (int i = 0; i < hasDonePracticalItems.Length; i++)
                    writeFs.Write(BitConverter.GetBytes(readFsArray[i].Length - sizeof(long)), 0, sizeof(long));
                #endregion

                #region 写入文档名
                for (int i = 0; i < hasDonePracticalItems.Length; i++)
                    writeFs.Write(practicalFileNameBufferArray[i], 0, practicalFileNameBufferArray[i].Length);
                #endregion

                #region 写入文档
                for (int i = 0; i < hasDonePracticalItems.Length; i++)
                {
                    readFsArray[i].Position = sizeof(long);
                    while (readFsArray[i].Position < readFsArray[i].Length)
                    {
                        buffer = new byte[readFsArray[i].Length - readFsArray[i].Position > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : readFsArray[i].Length - readFsArray[i].Position];
                        readFsArray[i].Read(buffer, 0, buffer.Length);
                        writeFs.Write(buffer, 0, buffer.Length);
                    }
                }
                #endregion
            }
            catch
            {
                throw new Exception("导出失败。");
            }
            finally
            {
                if (writeFs != null)
                    writeFs.Dispose();

                if (readFsArray != null)
                    for (int i = 0; i < readFsArray.Length; i++)
                        if (readFsArray[i] != null)
                            readFsArray[i].Dispose();
            }
        }

        private PracticalManager()
        {
            startIndex = 0;
            practicalCount = 0;
            practicalContentBufferLength = null;

            templateFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMPLATE_FILE_NAME);
            practicalFileFolder = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PRACTICAL_FILE_FOLDER), SingletonManager.Get<UserProcesser>().GetUser().ID);
            practicalProcessFileName = Path.Combine(practicalFileFolder, PRACTICAL_PROCESS_FILE_NAME);

            if (!Directory.Exists(practicalFileFolder))
                Directory.CreateDirectory(practicalFileFolder);
        }
    }
}
