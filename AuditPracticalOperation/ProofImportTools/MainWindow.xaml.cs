using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace ProofImportTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static RoutedUICommand OpenFolderDialog = new RoutedUICommand("Open the Folder Dialog", "OpenFolderDialog", typeof(MainWindow));

        private void OpenFolderDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ProofFileFolder proofFileFolder = GetFolder(fbd.SelectedPath);
                }
            }
        }

        private ProofFileFolder GetFolder(string folderPath)
        {
            ProofFileFolder proofFileFolder = new ProofFileFolder();
            proofFileFolder.Name = System.IO.Path.GetDirectoryName(folderPath);

            foreach (string file in Directory.GetFiles(folderPath))
            {
                ProofFile proofFile = new ProofFile();
                proofFile.Name = System.IO.Path.GetFileNameWithoutExtension(file);
                proofFile.Path = file;
                proofFileFolder.Files.Add(file);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
                proofFileFolder.Folders.Add(GetFolder(folder));

            return proofFileFolder;
        }
    }

    public class ProofFileFolder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;

        private ObservableCollection<string> files;

        private ObservableCollection<ProofFileFolder> folders;

        public string Name { get { return name; } set { name = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }

        public ObservableCollection<string> Files { get { return files; } set { files = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Files")); } }

        public ObservableCollection<ProofFileFolder> Folders { get { return folders; } set { folders = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Folders")); } }

        public ProofFileFolder()
        {
            files = new ObservableCollection<string>();
            folders = new ObservableCollection<ProofFileFolder>();
        }
    }

    public class ProofFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string path;

        private string name;

        public string Path { get { return path; } set { path = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Path")); } }

        public string Name { get { return name; } set { name = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Path")); } }
    }
}
