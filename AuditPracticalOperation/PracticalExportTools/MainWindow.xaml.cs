using Common.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PracticalExportTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<PracticalFile> files;
        private long bufferStartIndex;
        private Student student;
        private string practicalFilePath;

        private const int BUFFER_PACKAGE_LENGTH = 1000;

        public MainWindow()
        {
            files = new ObservableCollection<PracticalFile>();
            student = new Student();
            InitializeComponent();

            if (!this.IsInDesignMode())
                list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = files, Mode = BindingMode.OneWay });
        }

        public static RoutedUICommand OpenFileDialog = new RoutedUICommand("Open the File Dialog", "OpenFileDialog", typeof(MainWindow));

        private void OpenFileDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "实操文件|*.prac";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    practicalFilePath = ofd.FileName;
                    studentInfo.SetBinding(Panel.DataContextProperty, new Binding(".") { Source = Load(practicalFilePath) });
                }
                catch
                {
                    MessageBox.Show("读取失败。");
                }
            }
            ofd.Dispose();
        }

        private Student Load(string practicalFilePath)
        {
            FileStream fs = null;
            byte[] buffer = null;
            bufferStartIndex = 0;

            try
            {
                fs = new FileStream(practicalFilePath, FileMode.Open, FileAccess.Read);

                #region 读取学号
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                int idBufferLength = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[idBufferLength];
                fs.Read(buffer, 0, buffer.Length);
                student.ID = UTF8Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                #endregion

                #region 读取姓名
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                int nameBufferLength = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[nameBufferLength];
                fs.Read(buffer, 0, buffer.Length);
                student.Name = UTF8Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                #endregion

                #region 读取CPU序列号
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                int cpuIDBufferLength = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[cpuIDBufferLength];
                fs.Read(buffer, 0, buffer.Length);
                student.CPUID = UTF8Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                #endregion

                #region 读取实操文件结构
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                int fileCount = BitConverter.ToInt32(buffer, 0);
                int[] fileNameLengthArray = new int[fileCount];
                long[] fileLengthArray = new long[fileCount];
                string[] fileNameArray = new string[fileCount];

                for (int i = 0; i < fileCount; i++)
                {
                    buffer = new byte[sizeof(int)];
                    fs.Read(buffer, 0, buffer.Length);
                    fileNameLengthArray[i] = BitConverter.ToInt32(buffer, 0);
                }

                for (int i = 0; i < fileCount; i++)
                {
                    buffer = new byte[sizeof(long)];
                    fs.Read(buffer, 0, buffer.Length);
                    fileLengthArray[i] = BitConverter.ToInt64(buffer, 0);
                }

                for (int i = 0; i < fileCount; i++)
                {
                    buffer = new byte[fileNameLengthArray[i]];
                    fs.Read(buffer, 0, buffer.Length);
                    fileNameArray[i] = UTF8Encoding.UTF8.GetString(buffer);
                }

                for (int i = 0; i < fileCount; i++)
                {
                    PracticalFile file = new PracticalFile();
                    file.Index = i;
                    file.IsLast = i == fileCount - 1;
                    file.Name = fileNameArray[i];
                    file.FileLength = fileLengthArray[i];
                    files.Add(file);
                }
                #endregion

                bufferStartIndex = fs.Position;

                return student;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }

        public static RoutedUICommand ExportPracticalFile = new RoutedUICommand("Export the Practical File", "ExportPracticalFile", typeof(MainWindow));

        private void ExportPracticalFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            try
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Export(fbd.SelectedPath);
                    MessageBox.Show("导出成功。");
                }
            }
            catch
            {
                MessageBox.Show("导出失败。");
            }
            finally
            {
                fbd.Dispose();
            }
        }

        private void ExportPracticalFileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = files.Count > 0;
        }

        private void Export(string folder)
        {
            string exportFolder = System.IO.Path.Combine(folder, string.Format("{0}-{1}", student.ID, student.Name));

            if (!Directory.Exists(exportFolder))
                Directory.CreateDirectory(exportFolder);

            FileStream fs = null;
            byte[] buffer = null;

            try
            {
                StreamWriter sw = null;
                try
                {
                    string swPath = System.IO.Path.Combine(exportFolder, "学生信息.txt");

                    if (File.Exists(swPath))
                        File.Delete(swPath);

                    sw = File.AppendText(swPath);
                    sw.WriteLine(string.Format("学号：{0}", student.ID));
                    sw.WriteLine(string.Format("姓名：{0}", student.Name));
                    sw.WriteLine(string.Format("CPU序列号：{0}", student.CPUID));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (sw != null)
                        sw.Dispose();
                }

                File.Copy(practicalFilePath, System.IO.Path.Combine(exportFolder, System.IO.Path.GetFileName(practicalFilePath)));
                fs = new FileStream(practicalFilePath, FileMode.Open, FileAccess.Read);
                fs.Position = bufferStartIndex;

                for (int i = 0; i < files.Count; i++)
                {
                    string filePath = System.IO.Path.Combine(exportFolder, files[i].Name + ".xls");
                    FileStream writeFs = null;
                    try
                    {
                        writeFs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                        while (writeFs.Length != files[i].FileLength)
                        {
                            buffer = new byte[files[i].FileLength - writeFs.Length > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : files[i].FileLength - writeFs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            writeFs.Write(buffer, 0, buffer.Length);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (writeFs != null)
                            writeFs.Dispose();
                    }
                }


            }
            catch
            {
                throw;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }
    }

    public class PracticalFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;
        private string name;
        private bool isLast;
        private long fileLength;

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Index"));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public bool IsLast
        {
            get
            {
                return isLast;
            }

            set
            {
                isLast = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsLast"));
            }
        }

        public long FileLength
        {
            get
            {
                return fileLength;
            }
            set
            {
                fileLength = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FileLength"));
            }
        }
    }

    public class Student : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string id;
        private string name;
        private string cpuId;

        public string ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string CPUID
        {
            get
            {
                return cpuId;
            }

            set
            {
                cpuId = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CPUID"));
            }
        }
    }

    public class PracticalFileNumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
