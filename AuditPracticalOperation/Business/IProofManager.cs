using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace Business.Processer
{
    internal interface IProofManager
    {
        IList<ProofItem> GetProofs();
    }
}
