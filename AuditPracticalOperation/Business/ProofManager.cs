using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;
using System.IO;
using System.Collections.ObjectModel;

namespace Business.Processer
{
    [Singleton]
    internal class ProofManager : IProofManager
    {
        private const string PROOF_INDEX_FILE_NAME = "proof.idx";

        private const string PROOF_FILE_NAME = "proof.dat";

        private readonly string proofIndexFileName;

        private readonly string proofDatFileName;

        private const int BUFFER_LENGTH = 1000;

        private ProofManager()
        {
            proofIndexFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROOF_INDEX_FILE_NAME);
            proofDatFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROOF_FILE_NAME);
        }

        public IList<ProofItem> GetProofs()
        {
            return ConvertToProofItems(GetProofFileFolderRoot());
        }

        private IList<ProofItem> ConvertToProofItems(ProofFileFolder root, ProofItem parent = null)
        {
            IList<ProofItem> proofs = new List<ProofItem>();

            for (int i = 0; i < root.Folders.Count; i++)
            {
                ProofItem proof = new ProofItem();
                proof.Type = FileTypeEnum.Directory;
                proof.Name = root.Folders[i].Name;
                proof.ImgSource = GetImageSource(proof);
                proof.Proofs = new ObservableCollection<ProofItem>();
                proof.Parent = parent;

                IList<ProofItem> children = ConvertToProofItems(root.Folders[i], proof);

                for (int j = 0; j < children.Count; j++)
                    proof.Proofs.Add(children[j]);

                proofs.Add(proof);
            }

            for (int i = 0; i < root.Files.Count; i++)
            {
                ProofItem proof = new ProofItem();
                proof.Type = root.Files[i].FileType;
                proof.Name = root.Files[i].Name;
                proof.StartIndex = root.Files[i].StartIndex;
                proof.Length = root.Files[i].Length;
                proof.ImgSource = GetImageSource(proof);
                proofs.Add(proof);
            }

            return proofs;
        }

        private byte[] GetImageSource(ProofItem proof)
        {
            switch (proof.Type)
            {
                case FileTypeEnum.Directory:
                case FileTypeEnum.Word:
                case FileTypeEnum.Excel:
                    return File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "Images\\file.png");
                case FileTypeEnum.Pdf:
                    return File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "Images\\pdf.png");
                case FileTypeEnum.Img:
                    FileStream datFs = null;
                    try
                    {
                        datFs = new FileStream(proofDatFileName, FileMode.Open, FileAccess.Read);
                        byte[] buffer = new byte[proof.Length];
                        datFs.Position = proof.StartIndex;
                        datFs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                    finally
                    {
                        if (datFs != null)
                            datFs.Dispose();
                    }
                default: throw new Exception("获取证据失败。");
            }
        }

        private ProofFileFolder GetProofFileFolderRoot()
        {
            Dictionary<int, ProofFileFolder> folders = new Dictionary<int, ProofFileFolder>();
            IList<ProofFileFolder> _folders = new List<ProofFileFolder>();

            FileStream idxFs = null;
            byte[] buffer = null;

            int[] folderFileCountArray = null;
            int[] folderNameLengthArray = null;
            int[][] fileNameLengthArray = null;

            try
            {
                idxFs = new FileStream(proofIndexFileName, FileMode.Open, FileAccess.Read);

                buffer = new byte[sizeof(int)];
                idxFs.Read(buffer, 0, buffer.Length);

                int folderCount = BitConverter.ToInt32(buffer, 0);
                folderFileCountArray = new int[folderCount];
                folderNameLengthArray = new int[folderCount];
                fileNameLengthArray = new int[folderCount][];

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    buffer = new byte[sizeof(int)];
                    idxFs.Read(buffer, 0, buffer.Length);
                    folderFileCountArray[i] = BitConverter.ToInt32(buffer, 0);

                    ProofFileFolder folder = new ProofFileFolder();
                    _folders.Add(folder);

                    for (int j = 0; j < folderFileCountArray[i]; j++)
                    {
                        ProofFile proofFile = new ProofFile();
                        folder.Files.Add(proofFile);
                    }
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    buffer = new byte[sizeof(int)];
                    idxFs.Read(buffer, 0, buffer.Length);
                    _folders[i].Index = BitConverter.ToInt32(buffer, 0);
                    folders.Add(_folders[i].Index, _folders[i]);
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    buffer = new byte[sizeof(int)];
                    idxFs.Read(buffer, 0, buffer.Length);
                    _folders[i].ParentIndex = BitConverter.ToInt32(buffer, 0);
                }

                for (int i = 0; i < folderCount; i++)
                {
                    buffer = new byte[sizeof(int)];
                    idxFs.Read(buffer, 0, buffer.Length);
                    folderNameLengthArray[i] = BitConverter.ToInt32(buffer, 0);
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    fileNameLengthArray[i] = new int[folderFileCountArray[i]];
                    for (int j = 0; j < folderFileCountArray[i]; j++)
                    {
                        buffer = new byte[sizeof(int)];
                        idxFs.Read(buffer, 0, buffer.Length);
                        fileNameLengthArray[i][j] = BitConverter.ToInt32(buffer, 0);
                    }
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    buffer = new byte[folderNameLengthArray[i]];
                    idxFs.Read(buffer, 0, buffer.Length);
                    _folders[i].Name = UTF8Encoding.UTF8.GetString(buffer);
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    for (int j = 0; j < folderFileCountArray[i]; j++)
                    {
                        buffer = new byte[fileNameLengthArray[i][j]];
                        idxFs.Read(buffer, 0, buffer.Length);
                        _folders[i].Files[j].Name = UTF8Encoding.UTF8.GetString(buffer);
                    }
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    for (int j = 0; j < folderFileCountArray[i]; j++)
                    {
                        buffer = new byte[sizeof(int)];
                        idxFs.Read(buffer, 0, buffer.Length);
                        _folders[i].Files[j].FileType = (FileTypeEnum)BitConverter.ToInt32(buffer, 0);
                    }
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    for (int j = 0; j < folderFileCountArray[i]; j++)
                    {
                        buffer = new byte[sizeof(long)];
                        idxFs.Read(buffer, 0, buffer.Length);
                        _folders[i].Files[j].StartIndex = BitConverter.ToInt64(buffer, 0);

                        buffer = new byte[sizeof(long)];
                        idxFs.Read(buffer, 0, buffer.Length);
                        _folders[i].Files[j].Length = BitConverter.ToInt64(buffer, 0);
                    }
                }

                for (int i = 0; i < folderFileCountArray.Length; i++)
                {
                    if (_folders[i].ParentIndex == -1)
                        continue;

                    folders[_folders[i].ParentIndex].Folders.Add(_folders[i]);
                }

                return folders[0];
            }
            finally
            {
                if (idxFs != null)
                    idxFs.Dispose();
            }
        }

        public string GetProofTempFilePath(ProofItem proof)
        {
            FileStream datFs = null;
            FileStream tempFs = null;
            byte[] buffer;

            try
            {
                string tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                datFs = new FileStream(proofDatFileName, FileMode.Open, FileAccess.Read);
                tempFs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write);
                datFs.Position = proof.StartIndex;

                while (tempFs.Position < proof.Length)
                {
                    buffer = new byte[proof.Length - tempFs.Position > BUFFER_LENGTH ? BUFFER_LENGTH : proof.Length - tempFs.Position];
                    datFs.Read(buffer, 0, buffer.Length);
                    tempFs.Write(buffer, 0, buffer.Length);
                }

                return tempFileName;
            }
            finally
            {
                if (datFs != null)
                    datFs.Dispose();

                if (tempFs != null)
                    tempFs.Dispose();
            }
        }

        public void DeleteProofTempFile(string tempFilePath)
        {
            File.Delete(tempFilePath);
        }
    }
}
