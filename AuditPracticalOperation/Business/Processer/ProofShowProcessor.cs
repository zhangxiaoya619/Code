using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ViewModel;

namespace Business.Processer
{
    [Singleton]
    public class ProofShowProcessor
    {
        private ProofShowProcessor()
        {

        }
        public IList<ProofItem> GetProofItems()
        {
            return GetProofItemsFromFiles().ToList();
        }

        private ProofItem[] GetProofItemsFromFiles()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory + "Proofs\\";
            DirectoryInfo TheFolder = new DirectoryInfo(baseDir);
            ProofItem[] proofs = new ProofItem[TheFolder.GetDirectories().Length];
            int index = 0;
            foreach (var item in TheFolder.GetDirectories())
            {
                if (Directory.Exists(item.FullName))
                {
                    if (index == 0)
                        proofs[index++] = new ProofItem()
                        {
                            IsChecked = true,
                            Name = Regex.Replace(item.Name, @"\d", "").Replace(".", ""),
                            Path = item.FullName,
                            Proofs = null,
                            Type = FileTypeEnum.Directory
                        };
                    else
                        proofs[index++] = new ProofItem()
                        {
                            IsChecked = false,
                            Name = Regex.Replace(item.Name, @"\d", "").Replace(".", ""),
                            Path = item.FullName,
                            Proofs = null,
                            Type = FileTypeEnum.Directory
                        };
                }
            }
            for (int i = 0; i < proofs.Length; i++)
            {
                GetCurrentProofs(ref proofs[i]);
            }
            return proofs;
        }
        private void GetCurrentProofs(ref ProofItem current)
        {
            string baseDir = current.Path;
            DirectoryInfo TheFolder = new DirectoryInfo(baseDir);
            if ((TheFolder.GetDirectories().Length + TheFolder.GetFiles().Length) > 0)
            {
                ObservableCollection<ProofItem> proofs = new ObservableCollection<ProofItem>();
                if (TheFolder.GetDirectories().Length > 0)
                {
                    foreach (var item in TheFolder.GetDirectories())
                    {
                        ProofItem proof = new ProofItem()
                        {
                            IsChecked = true,
                            Name = item.Name,
                            Path = item.FullName,
                            ImgSource = AppDomain.CurrentDomain.BaseDirectory + "Images\\file.png",
                            Proofs = null,
                            Type = FileTypeEnum.Directory
                        };
                        GetCurrentProofs(ref proof);
                        proofs.Add(proof);
                    }
                }
                if (TheFolder.GetFiles().Length > 0)
                {
                    foreach (var item in TheFolder.GetFiles())
                    {
                        string aLastName = item.FullName.Substring(item.FullName.LastIndexOf(".") + 1, (item.FullName.Length - item.FullName.LastIndexOf(".") - 1)).ToLower();

                        ProofItem proof = new ProofItem()
                        {
                            IsChecked = true,
                            Name = item.Name,
                            Path = item.FullName,
                            Proofs = null,
                        };
                        switch (aLastName)
                        {
                            case "jpg":
                            case "jpeg":
                            case "png":
                            case "bmp":
                            case "gif":
                                proof.ImgSource = item.FullName;
                                proof.Type = FileTypeEnum.Img;
                                break;
                            case "doc":
                            case "docx":
                                proof.Type = FileTypeEnum.Word;
                                break;
                            case "xls":
                            case "xlsx":
                                proof.Type = FileTypeEnum.Excel;
                                break;
                            case "pdf":
                                proof.Type = FileTypeEnum.Pdf;
                                break;
                            default:
                                break;
                        }
                        proofs.Add(proof);
                    }
                }
                current.Proofs = proofs;
            }
        }
    }
}
