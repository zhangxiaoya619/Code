using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;
using System.IO;

namespace Business.Processer
{
    [Singleton]
    internal class ProofManager : IProofManager
    {
        private const string PROOF_INDEX_FILE_NAME = "proof.idx";

        private const string PROOF_FILE_NAME = "proof.dat";

        private readonly string proofIndexFileName;

        private readonly string proofDatFileName;

        private ProofManager()
        {
            proofIndexFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROOF_INDEX_FILE_NAME);
            proofDatFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROOF_FILE_NAME);
        }

        public IList<ProofItem> GetProofs()
        {
            Dictionary<int, ProofFileFolder> folders = new Dictionary<int, ProofFileFolder>();
            IList<ProofFileFolder> _folders = new List<ProofFileFolder>();

            FileStream idxFs = null;
            FileStream datFs = null;
            byte[] buffer = null;

            int[] folderFileCountArray = null;
            int[] folderNameLengthArray = null;
            int[][] fileNameLengthArray = null;

            try
            {
                idxFs = new FileStream(proofIndexFileName, FileMode.Open, FileAccess.Read);
                datFs = new FileStream(proofDatFileName, FileMode.Open, FileAccess.Read);

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
                    for (int j = 0; i < folderFileCountArray[i]; j++)
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
                    folders.Add(_folders[i].Index, _folders[i]);

                return null;
            }
            catch
            {
                throw new Exception("获取证据失败。");
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
