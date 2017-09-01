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
using ViewModel;

namespace ProofImportTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProofFileFolder proofFileFolder;

        private IList<ProofFileFolder> proofFileFolderArr;

        private const string PROOF_INDEX_FILE_NAME = "proof.idx";

        private const string PROOF_FILE_NAME = "proof.dat";

        private const int BUFFER_LENGTH = 1000;

        public MainWindow()
        {
            proofFileFolderArr = new List<ProofFileFolder>();
            InitializeComponent();
        }

        public static RoutedUICommand OpenFolderDialog = new RoutedUICommand("Open the Folder Dialog", "OpenFolderDialog", typeof(MainWindow));

        private void OpenFolderDialogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            proofFileFolderArr.Clear();

            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    fbd.ShowNewFolderButton = false;

                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        int index = 0;
                        proofFileFolder = GetFolder(fbd.SelectedPath, ref index);
                    }
                }
            }
            catch
            {
                MessageBox.Show("读取失败。");
            }
        }


        private ProofFileFolder GetFolder(string folderPath, ref int index, int parentIndex = -1)
        {
            ProofFileFolder proofFileFolder = new ProofFileFolder();
            proofFileFolder.Name = folderPath.Replace(System.IO.Path.GetDirectoryName(folderPath), string.Empty).Substring(1);
            proofFileFolder.ParentIndex = parentIndex;
            proofFileFolder.Index = index++;

            proofFileFolderArr.Add(proofFileFolder);

            foreach (string file in Directory.GetFiles(folderPath))
            {
                ProofFile proofFile = new ProofFile();
                proofFile.Name = System.IO.Path.GetFileNameWithoutExtension(file);
                proofFile.Path = file;
                switch (System.IO.Path.GetExtension(file).ToLower())
                {
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".bmp":
                    case ".gif":
                        proofFile.FileType = FileTypeEnum.Img; break;
                    case ".doc":
                    case ".docx":
                        proofFile.FileType = FileTypeEnum.Word; break;
                    case "xls":
                    case "xlsx":
                        proofFile.FileType = FileTypeEnum.Excel; break;
                    case ".pdf":
                        proofFile.FileType = FileTypeEnum.Pdf; break;
                    default: throw new Exception("未知文件格式");
                }
                proofFileFolder.Files.Add(proofFile);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
                proofFileFolder.Folders.Add(GetFolder(folder, ref index, proofFileFolder.Index));

            return proofFileFolder;
        }

        public static RoutedUICommand ImportProof = new RoutedUICommand("Open the Proof File", "ImportProof", typeof(MainWindow));

        private void ImportProofExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            try
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    Import(fbd.SelectedPath);

                MessageBox.Show("生成成功。");
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

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = proofFileFolder != null;
        }

        private void Import(string fileFolder)
        {
            FileStream idxFs = null;
            FileStream datFs = null;
            byte[] buffer = null;
            byte[][] folderNameBufferArray = null;
            byte[][][] fileNameBufferArray = null;
            long startIndex = 0;

            try
            {
                idxFs = new FileStream(System.IO.Path.Combine(fileFolder, PROOF_INDEX_FILE_NAME), FileMode.Create, FileAccess.Write);
                datFs = new FileStream(System.IO.Path.Combine(fileFolder, PROOF_FILE_NAME), FileMode.Create, FileAccess.Write);
                folderNameBufferArray = new byte[proofFileFolderArr.Count][];
                fileNameBufferArray = new byte[proofFileFolderArr.Count][][];

                #region 写文件夹个数
                buffer = BitConverter.GetBytes(proofFileFolderArr.Count);
                idxFs.Write(buffer, 0, buffer.Length);
                #endregion

                #region 写文件个数
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    fileNameBufferArray[i] = new byte[proofFileFolderArr[i].Files.Count][];
                    buffer = BitConverter.GetBytes(proofFileFolderArr[i].Files.Count);
                    idxFs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写文件夹Index
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    buffer = BitConverter.GetBytes(proofFileFolderArr[i].Index);
                    idxFs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写文件夹ParentIndex
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    buffer = BitConverter.GetBytes(proofFileFolderArr[i].ParentIndex);
                    idxFs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写文件夹名长度
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    folderNameBufferArray[i] = UTF8Encoding.UTF8.GetBytes(proofFileFolderArr[i].Name);
                    buffer = BitConverter.GetBytes(folderNameBufferArray[i].Length);
                    idxFs.Write(buffer, 0, buffer.Length);
                }
                #endregion

                #region 写文件名长度
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    for (int j = 0; j < fileNameBufferArray[i].Length; j++)
                    {
                        fileNameBufferArray[i][j] = UTF8Encoding.UTF8.GetBytes(proofFileFolderArr[i].Files[j].Name);
                        buffer = BitConverter.GetBytes(fileNameBufferArray[i][j].Length);
                        idxFs.Write(buffer, 0, buffer.Length);
                    }
                }
                #endregion

                #region 写文件夹名
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                    idxFs.Write(folderNameBufferArray[i], 0, folderNameBufferArray[i].Length);
                #endregion

                #region 写文件名
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                    for (int j = 0; j < fileNameBufferArray[i].Length; j++)
                        idxFs.Write(fileNameBufferArray[i][j], 0, fileNameBufferArray[i][j].Length);
                #endregion

                #region 写文件类型
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    for (int j = 0; j < proofFileFolderArr[i].Files.Count; j++)
                    {
                        buffer = BitConverter.GetBytes((int)proofFileFolderArr[i].Files[j].FileType);
                        idxFs.Write(buffer, 0, buffer.Length);
                    }
                }
                #endregion

                #region 写文件索引及文件内容
                for (int i = 0; i < proofFileFolderArr.Count; i++)
                {
                    for (int j = 0; j < proofFileFolderArr[i].Files.Count; j++)
                    {
                        using (FileStream fs = new FileStream(proofFileFolderArr[i].Files[j].Path, FileMode.Open, FileAccess.Read))
                        {
                            buffer = BitConverter.GetBytes(startIndex);
                            idxFs.Write(buffer, 0, buffer.Length);
                            buffer = BitConverter.GetBytes(fs.Length);
                            idxFs.Write(buffer, 0, buffer.Length);

                            while (fs.Position != fs.Length)
                            {
                                buffer = new byte[fs.Length - fs.Position > BUFFER_LENGTH ? BUFFER_LENGTH : fs.Length - fs.Position];
                                fs.Read(buffer, 0, buffer.Length);
                                datFs.Write(buffer, 0, buffer.Length);
                            }

                            startIndex += fs.Length;
                        }
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
                if (idxFs != null)
                    idxFs.Dispose();

                if (datFs != null)
                    datFs.Dispose();
            }
        }
    }
}
