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


        public MainWindow()
        {
            files = new ObservableCollection<PracticalFile>();

            InitializeComponent();

            if (!this.IsInDesignMode())
                list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Source = files, Mode = BindingMode.OneWay });
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

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = files.Count > 0;
        }

        private void Import(string folder)
        {
            string filePath = string.Empty;
            FileStream fs = null;
            byte[] buffer = null;
            FileStream[] fileFsArray = null;

            try
            {
                filePath = Path.Combine(folder, RESOURCE_FILE_NAME);
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                #region 写模板个数
                buffer = BitConverter.GetBytes(files.Count);
                fs.Write(buffer, 0, buffer.Length);
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
    }

    public class PracticalFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;
        private string name;
        private string path;
        private bool isLast;
        private ObservableCollection<PracticalProject> projects;

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

        public PracticalFile()
        {
            projects = new ObservableCollection<PracticalProject>();
        }
    }

    public class PracticalProject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int index;
        private string content;
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

