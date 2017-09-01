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
        IProofManager proofManager;

        public IList<ProofItem> GetProofItems()
        {
            return proofManager.GetProofs();
        }

        public string GetProofTempFilePath(ProofItem proof)
        {
            return proofManager.GetProofTempFilePath(proof);
        }

        public void DeleteProofTempFile(string tempFilePath)
        {
            proofManager.DeleteProofTempFile(tempFilePath);
        }

        private ProofShowProcessor()
        {
            proofManager = SingletonManager.Get<ProofManager>();
        }
    }
}
