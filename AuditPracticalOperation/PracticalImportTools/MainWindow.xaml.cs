using Common.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PracticalImportTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string RESOURCE_FILE_NAME = "resource.sub";
        private const int BUFFER_PACKAGE_LENGTH = 1000;
        private ObservableCollection<PracticalFile> files;
        private const string LOAD_PRACTICAL_FILE_FOLDER = "loadpracticalfile";
        private readonly string loadPracticalFileFolder;

        public MainWindow()
        {
            files = new ObservableCollection<PracticalFile>();
            loadPracticalFileFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOAD_PRACTICAL_FILE_FOLDER);

            if (!Directory.Exists(loadPracticalFileFolder))
                Directory.CreateDirectory(loadPracticalFileFolder);

            InitializeComponent();

            if (!this.IsInDesignMode())
                list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = files, Mode = BindingMode.OneWay });
        }

        public static RoutedUICommand Autograph = new RoutedUICommand("Config the Autograph", "Autograph", typeof(MainWindow));

        private void AutographExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PracticalFileAutograph practicalFileAutograph = new PracticalFileAutograph((PracticalFile)e.Parameter);
            practicalFileAutograph.Owner = this;
            practicalFileAutograph.ShowDialog();
        }

        public static RoutedUICommand OpenFileDialog = new RoutedUICommand("Open the File Dialog", "OpenFileDialog", typeof(MainWindow));

        private void OpenFileDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Excel文件|*.xls";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    PracticalFile file = new PracticalFile();
                    file.Path = ofd.FileNames[i];
                    file.Name = System.IO.Path.GetFileNameWithoutExtension(ofd.FileNames[i]);
                    file.Index = files.Count;
                    file.IsLast = i == ofd.FileNames.Length - 1;
                    files.Add(file);
                }
            }
            ofd.Dispose();
        }

        public static RoutedUICommand DeletePracticalFile = new RoutedUICommand("Delete the Practical File", "DeletePracticalFile", typeof(MainWindow));

        private void DeletePraticalFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            files.RemoveAt((int)e.Parameter);

            for (int i = (int)e.Parameter; i < files.Count; i++)
            {
                files[i].Index = i;
                files[i].IsLast = i == files.Count - 1;
            }
        }

        public static RoutedUICommand DeleteAllPracticalFile = new RoutedUICommand("Delete all the Practical File", "DeleteAllPracticalFile", typeof(MainWindow));

        private void DeleteAllPraticalFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            files.Clear();
        }

        public static RoutedUICommand OpenPracticalProject = new RoutedUICommand("Open the Practical Project", "OpenPracticalProject", typeof(MainWindow));

        private void OpenPracticalProjectExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            PracticalProjectEditor practicalProject = new PracticalProjectEditor((PracticalFile)e.Parameter);
            practicalProject.Owner = this;
            practicalProject.ShowDialog();
        }

        public static RoutedUICommand ImportPracticalFile = new RoutedUICommand("Import the Practical File", "ImportPracticalFile", typeof(MainWindow));

        private void ImportPracticalFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            try
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Import(fbd.SelectedPath);
                    MessageBox.Show("生成成功。");
                }
            }
            catch
            {
                MessageBox.Show("生成失败。");
            }
            finally
            {
                fbd.Dispose();
            }
        }

        private void AfterLoadCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = files.Count > 0;
        }

        private void Import(string folder)
        {
            string filePath = string.Empty;
            FileStream fs = null;
            byte[] buffer = null;
            FileStream[] fileFsArray = null;
            byte[][][] projectContentBufferArray = new byte[files.Count][][];
            byte[][][] projectF1BufferArray = new byte[files.Count][][];
            byte[][] projectF2BufferArray = new byte[files.Count][];

            try
            {
                filePath = Path.Combine(folder, RESOURCE_FILE_NAME);
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                #region 写模板个数
                buffer = BitConverter.GetBytes(files.Count);
                fs.Write(buffer, 0, buffer.Length);
                #endregion

                #region 写任务清单个数
                for (int i = 0; i < files.Count; i++)
                {
                    buffer = BitConverter.GetBytes(files[i].Projects.Count);
                    fs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写任务清单内容长度
                for (int i = 0; i < files.Count; i++)
                {
                    projectContentBufferArray[i] = new byte[files[i].Projects.Count][];
                    for (int j = 0; j < files[i].Projects.Count; j++)
                    {
                        projectContentBufferArray[i][j] = UTF8Encoding.UTF8.GetBytes(files[i].Projects[j].Content);
                        buffer = BitConverter.GetBytes(projectContentBufferArray[i][j].Length);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
                #endregion

                #region 写任务清单F1长度
                for (int i = 0; i < files.Count; i++)
                {
                    projectF1BufferArray[i] = new byte[files[i].Projects.Count][];
                    for (int j = 0; j < files[i].Projects.Count; j++)
                    {
                        projectF1BufferArray[i][j] = UTF8Encoding.UTF8.GetBytes(files[i].Projects[j].HelperText);
                        buffer = BitConverter.GetBytes(projectF1BufferArray[i][j].Length);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
                #endregion

                #region 写实操F2帮助长度
                for (int i = 0; i < files.Count; i++)
                {
                    projectF2BufferArray[i] = UTF8Encoding.UTF8.GetBytes(files[i].HelperText);
                    buffer = BitConverter.GetBytes(projectF2BufferArray[i].Length);
                    fs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写模板名长度
                byte[][] fileNameBufferArray = new byte[files.Count][];
                for (int i = 0; i < files.Count; i++)
                {
                    fileNameBufferArray[i] = UTF8Encoding.UTF8.GetBytes(files[i].Name);
                    buffer = BitConverter.GetBytes(fileNameBufferArray[i].Length);
                    fs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写模板长度
                fileFsArray = new FileStream[files.Count];
                for (int i = 0; i < files.Count; i++)
                {
                    fileFsArray[i] = new FileStream(files[i].Path, FileMode.Open, FileAccess.Read);
                    buffer = BitConverter.GetBytes(fileFsArray[i].Length);
                    fs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写任务清单内容
                for (int i = 0; i < files.Count; i++)
                    for (int j = 0; j < files[i].Projects.Count; j++)
                        fs.Write(projectContentBufferArray[i][j], 0, projectContentBufferArray[i][j].Length);
                #endregion

                #region 写任务清单F1帮助
                for (int i = 0; i < files.Count; i++)
                    for (int j = 0; j < files[i].Projects.Count; j++)
                        fs.Write(projectF1BufferArray[i][j], 0, projectF1BufferArray[i][j].Length);
                #endregion

                #region 写实操F2帮助
                for (int i = 0; i < files.Count; i++)
                    fs.Write(projectF2BufferArray[i], 0, projectF2BufferArray[i].Length);
                #endregion

                #region 写模板名
                for (int i = 0; i < files.Count; i++)
                    fs.Write(fileNameBufferArray[i], 0, fileNameBufferArray[i].Length);
                #endregion

                #region 写模板内容
                for (int i = 0; i < files.Count; i++)
                {
                    while (fileFsArray[i].Position < fileFsArray[i].Length)
                    {
                        buffer = new byte[fileFsArray[i].Length - fileFsArray[i].Position > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : fileFsArray[i].Length - fileFsArray[i].Position];
                        fileFsArray[i].Read(buffer, 0, buffer.Length);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
                #endregion
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();

                if (fileFsArray != null)
                    for (int i = 0; i < fileFsArray.Length; i++)
                        fileFsArray[i].Dispose();
            }
        }

        public static RoutedUICommand LoadPracticalFile = new RoutedUICommand("Load the Practical File", "LoadPracticalFile", typeof(MainWindow));

        private void LoadPracticalFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "题库文件|*.sub";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadPractical(ofd.FileName);

            ofd.Dispose();
        }

        private void LoadPractical(string practicalFilePath)
        {
            FileStream fs = null;
            byte[] buffer = null;
            PracticalFile[] items = null;
            int[] practicalProjectCountArray = null;
            int[][] practicalProjectContentBufferLengthArray = null;
            int[][] practicalProjectF1BufferLengthArray = null;
            int[] practicalProjectF2BufferLengthArray = null;
            int[] practicalNameBufferLength = null;
            long[] practicalContentBufferLength = null;

            try
            {
                fs = new FileStream(practicalFilePath, FileMode.Open, FileAccess.Read);
                buffer = new byte[sizeof(int)];
                fs.Read(buffer, 0, buffer.Length);
                items = new PracticalFile[BitConverter.ToInt32(buffer, 0)];
                practicalProjectCountArray = new int[items.Length];
                practicalProjectContentBufferLengthArray = new int[items.Length][];
                practicalProjectF1BufferLengthArray = new int[items.Length][];
                practicalProjectF2BufferLengthArray = new int[items.Length];
                practicalNameBufferLength = new int[items.Length];
                practicalContentBufferLength = new long[items.Length];

                for (int i = 0; i < items.Length; i++)
                    items[i] = new PracticalFile();

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
                        PracticalProject project = new PracticalProject();
                        project.Index = j;
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
                        items[i].Projects[j].Content = UTF8Encoding.UTF8.GetString(buffer);
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
                    items[i].Index = i;
                    items[i].Name = UTF8Encoding.UTF8.GetString(buffer);
                    items[i].Path = System.IO.Path.Combine(loadPracticalFileFolder, items[i].Name + ".xls");

                    for (int j = 0; j < practicalProjectCountArray[i]; j++)
                        items[i].Projects[j].PracticalName = items[i].Name;
                }
                for (int i = 0; i < items.Length; i++)
                {
                    FileStream writeFs = null;
                    try
                    {
                        writeFs = new FileStream(items[i].Path, FileMode.Create, FileAccess.Write);
                        while (writeFs.Length != practicalContentBufferLength[i])
                        {
                            buffer = new byte[practicalContentBufferLength[i] - writeFs.Length > BUFFER_PACKAGE_LENGTH ? BUFFER_PACKAGE_LENGTH : practicalContentBufferLength[i] - writeFs.Length];
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

                for (int i = 0; i < items.Length; i++)
                    files.Add(items[i]);
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
        }

        private void BeforeLoadCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = files.Count == 0;
        }

        public static RoutedUICommand HelperEdit = new RoutedUICommand("Edit the Helper", "HelperEdit", typeof(MainWindow));

        private void HelperEditExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            HelperEditor editor = new HelperEditor((PracticalFile)e.Parameter, (HelperTypeEnum)((Button)e.OriginalSource).Tag);
            editor.Owner = this;
            editor.ShowDialog();
        }
    }

    public interface IHelper
    {
        string HelperText { get; set; }

        string Title { get; }
    }

    public class PracticalFile : INotifyPropertyChanged, IHelper
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;
        private string name = string.Empty;
        private string path = string.Empty;
        private bool isLast;
        private ObservableCollection<PracticalProject> projects;
        private ObservableCollection<Autograph> autographs;

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

        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Path"));
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

        public ObservableCollection<PracticalProject> Projects
        {
            get
            {
                return projects;
            }

            set
            {
                projects = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Projects"));
            }
        }

        public ObservableCollection<Autograph> Autographs
        {
            get
            {
                return autographs;
            }
            set
            {
                autographs = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Autographs"));
            }
        }

        private string helperText = string.Empty;

        public string HelperText { get { return helperText; } set { helperText = value; } }

        public string Title { get { return name; } }

        public PracticalFile()
        {
            projects = new ObservableCollection<PracticalProject>();
            autographs = new ObservableCollection<Autograph>();
        }
    }

    public class PracticalProject : INotifyPropertyChanged, IHelper
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;
        private string content = string.Empty;
        private bool isLast;

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

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Content"));

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

        private string helperText = string.Empty;

        public string HelperText { get { return helperText; } set { helperText = value; } }

        public string PracticalName { get; set; }

        public string Title { get { return PracticalName + "-" + content; } }
    }

    public class Autograph : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string sheetName;
        private int rowIndex;
        private int colIndex;

        public string SheetName
        {
            get
            {
                return sheetName;
            }

            set
            {
                sheetName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SheetName"));
            }
        }

        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RowIndex"));
            }
        }

        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ColIndex"));
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

